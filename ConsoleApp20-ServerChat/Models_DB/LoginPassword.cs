using ConsoleApp20_ServerChat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp20_ServerChat.Models_DB;

public class LoginPassword
{
    public int Id { get; set; }
    public User User { get; set; }
    public string HashPassword { get; set; }
}
