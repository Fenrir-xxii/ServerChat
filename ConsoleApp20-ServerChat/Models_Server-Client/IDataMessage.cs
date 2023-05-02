using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp20_ServerChat.Models_Server_Client
{
    public interface IDataMessage
    {
        public DataMessage ToMessage();
    }
}
