using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ConsoleApp20_ServerChat.Models_Server_Client;

public class GetMessagesRequest : IDataMessage
{
    public ChatUser Questioner { get; set; } // who sends requests
    public int IdAfter { get; set; }

    public DataMessage ToMessage()
    {
        return new DataMessage()
        {
            Type = DataType.GETMESSAGES_REQUEST,
            Data = JsonSerializer.Serialize(this)
        };
    }
}
