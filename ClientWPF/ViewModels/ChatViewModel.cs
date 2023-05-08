using ConsoleApp20_ServerChat.Models;
using ConsoleApp20_ServerChat.Models_Server_Client;
using My.BaseViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientWPF.ViewModels;

public class ChatViewModel : NotifyPropertyChangedBase
{
    public ChatViewModel(ChatClientWPF client, ChatUser user) 
    {
        _client = client;
        _user = user;
        _users = new List<ChatUser> ();
        //request for users and messages from db
        var request = new AllUsersRequest
        {
            Login = _user.Login,
        };
        var response = _client.AllUsers(request);
        if (response.Success)
        {
            _users = response.AllUsers;
            OnPropertyChanged(nameof(Users));
        }
    }
    private ChatClientWPF _client;
    private ChatUser _user;
    public string Login
    {
        get => _user.Login;
    }
    public string Name
    {
        get => _user.Name;
    }
    private List<ChatUser> _users;
    public ObservableCollection<ChatUser> Users
    {
        get
        {
            var collection = new ObservableCollection<ChatUser>();
            _users.ForEach(u =>
            {
                if(!u.Login.Equals(_user.Login))
                {
                    collection.Add(u);  
                }
            }); 
            return collection;
        }
    }
    private ChatUser _selectedUser;
    public ChatUser SelectedUser
    {
        get => _selectedUser;
        set
        {
            _selectedUser = value;
            OnPropertyChanged(nameof(SelectedUser));
        }
    }
}
