using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ConsoleApp20_ServerChat.Models_Server_Client;

public class LoginResponse : IDataMessage
{
    public bool Success { get; set; }
    public string? Error { get; set; }
    public ChatUser? User { get; set; }

    public DataMessage ToMessage()
    {
        return new DataMessage()
        {
            Type = DataType.LOGIN_RESPONSE,
            Data = JsonSerializer.Serialize(this)
        };
    }
}
