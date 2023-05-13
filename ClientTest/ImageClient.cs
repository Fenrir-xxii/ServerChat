using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using BitmapImage = System.Drawing.Image;
using System.Drawing.Imaging;

namespace ClientTest;

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
        System.Drawing.Bitmap bitmap = new Bitmap(BitmapImage.FromFile(filename));

        using (MemoryStream ms = new MemoryStream())
        {
            bitmap.Save(ms, ImageFormat.Jpeg);
            socket.Send(BitConverter.GetBytes(ms.Length));
            socket.Send(ms.ToArray());
        }
    }
}
