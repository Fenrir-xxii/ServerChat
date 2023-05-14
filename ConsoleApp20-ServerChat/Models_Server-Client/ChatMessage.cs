using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp20_ServerChat.Models_Server_Client;

public class ChatMessage
{
    public ChatMessage() 
    { 
        Images = new ();
    }
    public int Id { get; set; }
    public string TextMessage { get; set; }
    public ChatUser Sender { get; set; }
    public ChatUser Receiver { get; set; }
    public DateTime CreatedAt { get; set; }

    public List<ChatMessageImage> Images { get; set; }
}
