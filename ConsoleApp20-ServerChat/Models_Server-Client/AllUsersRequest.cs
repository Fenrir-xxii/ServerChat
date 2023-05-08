using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ConsoleApp20_ServerChat.Models_Server_Client;

public class AllUsersRequest : IDataMessage
{
    public string Login { get; set; }
    public DataMessage ToMessage()
    {
        return new DataMessage()
        {
            Type = DataType.ALLUSERS_REQUEST,
            Data = JsonSerializer.Serialize(this)
        };
    }
}
