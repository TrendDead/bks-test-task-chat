using System;

namespace Chat
{
    class Program
    {
        private const int PORT = 90;

        static void Main(string[] args)
        {
            string host = System.Net.Dns.GetHostName();
            System.Net.IPAddress ip = System.Net.Dns.GetHostAddresses(host)[0];

            Console.WriteLine("IP adress " + ip.ToString());

            Server server = new Server(PORT, Console.Out);
            server.Work();
        }
    }
}
