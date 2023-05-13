using ConsoleApp20_ServerChat.Models;
using ConsoleApp20_ServerChat.Models_DB;
using ConsoleApp20_ServerChat.Models_Server_Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Drawing;
using BitmapImage = System.Drawing.Image;

//using System.Windows.Media.Imaging;
using static System.Net.Mime.MediaTypeNames;
using System.Drawing.Imaging;

namespace ConsoleApp20_ServerChat;

public class ImageServerCore
{
    private static Random random = new Random();
    protected string GenerateRandomName(int size)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        return new string(Enumerable.Repeat(chars, size)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
    protected string GetFullFileName(string filename)
    {
        return "C:\\Users\\user\\source\\repos\\ConsoleApp20-ServerChat\\ConsoleApp20-ServerChat\\ServerImages\\" + filename;
    }
    protected void ReceiveFile(Socket socket, string filename)
    {
        byte[] fileSizeBytes = new byte[sizeof(ulong)];
        socket.Receive(fileSizeBytes);
        long fileSize = BitConverter.ToInt64(fileSizeBytes, 0);

        byte[] imageData = new byte[fileSize];  //error  Array dimensions exceeded supported range
        socket.Receive(imageData);
        
        Bitmap bitmap;

        using (MemoryStream stream = new MemoryStream(imageData))
        {
            //stream.Seek(0, SeekOrigin.Begin);
            bitmap = new Bitmap(stream);
        }
        var bitmapCopy = new Bitmap(bitmap);

        bitmapCopy.Save(filename, System.Drawing.Imaging.ImageFormat.Jpeg);

        //string outputFileName = "..."; 
        //using (MemoryStream memory = new MemoryStream()) 
        //{ 
        //    using (FileStream fs = new FileStream(outputFileName, FileMode.Create, FileAccess.ReadWrite))
        //    { 
        //        thumbBMP.Save(memory, ImageFormat.Jpeg); 
        //        byte[] bytes = memory.ToArray(); 
        //        fs.Write(bytes, 0, bytes.Length); 
        //    } 
        //}

    }
    //ns.Shutdown(SocketShutdown.Both);
    //ns.Close();

//Task.Run(async () =>
//            {
//                do
//                {
//                    Task t = new Task(() =>
//                    {
//                        worker(_socket.Accept());
//                    });
//t.Start();
//                } while (true) ;
//            });
//    }
    protected void SendFile(Socket socket, string filename) 
    {
        if (!File.Exists(filename))
        {
            // empty image file if file noty exists
            //TODO
            return;
        }

        Bitmap bitmap = new Bitmap(BitmapImage.FromFile(filename));

        using (MemoryStream ms = new MemoryStream())
        {
            bitmap.Save(ms, ImageFormat.Jpeg);
            socket.Send(BitConverter.GetBytes(ms.Length));
            socket.Send(ms.ToArray());
        }
    }
    //public void SaveFile(string fileName)
    //{

    //}
    public void Handle(Socket socket)
    {
        // if ask get
        //get file
        byte[] requestTypeBuffer = new byte[sizeof(int)];
        socket.Receive(requestTypeBuffer);

        var requestType = BitConverter.ToInt32(requestTypeBuffer, 0);
        Console.WriteLine(requestType);
        string filename;

        switch (requestType)
        {
            case 0: //GET
                var buffer = new byte[1024];
                int read = socket.Receive(buffer);
                filename = Encoding.UTF8.GetString(buffer, 0, read);
                SendFile(socket, GetFullFileName(filename));
                break ;
            case 1: //UPLOAD
                Console.WriteLine("upload");
                filename = GenerateRandomName(20) + ".jpeg";
                ReceiveFile(socket, GetFullFileName(filename));
                socket.Send(Encoding.UTF8.GetBytes(filename));
                break;
            default:
                break;
        }

        //if ask upload
        //upload file
    }

}
