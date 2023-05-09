using ClientWPF.Windows;
using ConsoleApp20_ServerChat.Models;
using ConsoleApp20_ServerChat.Models_Server_Client;
using My.BaseViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

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
        _chatMessages = new List<ChatMessageModel>();
        GetMessages();
        _textMessage = String.Empty;
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
            OnPropertyChanged(nameof(ChatMessages));
        }
    }
    private string _textMessage;
    public string TextMessage
    {
        get => _textMessage;
        set
        {
            _textMessage = value;
            OnPropertyChanged(nameof(TextMessage));
        }
    }
    public ICommand SendCommand => new RelayCommand(x =>
    {
        var request = new SendMessageRequest
        {
            TextMessage = _textMessage,
            Sender = _user,
            Receiver = _selectedUser
        };
        var response = _client.SendMessage(request);
        if (response != null)
        {
            if (response.Success)
            {
                MessageBox.Show("Success", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                //GetMessages();
            }
            else
            {
                MessageBox.Show($"Error.\n{response.Error}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }, x => _textMessage.Length > 0 && _selectedUser !=null);
    public void GetMessages()
    {
        var request = new GetMessagesRequest
        {
            Questioner = _user,
            IdAfter = 0
        };
        var response = _client.GetMessages(request);
        
        response.Messages.ForEach(m =>
        {
            _chatMessages.Add(new ChatMessageModel { Id = m.Id, Sender = m.Sender, Receiver = m.Receiver, TextMessage = m.TextMessage, CreatedAt = m.CreatedAt, AmIReceiver = (m.Receiver.Id==_user.Id) });
        });
        OnPropertyChanged(nameof(ChatMessages));
    }
    private List<ChatMessageModel> _chatMessages;
    public ObservableCollection<ChatMessageModel> ChatMessages
    {
        get
        {
            var collection = new ObservableCollection<ChatMessageModel>();
            if (_selectedUser != null)
            {
                _chatMessages.Where(x => x.Sender.Id == _selectedUser.Id || x.Receiver.Id == _selectedUser.Id).ToList().ForEach(collection.Add);
            }
            return collection;
        }
    }
}
