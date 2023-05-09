using Azure;
using Azure.Core;
using ConsoleApp20_ServerChat.Models;
using ConsoleApp20_ServerChat.Models_DB;
using ConsoleApp20_ServerChat.Models_Server_Client;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ConsoleApp20_ServerChat;

public class ChatCore
{
    public ChatCore()
    {
        _db = new ChatDbContext();
    }
    private void Send(Socket socket, IDataMessage message)
    {
        var response = JsonSerializer.Serialize(message.ToMessage());
        Console.WriteLine(response);
        socket.Send(Encoding.UTF8.GetBytes(response));
    }
    private T GetModel<T>(string data)
    {
        return JsonSerializer.Deserialize<T>(data);
    }
    private IDataMessage HandleData(RegisterRequest request)
    {
        if (IsLoginAvailable(request.Login))
        {
            var user = new ChatUser()
            {
                Login = request.Login,
                Name = request.Name
            };
            try
            {
                RegisterUser(user, request.Password);
                var userFromDb = GetUserByLogin(request.Login);
                if (userFromDb != null)
                {
                    var response = new RegisterResponse()
                    {
                        Success = true,
                        User = new ChatUser()
                        {
                            Id = userFromDb.Id,
                            Login = userFromDb.Login,
                            Name = userFromDb.Name
                        },
                    };
                    return response;
                }
                else
                {
                    var response = new RegisterResponse()
                    {
                        Success = false,
                        User = null,
                        Error = "DataBase error."
                    };
                    return response;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                var response = new RegisterResponse()
                {
                    Success = false,
                    User = null,
                    Error = $"Exception: {ex.Message}"
                };
                return response;
            }
        }
        else
        {
            var response = new RegisterResponse()
            {
                Success = false,
                User = null,
                Error = "Such login already exists!"
            };
            return response;
        }
    }
    private IDataMessage HandleData(LoginRequest request)
    {
        if (!IsLoginAvailable(request.Login))
        {
            try
            {
                var userFromDb = GetUserByLogin(request.Login);
                var userFromDbPassword = GetPasswordFromLogin(userFromDb.Login.ToString());
                var pass = request.Password;
                if (userFromDb != null)
                {
                    if(userFromDbPassword.Equals(pass))
                    {
                        var response = new LoginResponse()
                        {
                            Success = true,
                            User = new ChatUser()
                            {
                                Id = userFromDb.Id,
                                Login = userFromDb.Login,
                                Name = userFromDb.Name
                            },
                        };
                        return response;
                    }
                    else
                    {
                        var response = new LoginResponse()
                        {
                            Success = false,
                            User = null,
                            Error = "Wrong password!"
                        };
                        return response;
                    }
                }
                else
                {
                    var response = new LoginResponse()
                    {
                        Success = false,
                        User = null,
                        Error = "DataBase error."
                    };
                    return response;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                var response = new LoginResponse()
                {
                    Success = false,
                    User = null,
                    Error = $"Exception: {ex.Message}"
                };
                return response;
            }
        }
        else
        {
            var response = new LoginResponse()
            {
                Success = false,
                User = null,
                Error = "Wrong login."
            };
            return response;
        }
    }
    private IDataMessage HandleData(AllUsersRequest request)
    {
        try
        {
            var allUsers = _db.Users.ToList();
            var res = new List<ChatUser>();

            allUsers.ForEach(u =>
            {
                var user = new ChatUser
                {
                    Id = u.Id,
                    Name = u.Name,
                    Login = u.Login
                };
                res.Add(user);
            });

            var response = new AllUsersResponse()
            {
                Success = true,
                Error = null,
                AllUsers = res
            };
            return response;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            var response = new AllUsersResponse()
            {
                Success = false,
                AllUsers = null,
                Error = $"Exception: {ex.Message}"
            };
            return response;
        }

    }
    private IDataMessage HandleData(SendMessageRequest request)
    {
        var sender = _db.Users.FirstOrDefault(x => x.Id== request.Sender.Id);
        var receiver = _db.Users.FirstOrDefault(x => x.Id == request.Receiver.Id);
        var message = new Message
        {
            Sender = sender,
            Receiver = receiver,
            TextContent = request.TextMessage,
            CreatedAt = DateTime.Now,
        };
        _db.Messages.Add(message);
        _db.SaveChanges();

        return new SendMessageResponse()
        {
            Error = null,
            Success = true,
            Id = message.Id
        };
    }
    private IDataMessage HandleData(GetMessagesRequest request)
    {
        return new GetMessagesResponse()
        {
            Messages = _db.Messages.Include(x => x.Sender).Include(x => x.Receiver)
            .Where(x => x.Sender.Id == request.Questioner.Id || x.Receiver.Id == request.Questioner.Id)
            .Where(x => x.Id > request.IdAfter)
            .OrderBy(x => x.Id)
            .Select(m => new ChatMessage
            {
                Id = m.Id,
                TextMessage = m.TextContent,
                CreatedAt = m.CreatedAt,
                Sender = GetChatUserFromDbUser(m.Sender),
                Receiver = GetChatUserFromDbUser(m.Receiver)
            }).ToList()
        };
    }
    public void Handle(Socket socket)
    {
        var buffer = new byte[1024];
        var read = socket.Receive(buffer);
        var incoming = Encoding.UTF8.GetString(buffer, 0, read);
        var request = JsonSerializer.Deserialize<DataMessage>(incoming);

        switch (request.Type)
        {
            case DataType.REGISTER_REQUEST:
                Send(socket, HandleData(GetModel<RegisterRequest>(request.Data)));
                break;
            case DataType.LOGIN_REQUEST:
                Send(socket, HandleData(GetModel<LoginRequest>(request.Data)));
                break;
            case DataType.ALLUSERS_REQUEST:
                Send(socket, HandleData(GetModel<AllUsersRequest>(request.Data)));
                break;
            case DataType.SENDMESSAGE_REQUEST:
                Send(socket, HandleData(GetModel<SendMessageRequest>(request.Data)));
                break;
            case DataType.GETMESSAGES_REQUEST:
                Send(socket, HandleData(GetModel<GetMessagesRequest>(request.Data)));
                break;
            default:
                break;
        }
    }
    private ChatDbContext _db;
    public bool IsLoginAvailable(string login)
    {
        if (login == null)
            return false;
        var logins = _db.Users.ToList();
        foreach (var l in logins)
        {
            if (l.Login.Equals(login))
            {
                return false;
            }
        };
        return true;
    }
    public void RegisterUser(ChatUser user, string password)
    {
        if (user == null)
            return;
        var usr = new User()
        {
            Name = user.Name,
            Login = user.Login,
            IsOnline = true
        };
        _db.Users.Add(usr);
        _db.LoginPasswords.Add(new LoginPassword { User = usr, HashPassword = password });
        _db.SaveChanges();
    }
    public User? GetUserByLogin(string login)
    {
        return _db.Users.FirstOrDefault(x => x.Login.Equals(login));
    }
    public static ChatUser? GetChatUserFromDbUser(User user)
    {
        return new ChatUser { Id = user.Id, Login = user.Login, Name=user.Name };
    }
    public string GetPasswordFromLogin(string login)  //REDO to hash
    {
        var passwords = _db.LoginPasswords.ToList();
        if(passwords.FirstOrDefault(x => x.User.Login.Equals(login)) != null)
        {
            return passwords.FirstOrDefault(x => x.User.Login.Equals(login)).HashPassword;
        }
        return String.Empty;
    }
}
