// See https://aka.ms/new-console-template for more information
using ConsoleApp20_ServerChat;
using ConsoleApp20_ServerChat.Models;
using ConsoleApp20_ServerChat.Models_Server_Client;
using System.Text;
using System.Text.Json;

var server = new ChatServer("127.0.0.1", 5555);

server.Worker = (s) =>
{
    var buffer = new byte[1024];
    var read = s.Receive(buffer);
    var incoming = Encoding.UTF8.GetString(buffer, 0, read);

    var request = JsonSerializer.Deserialize<DataMessage>(incoming);

    switch (request.Type)
    {
        case DataType.REGISTER_REQUEST:
            var r =  JsonSerializer.Deserialize<RegisterRequest>(request.Data);
            var response = new RegisterResponse()
            {
                Success = true,
                User = new ChatUser()
                {
                    Id = 1,
                    Login = r.Login,
                    Name = r.Name
                },
            };
            s.Send(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(response.ToMessage())));
            break;
        default:
            break;
    }
    s.Shutdown(System.Net.Sockets.SocketShutdown.Both);
    s.Close();

};

server.Listen();
