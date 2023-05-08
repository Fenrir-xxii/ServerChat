using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ConsoleApp20_ServerChat.Models_DB;
using ConsoleApp20_ServerChat.Models_Server_Client;
using ConsoleApp20_ServerChat.Models;

namespace ConsoleApp20_ServerChat;

public class ChatServer
{
    public ChatServer(string ipAddress, int port)
    {
        IPAddress ip = IPAddress.Parse(ipAddress);
        _endPoint = new IPEndPoint(ip, port);
        //_db = new ChatDbContext();
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
    //private ChatDbContext _db;
    //public bool IsLoginFree(string login)
    //{
    //    if (login == null)
    //        return false;
    //    var logins = _db.Users.ToList();
    //    foreach(var l in logins)
    //    {
    //        if(l.Login.Equals(login)) 
    //        {
    //            return false;
    //        }
    //    };
    //    return true;
    //}
    //public void RegisterUser(ChatUser user)
    //{
    //    if (user == null) 
    //        return;
    //    var usr = new User()
    //    {
    //        Name = user.Name,
    //        Login = user.Login,
    //        IsOnline = true
    //    };
    //    _db.Users.Add(usr);
    //    _db.SaveChanges();
    //}
    //public User? GetUserByLogin(string login)
    //{
    //    return _db.Users.FirstOrDefault(x => x.Login.Equals(login));
    //}
}
