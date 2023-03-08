using System.Net;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Text;

public class Client
{
    private static TcpClient _client;

    public Client(int port, string connectIp)
    {
        _client = new TcpClient();
        _client.Connect(IPAddress.Parse(connectIp), port);
    }

    public void Work()
    {
        Thread clientListener = new Thread(Reader);
        clientListener.Start();
    }

    public void SendMessage(string message)
    {
        message.Trim();
        byte[] buffer = Encoding.ASCII.GetBytes((message).ToCharArray());
        _client.GetStream().Write(buffer, 0, buffer.Length);
        Chat.Message.Add(message);
    }

    private static void Reader()
    {
        while (true)
        {
            NetworkStream NS = _client.GetStream();
            List<byte> buffer = new List<byte>();
            while (NS.DataAvailable)
            {
                int readByte = NS.ReadByte();
                if(readByte > -1)
                {
                    buffer.Add((byte)readByte);
                }
            }
            if(buffer.Count > 0)
            {
                Chat.Message.Add(Encoding.ASCII.GetString(buffer.ToArray()));
            }
        }
    }

    ~Client()
    {
        if (_client != null)
        {
            _client.Close();
        }
    }
}
