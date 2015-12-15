using CloudMoveyWorkerService.NetFlow.v5;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetFlowv9;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Netflow5
{
    class Program
    {
        static void Main(string[] args)
        {
            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint iep = new IPEndPoint(IPAddress.Any, 9991);
            sock.Bind(iep);
            EndPoint ep = (EndPoint)iep;

            byte[] data = new byte[2048];

            while (true)
            {
                int recv = sock.ReceiveFrom(data, ref ep);
                //Console.ReadKey();
                byte[] _bytes = new byte[recv];

                for (int i = 0; i < recv; i++)
                    _bytes[i] = data[i];

                NetflowCommon common = new NetflowCommon(_bytes);
                if (common._version == 5)
                {
                    Console.WriteLine("Version 5 Netflow");
                    PacketV5 packet = new PacketV5(_bytes);
                    Console.WriteLine("Version: " + packet.Header._version);
                    Console.WriteLine("Source Addr: " + Int32ToIp(packet.SrcAddr));
                    Console.WriteLine("Destination Addr: " + Int32ToIp(packet.DstAddr));
                    Console.WriteLine("Source Port: " + packet.SrcPort);
                    Console.WriteLine("Destination Port: " + packet.DstPort);
                    Console.WriteLine("Next Hop Addr: " + Int32ToIp(packet.NextHop));
                    Console.WriteLine("Uptime First: " + UnixTimeStampToDateTime(packet.UptimeFirst));
                    Console.WriteLine("Uptime Last: " + UnixTimeStampToDateTime(packet.UptimeLast));

                    Console.WriteLine("Ingress Inf: " + packet.InInf);
                    Console.WriteLine("Egress Inf: " + packet.OutInf);
                    Console.WriteLine("Packets: " + packet.Packets);
                    Console.WriteLine("Octects: " + packet.Octets);
                    Console.WriteLine("------------------------------------------------");
                }
                else if (common._version == 9)
                {

                    List<FlowSet> _flowset = new List<FlowSet>();

                    Int32 length = _bytes.Length - 20;

                    Byte[] header = new Byte[20];
                    Byte[] flowset = new Byte[length];

                    Array.Copy(_bytes, 0, header, 0, 20);
                    Array.Copy(_bytes, 20, flowset, 0, length);

                    V9Header _header = new V9Header(header);
                    byte[] reverse = flowset.Reverse().ToArray();

                    int templengh = 0;
                    TemplatesV9 _templates = new TemplatesV9();


                    while ((templengh + 2) < flowset.Length)
                    {
                        UInt16 lengths = BitConverter.ToUInt16(reverse, flowset.Length - sizeof(Int16) - (templengh + 2));
                        Byte[] bflowsets = new Byte[lengths];
                        Array.Copy(flowset, templengh, bflowsets, 0, lengths);

                        FlowSet flowsets = new FlowSet(bflowsets, _templates);
                        _flowset.Add(flowsets);

                        templengh += lengths;
                    }

                    if (_flowset.Any(x => x.Template.Count > 0))
                    {
                        Console.WriteLine("Version 9 Netflow");
                        Console.WriteLine("Uptime: " + _header.UpTime);
                        Console.WriteLine("DateTime: " + _header.Secs);
                        Console.WriteLine("Flows: " + _flowset.SelectMany(x => x.Template).Count());
                    }

                    Packet packet = new Packet(_bytes, _templates);
                    String ret = null;


                    int a = 0;

                    foreach (FlowSet flows in _flowset)
                    {
                        a++;


                        int i = 1;
                        foreach (Template templ in flows.Template)
                        {
                            ret += "\t\t---------------------------------------------\r\n";
                            foreach (Field fields in templ.Field)
                            {
                                if (fields.Value.Count != 0)
                                {
                                    switch (fields.Value.Count)
                                    {
                                        case 2:
                                            ret += "\t\t" + fields.Type + ":" + BitConverter.ToUInt16(fields.Value.ToArray().Reverse().ToArray(), 0) + " \n\r";
                                            break;
                                        case 4:
                                            ret += "\t\t" + fields.Type + ":" + BitConverter.ToUInt32(fields.Value.ToArray().Reverse().ToArray(), 0) + " \n\r";
                                            break;
                                        case 8:
                                            ret += "\t\t" + fields.Type + ":" + BitConverter.ToUInt64(fields.Value.ToArray().Reverse().ToArray(), 0) + " \n\r";
                                            break;
                                    
                                    }
                                }

                                //if ((fields.GetTypes() == (UInt16)FieldTypev9.IPV4_DST_ADDR) ||
                                //    (fields.GetTypes() == (UInt16)FieldTypev9.IPV4_SRC_ADDR) ||
                                //    (fields.GetTypes() == (UInt16)FieldTypev9.IPV4_NEXT_HOP) ||
                                //    (fields.GetTypes() == (UInt16)FieldTypev9.IPV6_DST_ADDR) ||
                                //    (fields.GetTypes() == (UInt16)FieldTypev9.IPV6_SRC_ADDR) ||
                                //    (fields.GetTypes() == (UInt16)FieldTypev9.IPV6_NEXT_HOP))
                                //{
                                //if (fields.Value.Count != 0) ret += new IPAddress(fields.Value.ToArray()).ToString();
                                //}
                                //else if ((fields.GetTypes() == (UInt16)FieldTypev9.L4_DST_PORT) || (fields.GetTypes() == (UInt16)FieldTypev9.L4_SRC_PORT))
                                //{

                                //if (fields.Value.Count != 0) ret += BitConverter.ToUInt16(fields.Value.ToArray().Reverse().ToArray(), 0);
                                //}
                                //else
                                //{

                                //    foreach (Byte bt in fields.Value)
                                //    {
                                //        ret += "0x" + bt.ToString("X") + " ";
                                //    }
                                //}

                                //if (fields.Value.Count != 0) ret += "\r\n";
                            }

                            i++;
                        }
                    }
                    if (ret != null) { Console.WriteLine(ret);  }
                }
            }
            sock.Close();

            Console.ReadKey();
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
