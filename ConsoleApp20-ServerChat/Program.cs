// See https://aka.ms/new-console-template for more information
using ConsoleApp20_ServerChat;
using ConsoleApp20_ServerChat.Models;
using ConsoleApp20_ServerChat.Models_DB;
using ConsoleApp20_ServerChat.Models_Server_Client;
using System.Text;
using System.Text.Json;

//using (var context = new ChatDbContext())
//{
//    // Creates the database if not exists
//    //context.Database.EnsureCreated();
//    context.Add(new User { Name ="userTest", Login="user1", IsOnline=true });
//    // Saves changes
//    context.SaveChanges();
//}

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



//var server = new ChatServer("127.0.0.1", 5555);
//var core = new ChatCore();
//server.Worker = (s) =>
//{
//    //var buffer = new byte[1024];
//    //var read = s.Receive(buffer);
//    //var incoming = Encoding.UTF8.GetString(buffer, 0, read);

//    //var request = JsonSerializer.Deserialize<DataMessage>(incoming);

//    //switch (request.Type)
//    //{
//    //    case DataType.REGISTER_REQUEST:
//    //        //var r =  JsonSerializer.Deserialize<RegisterRequest>(request.Data);
//    //        //var response = new RegisterResponse()
//    //        //{
//    //        //    Success = true,
//    //        //    User = new ChatUser()
//    //        //    {
//    //        //        Id = 1,
//    //        //        Login = r.Login,
//    //        //        Name = r.Name
//    //        //    },
//    //        //};
//    //        //s.Send(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(response.ToMessage())));

//    //        var r = JsonSerializer.Deserialize<RegisterRequest>(request.Data);

//    //        if(server.IsLoginFree(r.Login))
//    //        {
//    //            var user = new ChatUser()
//    //            {
//    //                Login = r.Login,
//    //                Name = r.Name
//    //            };
//    //            try
//    //            {
//    //                server.RegisterUser(user);
//    //                var userFromDb = server.GetUserByLogin(r.Login);
//    //                if(userFromDb != null)
//    //                {
//    //                    var response = new RegisterResponse()
//    //                    {
//    //                        Success = true,
//    //                        User = new ChatUser()
//    //                        {
//    //                            Id = userFromDb.Id,
//    //                            Login = userFromDb.Login,
//    //                            Name = userFromDb.Name
//    //                        },
//    //                    };
//    //                    s.Send(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(response.ToMessage())));
//    //                }
//    //                else
//    //                {
//    //                    var response = new RegisterResponse()
//    //                    {
//    //                        Success = false,
//    //                        User = new ChatUser()
//    //                        {
//    //                            Id = -1,
//    //                            Login = "",
//    //                            Name = ""
//    //                        },
//    //                    };
//    //                    s.Send(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(response.ToMessage())));
//    //                }
//    //            }catch(Exception ex)
//    //            {
//    //                Console.WriteLine(ex.Message);
//    //            }
//    //        }
//    //        else
//    //        {
//    //            var response = new RegisterResponse()
//    //            {
//    //                Success = false,
//    //                User = new ChatUser()
//    //                {
//    //                    Id = -1,
//    //                    Login = "",
//    //                    Name = ""
//    //                },
//    //            };
//    //            s.Send(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(response.ToMessage())));
//    //        }
//    //        break;
//    //    default:
//    //        break;
//    //}
//    core.Handle(s);
//    s.Shutdown(System.Net.Sockets.SocketShutdown.Both);
//    s.Close();

//};

//server.Listen();
