using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ConsoleApp20_ServerChat.Models_DB;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;
using BitmapImage = System.Drawing.Image;

namespace ClientWPF;

public class ImageClient
{
    public ImageClient(string ipAddress, int port)
    {
        IPAddress ip = IPAddress.Parse(ipAddress);
        _endPoint = new IPEndPoint(ip, port);
    }
    private Socket _socket;
    private IPEndPoint _endPoint;

    public string UploadImage(string filename)
    {
        var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
        socket.Connect(_endPoint);

        socket.Send(BitConverter.GetBytes(1));
        SendFile(socket, filename);

        var buffer = new byte[1024];
        int read = socket.Receive(buffer);
        filename = Encoding.UTF8.GetString(buffer, 0, read);
        socket.Close();
        return filename;
    }

    protected void SendFile(Socket socket, string filename)
    {
        Bitmap bitmap = new Bitmap(BitmapImage.FromFile(filename));

        using (MemoryStream ms = new MemoryStream())
        {
            bitmap.Save(ms, ImageFormat.Jpeg);
            socket.Send(BitConverter.GetBytes(ms.Length));
            socket.Send(ms.ToArray());
        }
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
    }
    public string GetFullFileName(string filename)
    {
        return "C:\\Users\\user\\source\\repos\\ConsoleApp20-ServerChat\\ConsoleApp20-ServerChat\\ClientImages\\" + filename;
    }
    public bool DownloadImage(string filename)
    {
        var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
        socket.Connect(_endPoint);

        socket.Send(BitConverter.GetBytes(0));
        socket.Send(Encoding.UTF8.GetBytes(filename));

        ReceiveFile(socket, GetFullFileName(filename));
        
        //SendFile(socket, filename);

        socket.Close();
        return true;
    }
}

