﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp20_ServerChat.Models_Server_Client;

public enum DataType
{
    REGISTER_REQUEST = 0, REGISTER_RESPONSE, LOGIN_REQUEST, LOGIN_RESPONSE, ALLUSERS_REQUEST, ALLUSERS_RESPONSE, 
    SENDMESSAGE_REQUEST, SENDMESSAGE_RESPONSE, GETMESSAGES_REQUEST, GETMESSAGES_RESPONSE, LOGOUT_REQUEST, LOGOUT_RESPONSE
}
public class DataMessage
{
    public DataType Type { get; set; }    
    public string Data { get; set; }
}
