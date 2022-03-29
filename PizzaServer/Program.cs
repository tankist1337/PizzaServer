using PizzaServer.data;
using PizzaServer.server;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace PizzaServer
{

    class Program
    {
        const int port = 27016;
        static TcpListener listener;
        static void Main(string[] args)
        {

            try
            {
                listener = new TcpListener(IPAddress.Any, port);
                listener.Start();

                Console.WriteLine($"{Dns.GetHostName()}");
                Console.WriteLine($"{System.Net.Dns.GetHostByName(Dns.GetHostName()).AddressList[1]}:{port}");
                string pubIp = new System.Net.WebClient().DownloadString("https://api.ipify.org");
                Console.WriteLine($"{pubIp}:{port}");
                Console.WriteLine("_______________________");

                Console.WriteLine("Ожидание подключение...");

                while (true)
                {
                    TcpClient client = listener.AcceptTcpClient();
                    ClientObject clientObject = new ClientObject(client);

                    Thread clientThread = new Thread(new ThreadStart(clientObject.Process));
                    clientThread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error: {ex.Message}") ;
            }
            finally
            {
                if (listener != null)
                    listener.Stop();
            }
        }

    }
}
