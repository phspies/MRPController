using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetFlowv9;
using System.Net.Sockets;
using System.Net;
using CloudMoveyWorkerService.NetFlow.v5;
using CloudMoveyWorkerService.NetFlow;
using CloudMoveyWorkerService.Database;

namespace CloudMoveyWorkerService.CloudMoveyWorkerService.Classes.Background_Classes
{
    class NetflowWorker
    {
        public void Start()
        {
            LocalDB db = new LocalDB();

            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint iep = new IPEndPoint(IPAddress.Any, 9991);
            sock.Bind(iep);
            EndPoint ep = (EndPoint)iep;

            byte[] data = new byte[2048];

            while (true)
            {
                int recv = sock.ReceiveFrom(data, ref ep);
                //Console.ReadKey();
                Console.Clear();
                byte[] _bytes = new byte[recv];

                for (int i = 0; i < recv; i++)
                    _bytes[i] = data[i];

                NetflowCommon common = new NetflowCommon(_bytes);
                if (common._version == 5)
                {
                    PacketV5 packet = new PacketV5(_bytes);
                    NetworkFlow _netflow = new NetworkFlow();
                    _netflow.source_address = Int32ToIp(packet.SrcAddr);
                    _netflow.target_address = Int32ToIp(packet.DstAddr);
                    _netflow.source_port = packet.SrcPort;
                    _netflow.target_port = packet.DstPort;
                    _netflow.protocol = packet.Prot;
                    _netflow.timestamp = packet.Header.Secs;
                    _netflow.start_timestamp = packet.UptimeFirst;
                    _netflow.stop_timestamp = packet.UptimeLast;
                    _netflow.packets = packet.Packets;
                    _netflow.kbyte = Convert.ToUInt32(Math.Round((double)((packet.Octets * 8) / 1024)));
                    _netflow.id = Guid.NewGuid().ToString().Replace("-", "").GetHashString();
                    db.NetworkFlows.Add(_netflow);
                    db.SaveChanges();
                }
            }
            sock.Close();
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
