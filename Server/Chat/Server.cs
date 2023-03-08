using System.Net;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

namespace Chat
{
    public class Server
    {
        public static Server ActualServer;
        public static System.IO.TextWriter Out;

        private TcpListener _listener;
        private List<ClientInfo> _clients = new List<ClientInfo>();
        private List<ClientInfo> _newClients = new List<ClientInfo>();

        public Server(int port, System.IO.TextWriter _out)
        {
            Out = _out;
            //Server.ActualServer = this;
            ActualServer = this;

            _listener = new TcpListener(IPAddress.Any, port);
            _listener.Start();
        }

        public void Work()
        {
            Thread clientListener = new Thread(ListnerClients);
            clientListener.Start();

            while (true)
            {
                foreach (var client in _clients)
                {
                    if (client.IsConnected)
                    {
                        NetworkStream stream = client.TcpClient.GetStream();
                        while (stream.DataAvailable)
                        {
                            int readByte = stream.ReadByte();
                            if(readByte != -1)
                            {
                                client.Buffer.Add((byte)readByte);
                            }
                        }
                        if (client.Buffer.Count > 0)
                        {
                            Out.WriteLine("Resend");
                            foreach (var otherClient in _clients)
                            {
                                byte[] msg = client.Buffer.ToArray();
                                client.Buffer.Clear();
                                foreach (var newClient in _clients)
                                {
                                    if (newClient != client)
                                    {
                                        try
                                        {
                                            newClient.TcpClient.GetStream().Write(msg, 0, msg.Length);
                                        }
                                        catch
                                        {
                                            newClient.IsConnected = false;
                                            newClient.TcpClient.Close();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                _clients.RemoveAll(delegate (ClientInfo CI)
                {
                    if (!CI.IsConnected)
                    {
                        Server.Out.WriteLine("Клиент отключился");
                        return true;
                    }
                    return false;
                });

                if(_newClients.Count > 0)
                {
                    _clients.AddRange(_newClients);
                    _newClients.Clear();
                }
            }
        }

        private static void ListnerClients()
        {
            while (true)
            {
                ActualServer._newClients.Add(new ClientInfo(ActualServer._listener.AcceptTcpClient()));
            }
        }

        class ClientInfo
        {
            public TcpClient TcpClient;
            public List<byte> Buffer = new List<byte>();
            public bool IsConnected;

            public ClientInfo(TcpClient tcpClient)
            {
                TcpClient = tcpClient;
                IsConnected = true;
            }
        }
    }
}
