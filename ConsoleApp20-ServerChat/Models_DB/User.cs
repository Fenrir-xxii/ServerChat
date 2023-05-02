using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp20_ServerChat.Models;

public class User
{
    public int Id { get; set; } 
    public string Name { get; set; }
    public string Login { get; set; }
    public bool IsOnline { get; set; }
}
