using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp20_ServerChat;

public class ChatServer
{
    public ChatServer(string ipAddress, int port)
    {
        IPAddress ip = IPAddress.Parse(ipAddress);
        _endPoint = new IPEndPoint(ip, port);
    }
    private Socket _socket;
    private IPEndPoint _endPoint;

    public Action<Socket> Worker;
    public void Listen()
    {
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
        try
        {
            _socket.Bind(_endPoint);
            _socket.Listen(1000);
            while (true)
            {
                Task.Run(() => Worker(_socket.Accept()));
                Thread.Sleep(1000);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }

}
