using ConsoleApp20_ServerChat.Models_Server_Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientWPF;

public class ChatMessageModel
{
    public int Id { get; set; }
    public string TextMessage { get; set; }
    public ChatUser Sender { get; set; }
    public ChatUser Receiver { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool AmIReceiver { get; set; }
    public string InfoText { get; set; }    
}
