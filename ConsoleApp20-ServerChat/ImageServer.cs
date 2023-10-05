using Azure;
using Azure.Core;
using ConsoleApp20_ServerChat.Models;
using ConsoleApp20_ServerChat.Models_DB;
using ConsoleApp20_ServerChat.Models_Server_Client;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ConsoleApp20_ServerChat;

public class ImageServer
{
    public Socket _socket;
    private IPEndPoint _endPoint;
    public ImageServer(string ipAddress, int port)
    {
        IPAddress ip = IPAddress.Parse(ipAddress);
        _endPoint = new IPEndPoint(ip, port);
    }
    public Action<Socket> Worker;
	public void Listen()
	{
		_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
		_socket.Bind(_endPoint);
		_socket.Listen(100);
		while (true)
		{
			Task.Run(() => Worker(_socket.Accept()));
			Thread.Sleep(500);
		}

	}
}
