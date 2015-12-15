using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetFlowv9;
using System.Net.Sockets;
using System.Net;
using CloudMoveyWorkerService.NetFlow.v5;

namespace CloudMoveyWorkerService.CloudMoveyWorkerService.Classes.Background_Classes
{
    class NetflowWorkerv5
    {
        static public void Start()
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
                Console.Clear();
                byte[] bytes = new byte[recv];

                for (int i = 0; i < recv; i++)
                    bytes[i] = data[i];

                PacketV5 packet = new PacketV5(bytes);

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(packet.ToString());
            }
            sock.Close();

            Console.ReadKey();
        }
    }
}
