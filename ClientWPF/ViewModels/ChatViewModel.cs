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
using System.Threading;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Threading;
using Microsoft.Win32;

namespace ClientWPF.ViewModels;

public class ChatViewModel : NotifyPropertyChangedBase
{
    public ChatViewModel(ChatClientWPF client, ChatUser user) 
    {
        _client = client;
        _user = user;
        _users = new List<ChatUser> ();
        _attachedImagePath = String.Empty;
        //_run = run;
        _lastSelectedUserId = -1;
        //request for users and messages from db
        //var request = new AllUsersRequest
        //{
        //    Login = _user.Login,
        //};
        //var response = _client.AllUsers(request);
        //if (response.Success)
        //{
        //    _users = response.AllUsers;
        //    OnPropertyChanged(nameof(Users));
        //}
        GetUsers();
        _chatMessages = new List<ChatMessageModel>();
        //GetMessages();
        UpdateMessagesTask();
        UpdateUserStatus();
        _textMessage = String.Empty;
    }
    private ChatClientWPF _client;
    private ChatUser _user;
    //private bool _run;
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
            _lastSelectedUserId = _selectedUser.Id;
            OnPropertyChanged(nameof(SelectedUser));
            OnPropertyChanged(nameof(ChatMessages));
        }
    }
    private int _lastSelectedUserId;
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
                //MessageBox.Show("Success", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                _textMessage = String.Empty;
                OnPropertyChanged(nameof(TextMessage));
                //GetMessages();
            }
            else
            {
                MessageBox.Show($"Error.\n{response.Error}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }, x => _textMessage.Length > 0 && _selectedUser !=null);
    public void GetUsers()
    {
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
    public void GetMessages()
    {
        var request = new GetMessagesRequest
        {
            Questioner = _user,
            IdAfter = GetLastMessageId()
        };
        var response = _client.GetMessages(request);
        
        response.Messages.ForEach(m =>
        {
            _chatMessages.Add(new ChatMessageModel { Id = m.Id, Sender = m.Sender, Receiver = m.Receiver, TextMessage = m.TextMessage, CreatedAt = m.CreatedAt, AmIReceiver = (m.Receiver.Id==_user.Id), InfoText = (m.Receiver.Id == _user.Id ? "received":"sended") });
        });
        OnPropertyChanged(nameof(ChatMessages));
    }
    public void UpdateMessagesTask()
    {
        Task t = new Task (() =>
        {
            while(true)
            {
                GetMessages();
                Thread.Sleep(1500);
            }
        });
        t.Start();
    }
    public void UpdateUserStatus()
    {
        Task t = new Task(() =>
        {
            while (true)
            {
                var request = new AllUsersRequest
                {
                    Login = _user.Login,
                };
                var response = _client.AllUsers(request);
                if (response.Success)
                {
                    var updatedUsers = response.AllUsers;
                    _users.ForEach(user =>
                    {
                        var u = updatedUsers.FirstOrDefault(x => x.Id == user.Id);
                        if (user.IsOnline != u.IsOnline)
                        {
                            _users = updatedUsers;
                            OnPropertyChanged(nameof(Users));
                            return;
                        }
                    });
                }
                Thread.Sleep(2500);
            }
        });
        t.Start();
    }
    public int GetLastMessageId()
    {
        int lastMessageId = 0;
        if(_chatMessages.Count ==0)
        {
            return 0;
        }
        _chatMessages.ForEach(m =>
        {
            int last = m.Id;
            if(last> lastMessageId)
            {
                lastMessageId = last;
            }
        });
        return lastMessageId;
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
    public ICommand LogoutCommand => new RelayCommand(x =>
    {
        //REDO
        var request = new LogoutRequest
        {
            User = _user
        };
        var response = _client.Logout(request);
        if (!response.Success)
        {
            MessageBox.Show("Something went wrong!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        System.Windows.Application.Current.Dispatcher.Invoke(
        System.Windows.Threading.DispatcherPriority.Normal, (Action)delegate
        {
            for (int intCounter = App.Current.Windows.Count - 1; intCounter >= 0; intCounter--)
            {
                if (!App.Current.Windows[intCounter].Equals(App.Current.MainWindow))
                {
                    App.Current.Windows[intCounter].Dispatcher.Invoke(() =>
                    {
                        App.Current.Windows[intCounter].Close();
                    });
                }
            }
            Application.Current.MainWindow.Show();
        });
       
        //REQUEST TO SERVER FOR OFFLINE USER
    }, x => true);
    private string _attachedImagePath;
    public ICommand AttachFileCommand => new RelayCommand(x =>
    {
        var dialog = new OpenFileDialog();

        dialog.Title = "Open Image";
        dialog.Filter = "All Images|*.BMP;*.DIB;*.RLE;*.JPG;*.JPEG;*.JPE;*.JFIF;*.GIF;*.TIF;*.TIFF;*.PNG";

        if (dialog.ShowDialog() == true)
        {
            _attachedImagePath = dialog.FileName;
        }

    }, x => true);
}
