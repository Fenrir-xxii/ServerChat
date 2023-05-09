using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ConsoleApp20_ServerChat.Models_Server_Client;

public class GetMessagesResponse : IDataMessage
{
    public List<ChatMessage> Messages { get; set; }
    public DataMessage ToMessage()
    {
        return new DataMessage()
        {
            Type = DataType.GETMESSAGES_RESPONSE,
            Data = JsonSerializer.Serialize(this)
        };
    }
}
