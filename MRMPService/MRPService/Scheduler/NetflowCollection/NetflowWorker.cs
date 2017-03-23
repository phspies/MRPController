﻿using System;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using MRMPService.LocalDatabase;
using MRMPService.Utilities;
using MRMPService.Modules.Netflow.v9;
using MRMPService.Modules.Netflow.v10;
using MRMPService.Modules.Netflow;
using MRMPService.Modules.Netflow.V5;
using System.Threading;
using System.IO;
using Newtonsoft.Json;

namespace MRMPService.Scheduler.NetflowCollection
{
    class NetflowWorker
    {
        public void Start()
        {
            TemplatesV10 _templates_v10 = new TemplatesV10();
            TemplatesV9 _templates_v9 = new TemplatesV9();

            try
            {
                Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                IPEndPoint iep = new IPEndPoint(IPAddress.Any, 9996);
                sock.Bind(iep);
                EndPoint ep = (EndPoint)iep;
                byte[] data = new byte[2048];

                while (true)
                {
                    int recv = sock.ReceiveFrom(data, ref ep);
                    byte[] _bytes = new byte[recv];

                    for (int i = 0; i < recv; i++)
                    {
                        _bytes[i] = data[i];
                    }
                    NetflowCommon common = new NetflowCommon(_bytes);
                    if (common._version == 5)
                    {
                        PacketV5 packet = new PacketV5(_bytes);
                        if (MRMPServiceBase.debug_netflow)
                        {
                            MRMPService.Log.Logger.log(String.Format("Received {0}v netflow packet: {1}", common._version, JsonConvert.SerializeObject(packet)), MRMPService.Log.Logger.Severity.Debug, "netflow");
                        }
                        NetworkFlow _netflow = new NetworkFlow();
                        _netflow.source_address = Int32ToIp(packet.SrcAddr);
                        _netflow.target_address = Int32ToIp(packet.DstAddr);
                        _netflow.source_port = (int)packet.SrcPort;
                        _netflow.target_port = (int)packet.DstPort;
                        _netflow.protocol = (int)packet.Prot;
                        _netflow.timestamp = packet.Header.Secs;
                        _netflow.start_timestamp = packet.UptimeFirst;
                        _netflow.stop_timestamp = packet.UptimeLast;
                        _netflow.packets = (int)packet.Packets;
                        _netflow.kbyte = Convert.ToInt32(Math.Round((double)((packet.Octets * 8) / 1024)));
                        _netflow.id = Guid.NewGuid().ToString().Replace("-", "").GetHashString();
                        using (NetworkFlowSet _ctx = new NetworkFlowSet())
                        {
                            _ctx.ModelRepository.Insert(_netflow);
                        }
                    }
                    else if ((common._version == 9))
                    {
                        if (_bytes.Count() > 16)
                        {
                            V9Packet packet = new V9Packet(_bytes, _templates_v9);
                            if (MRMPServiceBase.debug_netflow)
                            {
                                MRMPService.Log.Logger.log(String.Format("Received {0}v netflow packet: {1}", common._version, JsonConvert.SerializeObject(packet)), MRMPService.Log.Logger.Severity.Debug, "netflow");
                            }
                            Modules.Netflow.v9.FlowSet _flowset = packet.FlowSet.FirstOrDefault(x => x.Template.Count() != 0);
                            if (_flowset != null)
                            {
                                foreach (Modules.Netflow.v9.Template _template in _flowset.Template.Where(x => x.Field.Any(y => y.Value.Count != 0)))
                                {

                                    NetworkFlow _netflow = new NetworkFlow();

                                    if (_template.Field.Exists(x => x.GetTypes() == (UInt16)Modules.Netflow.v9.FieldType.IPV4_SRC_ADDR))
                                    {
                                        _netflow.source_address = new IPAddress(_template.Field.FirstOrDefault(x => x.GetTypes() == (UInt16)Modules.Netflow.v9.FieldType.IPV4_SRC_ADDR).Value.ToArray()).ToString();
                                        _netflow.target_address = new IPAddress(_template.Field.FirstOrDefault(x => x.GetTypes() == (UInt16)Modules.Netflow.v9.FieldType.IPV4_DST_ADDR).Value.ToArray()).ToString();
                                    }
                                    else if (_template.Field.Exists(x => x.GetTypes() == (UInt16)Modules.Netflow.v9.FieldType.IPV6_SRC_ADDR))
                                    {
                                        _netflow.source_address = new IPAddress(_template.Field.FirstOrDefault(x => x.GetTypes() == (UInt16)Modules.Netflow.v9.FieldType.IPV6_SRC_ADDR).Value.ToArray()).ToString();
                                        _netflow.target_address = new IPAddress(_template.Field.FirstOrDefault(x => x.GetTypes() == (UInt16)Modules.Netflow.v9.FieldType.IPV6_DST_ADDR).Value.ToArray()).ToString();
                                    }
                                    if (_template.Field.Exists(x => x.GetTypes() == (UInt16)Modules.Netflow.v9.FieldType.L4_SRC_PORT))
                                    {
                                        _netflow.source_port = BitConverter.ToUInt16(_template.Field.FirstOrDefault(x => x.GetTypes() == (UInt16)Modules.Netflow.v9.FieldType.L4_SRC_PORT).Value.ToArray().Reverse().ToArray(), 0);
                                    }
                                    if (_template.Field.Exists(x => x.GetTypes() == (UInt16)Modules.Netflow.v9.FieldType.L4_DST_PORT))
                                    {
                                        _netflow.target_port = BitConverter.ToUInt16(_template.Field.FirstOrDefault(x => x.GetTypes() == (UInt16)Modules.Netflow.v9.FieldType.L4_DST_PORT).Value.ToArray().Reverse().ToArray(), 0);
                                    }
                                    if (_template.Field.Exists(x => x.GetTypes() == (UInt16)Modules.Netflow.v9.FieldType.PROTOCOL))
                                    {
                                        _netflow.protocol = _template.Field.FirstOrDefault(x => x.GetTypes() == (UInt16)Modules.Netflow.v9.FieldType.PROTOCOL).Value[0];
                                    }
                                    if (_template.Field.Exists(x => x.GetTypes() == (UInt16)Modules.Netflow.v9.FieldType.FIRST_SWITCHED))
                                    {
                                        _netflow.start_timestamp = BitConverter.ToUInt16(_template.Field.FirstOrDefault(x => x.GetTypes() == (UInt16)Modules.Netflow.v9.FieldType.FIRST_SWITCHED).Value.ToArray().Reverse().ToArray(), 0);
                                    }
                                    if (_template.Field.Exists(x => x.GetTypes() == (UInt16)Modules.Netflow.v9.FieldType.LAST_SWITCHED))
                                    {
                                        _netflow.stop_timestamp = BitConverter.ToUInt16(_template.Field.FirstOrDefault(x => x.GetTypes() == (UInt16)Modules.Netflow.v9.FieldType.LAST_SWITCHED).Value.ToArray().Reverse().ToArray(), 0);
                                    }
                                    _netflow.timestamp = DateTime.UtcNow;
                                    if (_template.Field.Exists(x => x.GetTypes() == (UInt16)Modules.Netflow.v9.FieldType.IN_PKTS))
                                    {
                                        _netflow.packets = BitConverter.ToUInt16(_template.Field.FirstOrDefault(x => x.GetTypes() == (UInt16)Modules.Netflow.v9.FieldType.IN_PKTS).Value.ToArray().Reverse().ToArray(), 0);
                                    }
                                    if (_template.Field.Exists(x => x.GetTypes() == (UInt16)Modules.Netflow.v9.FieldType.IN_BYTES))
                                    {
                                        _netflow.kbyte = BitConverter.ToUInt16(_template.Field.FirstOrDefault(x => x.GetTypes() == (UInt16)Modules.Netflow.v9.FieldType.IN_BYTES).Value.ToArray().Reverse().ToArray(), 0);
                                    }

                                    _netflow.id = Guid.NewGuid().ToString().Replace("-", "").GetHashString();
                                    using (NetworkFlowSet _ctx = new NetworkFlowSet())
                                    {
                                        _ctx.ModelRepository.Insert(_netflow);
                                    }
                                }
                            }
                        }
                    }
                    else if ((common._version == 10))
                    {
                        if (_bytes.Count() > 16)
                        {
                            V10Packet packet = new V10Packet(_bytes, _templates_v10);
                            if (MRMPServiceBase.debug_netflow)
                            {
                                MRMPService.Log.Logger.log(String.Format("Received {0}v netflow packet: {1}", common._version, JsonConvert.SerializeObject(packet)), MRMPService.Log.Logger.Severity.Debug, "netflow");
                            }
                            Modules.Netflow.v10.FlowSet _flowset = packet.FlowSet.FirstOrDefault(x => x.Template.Count() != 0);
                            if (_flowset != null)
                            {
                                foreach (Modules.Netflow.v10.Template _template in _flowset.Template.Where(x => x.Field.Any(y => y.Type == "sourceTransportPort" && y.Value.Count > 0)))
                                {
                                    NetworkFlow _netflow = new NetworkFlow();
                                    _netflow.id = Guid.NewGuid().ToString().Replace("-", "").GetHashString();
                                    _netflow.source_port = BitConverter.ToUInt16(_template.Field.FirstOrDefault(x => x.GetTypes() == (UInt16)Modules.Netflow.v10.FieldType.sourceTransportPort).Value.ToArray().Reverse().ToArray(), 0);
                                    _netflow.target_port = BitConverter.ToUInt16(_template.Field.FirstOrDefault(x => x.GetTypes() == (UInt16)Modules.Netflow.v10.FieldType.destinationTransportPort).Value.ToArray().Reverse().ToArray(), 0);
                                    _netflow.protocol = _template.Field.FirstOrDefault(x => x.GetTypes() == (UInt16)Modules.Netflow.v10.FieldType.protocolIdentifier).Value[0];
                                    _netflow.start_timestamp = (long)BitConverter.ToUInt64(_template.Field.FirstOrDefault(x => x.GetTypes() == (UInt16)Modules.Netflow.v10.FieldType.flowStartMilliseconds).Value.ToArray().Reverse().ToArray(), 0);
                                    _netflow.stop_timestamp = (long)BitConverter.ToUInt64(_template.Field.FirstOrDefault(x => x.GetTypes() == (UInt16)Modules.Netflow.v10.FieldType.flowEndMilliseconds).Value.ToArray().Reverse().ToArray(), 0);
                                    _netflow.timestamp = DateTime.UtcNow;
                                    _netflow.packets = BitConverter.ToUInt16(_template.Field.FirstOrDefault(x => x.GetTypes() == (UInt16)Modules.Netflow.v10.FieldType.packetDeltaCount).Value.ToArray().Reverse().ToArray(), 0);
                                    _netflow.kbyte = BitConverter.ToUInt16(_template.Field.FirstOrDefault(x => x.GetTypes() == (UInt16)Modules.Netflow.v10.FieldType.octetDeltaCount).Value.ToArray().Reverse().ToArray(), 0);
                                    if (_template.Field.Exists(x => x.GetTypes() == (UInt16)Modules.Netflow.v10.FieldType.sourceIPv4Address))
                                    {
                                        _netflow.source_address = new IPAddress(_template.Field.FirstOrDefault(x => x.GetTypes() == (UInt16)Modules.Netflow.v10.FieldType.sourceIPv4Address).Value.ToArray()).ToString();
                                        _netflow.target_address = new IPAddress(_template.Field.FirstOrDefault(x => x.GetTypes() == (UInt16)Modules.Netflow.v10.FieldType.destinationIPv4Address).Value.ToArray()).ToString();
                                    }
                                    else if (_template.Field.Exists(x => x.GetTypes() == (UInt16)Modules.Netflow.v10.FieldType.sourceIPv6Address))
                                    {
                                        _netflow.source_address = new IPAddress(_template.Field.FirstOrDefault(x => x.GetTypes() == (UInt16)Modules.Netflow.v10.FieldType.sourceIPv6Address).Value.ToArray()).ToString();
                                        _netflow.target_address = new IPAddress(_template.Field.FirstOrDefault(x => x.GetTypes() == (UInt16)Modules.Netflow.v10.FieldType.destinationIPv6Address).Value.ToArray()).ToString();
                                    }
                                    using (NetworkFlowSet _ctx = new NetworkFlowSet())
                                    {
                                        _ctx.ModelRepository.Insert(_netflow);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MRMPService.Log.Logger.log(String.Format("Fatal error while processing netflow packets: {0}", ex.ToString()), MRMPService.Log.Logger.Severity.Fatal, "netflow");
            }
        }
        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
        static private int IpToInt32(string ipAddress)
        {

            return BitConverter.ToInt32(IPAddress.Parse(ipAddress).GetAddressBytes().Reverse().ToArray(), 0);
        }

        static private string Int32ToIp(uint ipAddress)
        {
            return new IPAddress(BitConverter.GetBytes(ipAddress).Reverse().ToArray()).ToString();
        }
    }
}
