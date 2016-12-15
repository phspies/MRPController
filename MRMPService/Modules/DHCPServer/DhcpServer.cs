using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Net.NetworkInformation;
using System.Diagnostics;
using MRMPService.LocalDatabase;
using MRMPService.MRMPService.Log;

namespace Modules.DHCPServer.Library
{
    public class DhcpServer
    {
        private const Int32 DhcpPort = 67;
        private const Int32 DhcpClientPort = 68;
        private const Int32 DhcpMessageMaxSize = 1024;

        private TimeSpan m_OfferTimeout = TimeSpan.FromSeconds(30);
        private TimeSpan m_LeaseDuration = TimeSpan.FromDays(1);

        private NetworkInterface m_DhcpInterface;
        private IPAddress m_DhcpInterfaceAddress;

        private InternetAddress m_StartAddress = new InternetAddress(192, 168, 1, 100);
        private InternetAddress m_EndAddress = new InternetAddress(192, 168, 1, 150);
        private InternetAddress m_Subnet = new InternetAddress(255, 255, 255, 255);
        private InternetAddress m_Gateway = new InternetAddress(192, 168, 1, 1);
        private String m_DnsSuffix;
        private List<InternetAddress> m_DnsServers = new List<InternetAddress>();
        private SortedList<PhysicalAddress, Boolean> m_Acl = new SortedList<PhysicalAddress, Boolean>();
        private Boolean m_AllowAny = true;
        private Dictionary<PhysicalAddress, InternetAddress> m_Reservations = new Dictionary<PhysicalAddress, InternetAddress>();

        private Object m_LeaseSync = new Object();
        private ReaderWriterLock m_AclLock = new ReaderWriterLock();
        private ReaderWriterLock m_AbortLock = new ReaderWriterLock();
        private Socket m_DhcpSocket;
        private Boolean m_Abort = false;

        private Timer m_CleanupTimer;

        private Dictionary<InternetAddress, AddressLease> m_ActiveLeases = new Dictionary<InternetAddress, AddressLease>();
        private SortedList<InternetAddress, AddressLease> m_InactiveLeases = new SortedList<InternetAddress, AddressLease>();

        public TimeSpan OfferTimeout
        {
            get { return this.m_OfferTimeout; }
            set { this.m_OfferTimeout = value; }
        }

        public TimeSpan LeaseDuration
        {
            get { return this.m_LeaseDuration; }
            set { this.m_LeaseDuration = value; }
        }

        public NetworkInterface DhcpInterface
        {
            get { return this.m_DhcpInterface; }
            set { this.m_DhcpInterface = value; }
        }

        public InternetAddress StartAddress
        {
            get { return this.m_StartAddress; }
            set { this.m_StartAddress = value; }
        }

        public InternetAddress EndAddress
        {
            get { return this.m_EndAddress; }
            set { this.m_EndAddress = value; }
        }

        public InternetAddress Subnet
        {
            get { return this.m_Subnet; }
            set { this.m_Subnet = value; }
        }

        public InternetAddress Gateway
        {
            get { return this.m_Gateway; }
            set { this.m_Gateway = value; }
        }

        public String DnsSuffix
        {
            get { return this.m_DnsSuffix; }
            set { this.m_DnsSuffix = value; }
        }

        public List<InternetAddress> DnsServers
        {
            get { return this.m_DnsServers; }
        }

        public Boolean AllowAny
        {
            get { return this.m_AllowAny; }
            set { this.m_AllowAny = value; }
        }

        public Dictionary<PhysicalAddress, InternetAddress> Reservations
        {
            get { return this.m_Reservations; }
        }

        public DhcpServer()
        {
        }

        public void AddAcl(PhysicalAddress address, Boolean deny)
        {
            this.m_AclLock.AcquireWriterLock(-1);

            try
            {
                if (this.m_Acl.ContainsKey(address))
                {
                    this.m_Acl[address] = !deny;
                }
                else
                {
                    this.m_Acl.Add(address, !deny);
                }
            }
            finally
            {
                this.m_AclLock.ReleaseLock();
            }
        }

        public void RemoveAcl(PhysicalAddress address)
        {
            this.m_AclLock.AcquireWriterLock(-1);

            try
            {
                if (this.m_Acl.ContainsKey(address))
                {
                    this.m_Acl.Remove(address);
                }
            }
            finally
            {
                this.m_AclLock.ReleaseLock();
            }
        }

        public void ClearAcls()
        {
            this.m_AclLock.AcquireWriterLock(-1);

            try
            {
                this.m_Acl.Clear();
            }
            finally
            {
                this.m_AclLock.ReleaseLock();
            }
        }

        public void Start()
        {
            this.m_ActiveLeases.Clear();
            this.m_InactiveLeases.Clear();


            this.m_Abort = false;

            this.m_CleanupTimer = new Timer(new TimerCallback(this.CleanUp), null, 60000, 30000);

            this.m_DhcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            this.m_DhcpSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);
            this.m_DhcpSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
            this.m_DhcpSocket.Bind(new IPEndPoint(IPAddress.Parse("0.0.0.0"), DhcpPort));

            m_DhcpInterfaceAddress = IPAddress.Parse("0.0.0.0");

            this.Listen();
        }

        public void Stop()
        {
            this.m_AbortLock.AcquireWriterLock(-1);

            try
            {
                this.m_Abort = true;

                this.m_CleanupTimer.Dispose();

                this.m_DhcpSocket.Close();
                this.m_DhcpSocket = null;
            }
            finally
            {
                this.m_AbortLock.ReleaseLock();
            }
        }

        private void Listen()
        {
            Byte[] messageBufer = new Byte[DhcpMessageMaxSize];
            EndPoint source = new IPEndPoint(0, 0);

            this.m_AbortLock.AcquireReaderLock(-1);

            try
            {
                if (this.m_Abort)
                {
                    return;
                }

                this.m_DhcpSocket.BeginReceiveFrom(messageBufer, 0, DhcpMessageMaxSize, SocketFlags.None, ref source, new AsyncCallback(this.OnReceive), messageBufer);
            }
            finally
            {
                this.m_AbortLock.ReleaseLock();
            }
        }

        private void CleanUp(Object state)
        {
            lock (this.m_LeaseSync)
            {
                List<AddressLease> toRemove = new List<AddressLease>();

                foreach (AddressLease lease in this.m_ActiveLeases.Values)
                {
                    if (!lease.Acknowledged && lease.Expiration > DateTime.Now.Add(this.m_OfferTimeout) ||
                        lease.Acknowledged && lease.Expiration < DateTime.Now)
                    {
                        toRemove.Add(lease);
                    }
                }

                foreach (AddressLease lease in toRemove)
                {
                    this.m_ActiveLeases.Remove(lease.Address);
                    lease.Acknowledged = false;
                    this.m_InactiveLeases.Add(lease.Address, lease);
                }
            }
        }

        private void OnReceive(IAsyncResult result)
        {
            DhcpData data = new DhcpData((Byte[])result.AsyncState);
            data.Result = result;

            if (!this.m_Abort)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(this.CompleteRequest), data);

                this.Listen();
            }
        }

        private void CompleteRequest(Object state)
        {
            DhcpData messageData = (DhcpData)state;
            EndPoint source = new IPEndPoint(0, 0);

            this.m_AbortLock.AcquireReaderLock(-1);

            try
            {
                if (this.m_Abort)
                {
                    return;
                }

                messageData.BufferSize = this.m_DhcpSocket.EndReceiveFrom(messageData.Result, ref source);
                messageData.Source = (IPEndPoint)source;
            }
            finally
            {
                this.m_AbortLock.ReleaseLock();
            }

            DhcpMessage message;

            try
            {
                message = new DhcpMessage(messageData);
            }
            catch (ArgumentException ex)
            {
                TraceException("Error Parsing Dhcp Message", ex);
                return;
            }
            catch (InvalidCastException ex)
            {
                TraceException("Error Parsing Dhcp Message", ex);
                return;
            }
            catch (IndexOutOfRangeException ex)
            {
                TraceException("Error Parsing Dhcp Message", ex);
                return;
            }
            catch (Exception ex)
            {
                TraceException("Error Parsing Dhcp Message", ex);
                throw;
            }

            if (message.Operation == DhcpOperation.BootRequest)
            {
                Byte[] messageTypeData = message.GetOptionData(DhcpOption.DhcpMessageType);

                if (messageTypeData != null && messageTypeData.Length == 1)
                {
                    DhcpMessageType messageType = (DhcpMessageType)messageTypeData[0];

                    switch (messageType)
                    {
                        case DhcpMessageType.Discover:
                            Trace.TraceInformation("{0} Dhcp DISCOVER Message Received.", Thread.CurrentThread.ManagedThreadId);
                            this.DhcpDiscover(message);
                            Trace.TraceInformation("{0} Dhcp DISCOVER Message Processed.", Thread.CurrentThread.ManagedThreadId);
                            break;
                        case DhcpMessageType.Request:
                            Trace.TraceInformation("{0} Dhcp REQUEST Message Received.", Thread.CurrentThread.ManagedThreadId);
                            this.DhcpRequest(message);
                            Trace.TraceInformation("{0} Dhcp REQUEST Message Processed.", Thread.CurrentThread.ManagedThreadId);
                            break;
                        default:
                            Trace.TraceWarning("Unknown Dhcp Message ({0}) Received, Ignoring.", messageType.ToString());
                            break;
                    }
                }
                else
                {
                    Trace.TraceWarning("Unknown Dhcp Data Received, Ignoring.");
                }
            }
        }

        private static void TraceException(String prefix, Exception ex)
        {
            Trace.TraceError("{0}: ({1}) - {2}\r\n{3}", prefix, ex.GetType().Name, ex.Message, ex.StackTrace);

            if (ex.InnerException != null)
            {
                TraceException("    Inner Exception", ex.InnerException);
            }
        }

        private void DhcpDiscover(DhcpMessage message)
        {
            //dont respond to DHCP requests if we have nothing to offer
            using (DHCPLeaseSet _dhcp_db = new DHCPLeaseSet())
            {
                if (_dhcp_db.ModelRepository.Get().Count == 0)
                {
                    return;
                }
            }
            Byte[] addressRequestData = message.GetOptionData(DhcpOption.AddressRequest);
            if (addressRequestData == null)
            {
                addressRequestData = message.ClientAddress;
            }

            InternetAddress addressRequest = new InternetAddress(addressRequestData);

            // Assume we're on an ethernet network
            Byte[] hardwareAddressData = new Byte[6];
            Array.Copy(message.ClientHardwareAddress, hardwareAddressData, 6);
            PhysicalAddress clientHardwareAddress = new PhysicalAddress(hardwareAddressData);

            AddressLease offer = null;
            string _physical_address = clientHardwareAddress.HumanString();
            Logger.log(String.Format("DHCP: Received DHCP Discover Request from {0}", _physical_address), Logger.Severity.Debug);
            String _hostname = Encoding.UTF8.GetString(message.GetOptionData(DhcpOption.Hostname));

            DHCPLease _dhcp_lease;
            using (DHCPLeaseSet _dhcp_db = new DHCPLeaseSet())
            {
                _dhcp_lease = _dhcp_db.ModelRepository.GetFirstOrDefault(x => x.hostname == _hostname || x.macaddress == _physical_address);
            }
            if (_dhcp_lease != null)
            {
                offer = new AddressLease(clientHardwareAddress, InternetAddress.Parse(_dhcp_lease.ipv4address), DateTime.Now.Add(this.m_LeaseDuration));
                Logger.log(String.Format("DHCP: Respond to DHCP Discover from {0} {1}", _hostname, _physical_address), Logger.Severity.Debug);
            }

            if (offer == null)
            {
                this.SendNak(message);
            }
            else
            {
                offer.Acknowledged = false;
                offer.Expiration = DateTime.Now.Add(this.m_OfferTimeout);
                offer.SessionId = message.SessionId;
                offer.Owner = clientHardwareAddress;
                this.SendOffer(message, offer);
            }
        }

        private void DhcpRequest(DhcpMessage message)
        {
            //dont respond to DHCP requests if we have nothing to offer
            using (DHCPLeaseSet _dhcp_db = new DHCPLeaseSet())
            {
                if (_dhcp_db.ModelRepository.Get().Count == 0)
                {
                    return;
                }
            }
            Byte[] addressRequestData = message.GetOptionData(DhcpOption.AddressRequest);
            if (addressRequestData == null)
            {
                addressRequestData = message.ClientAddress;
            }

            InternetAddress addressRequest = new InternetAddress(addressRequestData);

            if (addressRequest.IsEmpty)
            {
                this.SendNak(message);
                return;
            }

            // Assume we're on an ethernet network
            Byte[] hardwareAddressData = new Byte[6];
            Array.Copy(message.ClientHardwareAddress, hardwareAddressData, 6);
            PhysicalAddress clientHardwareAddress = new PhysicalAddress(hardwareAddressData);

            AddressLease assignment = null;
            String _hostname = Encoding.UTF8.GetString(message.GetOptionData(DhcpOption.Hostname));
            string _physical_address = Encoding.UTF8.GetString(hardwareAddressData);

            Logger.log(String.Format("DHCP: Received DHCP Request from {0}", _physical_address), Logger.Severity.Debug);


            Boolean ack = false;

            DHCPLease _dhcp_lease;
            using (DHCPLeaseSet _dhcp_db = new DHCPLeaseSet())
            {
                _dhcp_lease = _dhcp_db.ModelRepository.GetFirstOrDefault(x => x.hostname == _hostname || x.macaddress == _physical_address);
            }
            if (_dhcp_lease != null)
            {
                assignment = new AddressLease(clientHardwareAddress, InternetAddress.Parse(_dhcp_lease.ipv4address), DateTime.Now.Add(this.m_LeaseDuration));
                if (addressRequest.Equals(assignment.Address))
                {
                    ack = true;
                    Logger.log(String.Format("DHCP: Respond to DHCP Discover from {0} {1}", _hostname, _physical_address), Logger.Severity.Debug);
                }
            }

            if (ack)
            {
                this.SendAck(message, assignment);
            }
            else
            {
                this.SendNak(message);
            }
        }

        private void SendOffer(DhcpMessage message, AddressLease offer)
        {
            //dont respond to DHCP requests if we have nothing to offer
            using (DHCPLeaseSet _dhcp_db = new DHCPLeaseSet())
            {
                if (_dhcp_db.ModelRepository.Get().Count == 0)
                {
                    return;
                }
            }
            DhcpMessage response = new DhcpMessage();
            response.Operation = DhcpOperation.BootReply;
            response.Hardware = HardwareType.Ethernet;
            response.HardwareAddressLength = 6;
            response.SecondsElapsed = message.SecondsElapsed;
            response.SessionId = message.SessionId;
            response.Flags = message.Flags;

            response.ClientHardwareAddress = message.ClientHardwareAddress;

            String _hostname = Encoding.UTF8.GetString(message.GetOptionData(DhcpOption.Hostname));
            string _physical_address = Encoding.UTF8.GetString(message.ClientHardwareAddress);



            DHCPLease _dhcp_lease;
            using (DHCPLeaseSet _dhcp_db = new DHCPLeaseSet())
            {
                _dhcp_lease = _dhcp_db.ModelRepository.GetFirstOrDefault(x => x.hostname == _hostname || x.macaddress == _physical_address);
            }
            if (_dhcp_lease != null)
            {
                response.AssignedAddress = Encoding.ASCII.GetBytes(_dhcp_lease.ipv4address);

                response.AddOption(DhcpOption.DhcpMessageType, (Byte)DhcpMessageType.Offer);
                response.AddOption(DhcpOption.AddressRequest, Encoding.ASCII.GetBytes(_dhcp_lease.ipv4address));
                response.AddOption(DhcpOption.NameServer, Encoding.ASCII.GetBytes(_dhcp_lease.dnsservers));
                response.AddOption(DhcpOption.DomainNameServer, Encoding.ASCII.GetBytes(_dhcp_lease.dnsservers));
                response.AddOption(DhcpOption.Router, Encoding.ASCII.GetBytes(_dhcp_lease.ipv4gateway));
                response.AddOption(DhcpOption.SubnetMask, Encoding.ASCII.GetBytes(_dhcp_lease.ipv4mask));
                response.AddOption(DhcpOption.DomainNameSuffix, Encoding.ASCII.GetBytes(_dhcp_lease.dnsdomains));

                AddDhcpOptions(response);

                Byte[] paramList = message.GetOptionData(DhcpOption.ParameterList);
                if (paramList != null)
                {
                    response.OptionOrdering = paramList;
                }
                Logger.log(String.Format("DHCP: Send DHCP Offer to {0} {1} {2}", _hostname, _physical_address, _dhcp_lease.ipv4address), Logger.Severity.Debug);
                using (DHCPLeaseSet _dhcp_db = new DHCPLeaseSet())
                {
                    _dhcp_lease.issued = true;
                    _dhcp_lease.issued_at = DateTime.UtcNow;
                    _dhcp_db.Save();
                }
                this.SendReply(response);
            }
            else
            {
                this.SendNak(message);
            }
        }

        private void SendAck(DhcpMessage message, AddressLease lease)
        {
            DhcpMessage response = new DhcpMessage();
            response.Operation = DhcpOperation.BootReply;
            response.Hardware = HardwareType.Ethernet;
            response.HardwareAddressLength = 6;
            response.SecondsElapsed = message.SecondsElapsed;
            response.SessionId = message.SessionId;

            response.AssignedAddress = lease.Address.ToArray();
            response.ClientHardwareAddress = message.ClientHardwareAddress;

            response.AddOption(DhcpOption.DhcpMessageType, (Byte)DhcpMessageType.Ack);
            response.AddOption(DhcpOption.AddressRequest, lease.Address.ToArray());
            AddDhcpOptions(response);

            this.SendReply(response);
            Trace.TraceInformation("{0} Dhcp Acknowledge Sent.", Thread.CurrentThread.ManagedThreadId);
        }

        private void AddDhcpOptions(DhcpMessage response)
        {
            response.AddOption(DhcpOption.AddressTime, DhcpMessage.ReverseByteOrder(BitConverter.GetBytes((Int32)this.m_LeaseDuration.TotalSeconds)));
            response.AddOption(DhcpOption.Router, this.m_Gateway.ToArray());
            response.AddOption(DhcpOption.SubnetMask, this.m_Subnet.ToArray());

            if (!String.IsNullOrEmpty(this.m_DnsSuffix))
            {
                response.AddOption(DhcpOption.DomainNameSuffix, Encoding.ASCII.GetBytes(this.m_DnsSuffix));
            }

            if (this.m_DnsServers.Count > 0)
            {
                Byte[] dnsServerAddresses = new Byte[this.m_DnsServers.Count * 4];
                for (Int32 i = 0; i < this.m_DnsServers.Count; i++)
                {
                    this.m_DnsServers[i].ToArray().CopyTo(dnsServerAddresses, i * 4);
                }

                response.AddOption(DhcpOption.DomainNameServer, dnsServerAddresses);
            }
        }

        private void SendNak(DhcpMessage message)
        {
            Trace.TraceInformation("{0} Sending Dhcp Negative Acknowledge.", Thread.CurrentThread.ManagedThreadId);

            DhcpMessage response = new DhcpMessage();
            response.Operation = DhcpOperation.BootReply;
            response.Hardware = HardwareType.Ethernet;
            response.HardwareAddressLength = 6;
            response.SecondsElapsed = message.SecondsElapsed;
            response.SessionId = message.SessionId;

            response.ClientHardwareAddress = message.ClientHardwareAddress;

            response.AddOption(DhcpOption.DhcpMessageType, (Byte)DhcpMessageType.Nak);

            this.SendReply(response);
            Trace.TraceInformation("{0} Dhcp Negative Acknowledge Sent.", Thread.CurrentThread.ManagedThreadId);
        }

        private void SendReply(DhcpMessage response)
        {
            response.AddOption(DhcpOption.DhcpAddress, this.m_DhcpInterfaceAddress.GetAddressBytes());

            Byte[] sessionId = BitConverter.GetBytes(response.SessionId);

            try
            {
                this.m_DhcpSocket.SendTo(response.ToArray(), new IPEndPoint(IPAddress.Broadcast, DhcpClientPort));
            }
            catch (Exception ex)
            {
                TraceException("Error Sending Dhcp Reply", ex);
                throw;
            }
        }
    }
}
