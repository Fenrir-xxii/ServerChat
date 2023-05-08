﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ConsoleApp20_ServerChat.Models_Server_Client;

namespace ClientWPF;

public class ChatClientWPF
{
    public ChatClientWPF(string ipAddress, int port)
    {
        IPAddress ip = IPAddress.Parse(ipAddress);
        _endPoint = new IPEndPoint(ip, port);
    }
    private Socket _socket;
    private IPEndPoint _endPoint;

    private Object? Send(IDataMessage message)
    {
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
        _socket.Connect(_endPoint);
        var m = message.ToMessage();
        _socket.Send(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message.ToMessage())));
        var buffer = new byte[1024];
        var read = _socket.Receive(buffer);
        _socket.Close();

        var incoming = Encoding.UTF8.GetString(buffer, 0, read);
        var response = JsonSerializer.Deserialize<DataMessage>(incoming);

        switch (response.Type)
        {
            case DataType.REGISTER_RESPONSE:
                return JsonSerializer.Deserialize<RegisterResponse>(response.Data);
            case DataType.LOGIN_RESPONSE:
                return JsonSerializer.Deserialize<LoginResponse>(response.Data);
            case DataType.ALLUSERS_RESPONSE:
                return JsonSerializer.Deserialize<AllUsersResponse>(response.Data);
            default:
                return null;
        }

    }
    public RegisterResponse Register(RegisterRequest request)
    {
        return Send(request) as RegisterResponse;
    }
    public LoginResponse Login(LoginRequest request)
    {
        return Send(request) as LoginResponse;
    }
    public AllUsersResponse AllUsers(AllUsersRequest request)
    {
        return Send(request) as AllUsersResponse;
    }
}
