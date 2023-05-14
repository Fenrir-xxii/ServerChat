// See https://aka.ms/new-console-template for more information
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using ClientTest;
using ConsoleApp20_ServerChat.Models_Server_Client;

var client = new ChatClient("127.0.0.1", 5555);

var imageClient = new ImageClient("127.0.0.1", 4444);
//var filename = imageClient.UploadImage("C:\\Users\\user\\source\\repos\\ConsoleApp20-ServerChat\\ConsoleApp20-ServerChat\\mem.jpg");

//Console.WriteLine(filename);
var filename = "filename.jpg";
//imageClient.DownloadImage(filename);



//var request = new LoginRequest
//{
//    Login = "userlog",
//    Password = "userlog",
//};
//var response = client.Login(request);
//Console.WriteLine($"Success: {response.Success}");
//Console.WriteLine($"Error: {response.Error}");
//Console.WriteLine($"User id: {response.User?.Id}");
//Console.WriteLine($"User name: {response.User?.Name}");
//Console.WriteLine($"User login: {response.User?.Login}");


/*
 // REGISTER
var request = new RegisterRequest
{
    Name = "Test",
    Login = "user1",
    Password = "user1",
};
var response = client.Register(request);
Console.WriteLine($"Success: {response.Success}");
Console.WriteLine($"Error: {response.Error}");
Console.WriteLine($"User id: {response.User?.Id}");
Console.WriteLine($"User name: {response.User?.Name}");
Console.WriteLine($"User login: {response.User?.Login}");


*/