using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ConsoleApp20_ServerChat.Models_Server_Client;

public class SendMessageRequest : IDataMessage
{
    public SendMessageRequest()
    {
        Images = new ();
    }
    public string TextMessage { get; set; }
    public ChatUser Sender { get; set; }
    public ChatUser Receiver { get; set; }
    public List<ChatMessageImage> Images { get; set; }

    public DataMessage ToMessage()
    {
        return new DataMessage()
        {
            Type = DataType.SENDMESSAGE_REQUEST,
            Data = JsonSerializer.Serialize(this)
        };
    }
}
