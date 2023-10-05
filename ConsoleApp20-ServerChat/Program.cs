// See https://aka.ms/new-console-template for more information
using ConsoleApp20_ServerChat;
using ConsoleApp20_ServerChat.Models;
using ConsoleApp20_ServerChat.Models_DB;
using ConsoleApp20_ServerChat.Models_Server_Client;
using System.Text;
using System.Text.Json;

var imageServer = new ImageServer("127.0.0.1", 4444);
imageServer.Worker = (s) =>
{
    var core = new ImageServerCore();
    core.Handle(s);
    s.Shutdown(System.Net.Sockets.SocketShutdown.Both);
    s.Close();

};
Task.Run(() =>
{
    imageServer.Listen();
    while(true)
    {
        Task.Run(() => imageServer.Worker(imageServer._socket.Accept()));
        Thread.Sleep(1000);
    }
});

var server = new ChatServer("127.0.0.1", 5555);
var core = new ChatCore();
server.Worker = (s) =>
{
    core.Handle(s);
    s.Shutdown(System.Net.Sockets.SocketShutdown.Both);
    s.Close();
};

server.Listen();
