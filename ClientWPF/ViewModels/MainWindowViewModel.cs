using ConsoleApp20_ServerChat.Models_Server_Client;
using My.BaseViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ClientWPF.ViewModels;

public class MainWindowViewModel : NotifyPropertyChangedBase
{
    public MainWindowViewModel() 
    {
        _login = String.Empty;
        _password = String.Empty;
        _infoMessage = String.Empty;
        _loginRegister = String.Empty;
        _nameRegister = String.Empty;
        _passwordRegister = String.Empty;
        _passwordConformationRegister = String.Empty;

        _client = new ChatClientWPF("127.0.0.1", 5555);
    }
    private string _login;
    private string _password;
    public string Login
    {
        get => _login;
        set 
        { 
            _login = value;
            OnPropertyChanged(nameof(Login));
        }
    }
    public string Password
    {
        get => _password;
        set
        {
            _password = value;
            OnPropertyChanged(nameof(Password));
        }
    }
    private string _infoMessage;
    public string InfoMessage
    {
        get => _infoMessage;
        set
        {
            _infoMessage = value;
            OnPropertyChanged(nameof(InfoMessage));
        }
    }
    private ChatClientWPF _client;
    public ICommand LoginCommand => new RelayCommand(x =>
    {
        var request = new RegisterRequest
        {
            Name = "Test",
            Login = "user1",
            Password = "user1",
        };
        var response = _client.Register(request);

    }, x => _login.Length>0 && _password.Length>0);

    private string _loginRegister;
    private string _nameRegister;
    private string _passwordRegister;
    private string _passwordConformationRegister;
    public string LoginRegister
    {
        get => _loginRegister;
        set
        {
            _loginRegister = value;
            OnPropertyChanged(nameof(LoginRegister));
        }
    }
    public string NameRegister
    {
        get => _nameRegister;
        set
        {
            _nameRegister = value;
            OnPropertyChanged(nameof(NameRegister));
        }
    }
    public string PasswordRegister
    {
        get => _passwordRegister; 
        set
        {
            _passwordRegister = value;
            OnPropertyChanged(nameof(PasswordRegister));
        }
    }
    public string PasswordConformationRegister
    {
        get => _passwordConformationRegister;
        set
        {
            _passwordConformationRegister = value;
            OnPropertyChanged(nameof(PasswordConformationRegister));
        }
    }
    public ICommand RegisterCommand => new RelayCommand(x =>
    {
        var request = new RegisterRequest
        {
            Name = _nameRegister,
            Login = _loginRegister,
            Password = _passwordRegister,  //re-do to hash
        };
        var response = _client.Register(request);
        if(response != null)
        {
            if(response.Success)
            {
                MessageBox.Show($"User registered successfully with id No.{response.User.Id}.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Registration error.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }, x => _loginRegister.Length > 0 && _nameRegister.Length > 0 && _passwordRegister.Length>0 && _passwordRegister.Equals(_passwordConformationRegister));

}
