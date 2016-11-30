using System;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using MRMPService.NetFlow.v5;
using MRMPService.NetFlow;
using MRMPService.LocalDatabase;
using MRMPService.Utilities;
using System.Net.NetFlowv9;
using System.Net.NetFlowv10;

namespace MRMPService.NetflowCollection
{
    class NetflowWorker
    {
        public async void Start()
        {

            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint iep = new IPEndPoint(IPAddress.Any, 9996);
            sock.Bind(iep);
            EndPoint ep = (EndPoint)iep;

            byte[] data = new byte[2048];
            TemplatesV9 _templates_v9 = new TemplatesV9();
            TemplatesV10 _templates_v10 = new TemplatesV10();

            while (true)
            {
                int recv = sock.ReceiveFrom(data, ref ep);
                byte[] _bytes = new byte[recv];

                for (int i = 0; i < recv; i++)
                    _bytes[i] = data[i];

                await System.Threading.Tasks.Task.Run(() =>
                {
                    NetflowCommon common = new NetflowCommon(_bytes);
                    if (common._version == 5)
                    {
                        PacketV5 packet = new PacketV5(_bytes);
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
                        using (MRPDatabase db = new MRPDatabase())
                        {
                            db.NetworkFlows.Add(_netflow);
                            db.SaveChanges();
                        }
                    }
                    else if ((common._version == 9))
                    {
                        if (_bytes.Count() > 16)
                        {
                            V9Packet packet = new V9Packet(_bytes, _templates_v9);
                            System.Net.NetFlowv9.FlowSet _flowset = packet.FlowSet.FirstOrDefault(x => x.Template.Count() != 0);
                            if (_flowset != null)
                            {
                                foreach (System.Net.NetFlowv9.Template _template in _flowset.Template.Where(x => x.Field.Any(y => y.Value.Count != 0)))
                                {
                                    NetworkFlow _netflow = new NetworkFlow();

                                    if (_template.Field.Exists(x => x.GetTypes() == (UInt16)System.Net.NetFlowv9.FieldType.IPV4_SRC_ADDR))
                                    {
                                        _netflow.source_address = new IPAddress(_template.Field.FirstOrDefault(x => x.GetTypes() == (UInt16)System.Net.NetFlowv9.FieldType.IPV4_SRC_ADDR).Value.ToArray()).ToString();
                                        _netflow.target_address = new IPAddress(_template.Field.FirstOrDefault(x => x.GetTypes() == (UInt16)System.Net.NetFlowv9.FieldType.IPV4_DST_ADDR).Value.ToArray()).ToString();
                                    }
                                    else if (_template.Field.Exists(x => x.GetTypes() == (UInt16)System.Net.NetFlowv9.FieldType.IPV6_SRC_ADDR))
                                    {
                                        _netflow.source_address = new IPAddress(_template.Field.FirstOrDefault(x => x.GetTypes() == (UInt16)System.Net.NetFlowv9.FieldType.IPV6_SRC_ADDR).Value.ToArray()).ToString();
                                        _netflow.target_address = new IPAddress(_template.Field.FirstOrDefault(x => x.GetTypes() == (UInt16)System.Net.NetFlowv9.FieldType.IPV6_DST_ADDR).Value.ToArray()).ToString();
                                    }
                                    if (_template.Field.Exists(x => x.GetTypes() == (UInt16)System.Net.NetFlowv9.FieldType.L4_SRC_PORT))
                                    {
                                        _netflow.source_port = BitConverter.ToUInt16(_template.Field.FirstOrDefault(x => x.GetTypes() == (UInt16)System.Net.NetFlowv9.FieldType.L4_SRC_PORT).Value.ToArray().Reverse().ToArray(), 0);
                                    }
                                    if (_template.Field.Exists(x => x.GetTypes() == (UInt16)System.Net.NetFlowv9.FieldType.L4_DST_PORT))
                                    {
                                        _netflow.target_port = BitConverter.ToUInt16(_template.Field.FirstOrDefault(x => x.GetTypes() == (UInt16)System.Net.NetFlowv9.FieldType.L4_DST_PORT).Value.ToArray().Reverse().ToArray(), 0);
                                    }
                                    if (_template.Field.Exists(x => x.GetTypes() == (UInt16)System.Net.NetFlowv9.FieldType.PROTOCOL))
                                    {
                                        _netflow.protocol = _template.Field.FirstOrDefault(x => x.GetTypes() == (UInt16)System.Net.NetFlowv9.FieldType.PROTOCOL).Value[0];
                                    }
                                    if (_template.Field.Exists(x => x.GetTypes() == (UInt16)System.Net.NetFlowv9.FieldType.FIRST_SWITCHED))
                                    {
                                        _netflow.start_timestamp = BitConverter.ToUInt16(_template.Field.FirstOrDefault(x => x.GetTypes() == (UInt16)System.Net.NetFlowv9.FieldType.FIRST_SWITCHED).Value.ToArray().Reverse().ToArray(), 0);
                                    }
                                    if (_template.Field.Exists(x => x.GetTypes() == (UInt16)System.Net.NetFlowv9.FieldType.LAST_SWITCHED))
                                    {
                                        _netflow.stop_timestamp = BitConverter.ToUInt16(_template.Field.FirstOrDefault(x => x.GetTypes() == (UInt16)System.Net.NetFlowv9.FieldType.LAST_SWITCHED).Value.ToArray().Reverse().ToArray(), 0);
                                    }
                                    _netflow.timestamp = DateTime.UtcNow;
                                    if (_template.Field.Exists(x => x.GetTypes() == (UInt16)System.Net.NetFlowv9.FieldType.IN_PKTS))
                                    {
                                        _netflow.packets = BitConverter.ToUInt16(_template.Field.FirstOrDefault(x => x.GetTypes() == (UInt16)System.Net.NetFlowv9.FieldType.IN_PKTS).Value.ToArray().Reverse().ToArray(), 0);
                                    }
                                    if (_template.Field.Exists(x => x.GetTypes() == (UInt16)System.Net.NetFlowv9.FieldType.IN_BYTES))
                                    {
                                        _netflow.kbyte = BitConverter.ToUInt16(_template.Field.FirstOrDefault(x => x.GetTypes() == (UInt16)System.Net.NetFlowv9.FieldType.IN_BYTES).Value.ToArray().Reverse().ToArray(), 0);
                                    }

                                    _netflow.id = Guid.NewGuid().ToString().Replace("-", "").GetHashString();
                                    using (MRPDatabase db = new MRPDatabase())
                                    {
                                        db.NetworkFlows.Add(_netflow);
                                        db.SaveChanges();
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
                            System.Net.NetFlowv10.FlowSet _flowset = packet.FlowSet.FirstOrDefault(x => x.Template.Count() != 0);
                            if (_flowset != null)
                            {
                                foreach (System.Net.NetFlowv10.Template _template in _flowset.Template.Where(x => x.Field.Any(y => y.Value.Count != 0)))
                                {
                                    NetworkFlow _netflow = new NetworkFlow();

                                    if (_template.Field.Exists(x => x.GetTypes() == (UInt16)System.Net.NetFlowv10.FieldType.IPV4_SRC_ADDR))
                                    {
                                        _netflow.source_address = new IPAddress(_template.Field.FirstOrDefault(x => x.GetTypes() == (UInt16)System.Net.NetFlowv10.FieldType.IPV4_SRC_ADDR).Value.ToArray()).ToString();
                                        _netflow.target_address = new IPAddress(_template.Field.FirstOrDefault(x => x.GetTypes() == (UInt16)System.Net.NetFlowv10.FieldType.IPV4_DST_ADDR).Value.ToArray()).ToString();
                                    }
                                    else if (_template.Field.Exists(x => x.GetTypes() == (UInt16)System.Net.NetFlowv10.FieldType.IPV6_SRC_ADDR))
                                    {
                                        _netflow.source_address = new IPAddress(_template.Field.FirstOrDefault(x => x.GetTypes() == (UInt16)System.Net.NetFlowv10.FieldType.IPV6_SRC_ADDR).Value.ToArray()).ToString();
                                        _netflow.target_address = new IPAddress(_template.Field.FirstOrDefault(x => x.GetTypes() == (UInt16)System.Net.NetFlowv10.FieldType.IPV6_DST_ADDR).Value.ToArray()).ToString();
                                    }
                                    if (_template.Field.Exists(x => x.GetTypes() == (UInt16)System.Net.NetFlowv10.FieldType.L4_SRC_PORT))
                                    {
                                        _netflow.source_port = BitConverter.ToUInt16(_template.Field.FirstOrDefault(x => x.GetTypes() == (UInt16)System.Net.NetFlowv10.FieldType.L4_SRC_PORT).Value.ToArray().Reverse().ToArray(), 0);
                                    }
                                    if (_template.Field.Exists(x => x.GetTypes() == (UInt16)System.Net.NetFlowv10.FieldType.L4_DST_PORT))
                                    {
                                        _netflow.target_port = BitConverter.ToUInt16(_template.Field.FirstOrDefault(x => x.GetTypes() == (UInt16)System.Net.NetFlowv10.FieldType.L4_DST_PORT).Value.ToArray().Reverse().ToArray(), 0);
                                    }
                                    if (_template.Field.Exists(x => x.GetTypes() == (UInt16)System.Net.NetFlowv10.FieldType.PROTOCOL))
                                    {
                                        _netflow.protocol = _template.Field.FirstOrDefault(x => x.GetTypes() == (UInt16)System.Net.NetFlowv10.FieldType.PROTOCOL).Value[0];
                                    }
                                    if (_template.Field.Exists(x => x.GetTypes() == (UInt16)System.Net.NetFlowv10.FieldType.FIRST_SWITCHED))
                                    {
                                        _netflow.start_timestamp = BitConverter.ToUInt16(_template.Field.FirstOrDefault(x => x.GetTypes() == (UInt16)System.Net.NetFlowv10.FieldType.FIRST_SWITCHED).Value.ToArray().Reverse().ToArray(), 0);
                                    }
                                    if (_template.Field.Exists(x => x.GetTypes() == (UInt16)System.Net.NetFlowv10.FieldType.LAST_SWITCHED))
                                    {
                                        _netflow.stop_timestamp = BitConverter.ToUInt16(_template.Field.FirstOrDefault(x => x.GetTypes() == (UInt16)System.Net.NetFlowv10.FieldType.LAST_SWITCHED).Value.ToArray().Reverse().ToArray(), 0);
                                    }
                                    _netflow.timestamp = DateTime.UtcNow;
                                    if (_template.Field.Exists(x => x.GetTypes() == (UInt16)System.Net.NetFlowv10.FieldType.IN_PKTS))
                                    {
                                        _netflow.packets = BitConverter.ToUInt16(_template.Field.FirstOrDefault(x => x.GetTypes() == (UInt16)System.Net.NetFlowv10.FieldType.IN_PKTS).Value.ToArray().Reverse().ToArray(), 0);
                                    }
                                    if (_template.Field.Exists(x => x.GetTypes() == (UInt16)System.Net.NetFlowv10.FieldType.IN_BYTES))
                                    {
                                        _netflow.kbyte = BitConverter.ToUInt16(_template.Field.FirstOrDefault(x => x.GetTypes() == (UInt16)System.Net.NetFlowv10.FieldType.IN_BYTES).Value.ToArray().Reverse().ToArray(), 0);
                                    }

                                    _netflow.id = Guid.NewGuid().ToString().Replace("-", "").GetHashString();
                                    using (MRPDatabase db = new MRPDatabase())
                                    {
                                        db.NetworkFlows.Add(_netflow);
                                        db.SaveChanges();
                                    }
                                }
                            }
                        }
                    }
                });
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
