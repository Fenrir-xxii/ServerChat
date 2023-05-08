using ConsoleApp20_ServerChat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp20_ServerChat.Models_DB;

public class Message
{
    public int Id { get; set; }
    public virtual User Sender { get; set; }
    public virtual User Receiver { get; set; }
    public string TextContent { get; set; }
    public DateTime CreatedAt { get; set; }
}
