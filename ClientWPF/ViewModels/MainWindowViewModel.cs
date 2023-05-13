using ClientWPF.Windows;
using ConsoleApp20_ServerChat.Models_Server_Client;
using My.BaseViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
        _isPasswordHidden = true;

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
        get
        {
            if (_isPasswordHidden)
            {
                return new string('*', _password.Length); ;
            }
            return _password;
        }
        set
        {
            _password = value;
            OnPropertyChanged(nameof(Password));
        }
    }
    private bool _isPasswordHidden;
    public bool IsPasswordHidden
    {
        get => _isPasswordHidden;
        set
        {
            _isPasswordHidden = value;
            OnPropertyChanged(nameof(IsPasswordHidden));
            OnPropertyChanged(nameof(Password));
            OnPropertyChanged(nameof(ShowHideButtonText));
        }
    }
    public string ShowHideButtonText
    {
        get
        {
            if(IsPasswordHidden)
            {
                return "Show";
            }
            return "Hide";
        }
    }
    public ICommand ShowHidePassword => new RelayCommand(x =>
    {
        Password = (Application.Current.MainWindow as MainWindow).hiddenPass.Password;
        if (IsPasswordHidden)
        {
            (Application.Current.MainWindow as MainWindow).hiddenPass.Visibility = Visibility.Hidden;
            (Application.Current.MainWindow as MainWindow).visiblePass.Visibility = Visibility.Visible;
        }
        else
        {
            (Application.Current.MainWindow as MainWindow).visiblePass.Visibility = Visibility.Collapsed;
            (Application.Current.MainWindow as MainWindow).hiddenPass.Visibility = Visibility.Visible;
        }
        IsPasswordHidden = !IsPasswordHidden;
    }, x => true);

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
        InfoMessage = String.Empty;
        Password = (Application.Current.MainWindow as MainWindow).hiddenPass.Password;
        var request = new LoginRequest
        {
            Login = _login,
            Password = _password,
        };
        var response = _client.Login(request);
        if (response != null)
        {
            if (response.Success)
            {
                // new window
                MessageBox.Show($"Welcome {response.User.Name}.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                Application.Current.MainWindow.Hide();
                _login = String.Empty;
                _password = String.Empty;
                Password = String.Empty;    
                OnPropertyChanged(nameof(Login));
                OnPropertyChanged(nameof(Password));
                OnPropertyChanged(nameof(IsPasswordHidden));
                //bool run = true;
                var window = new ChatWindow(_client, new ChatUser { Login = response.User.Login, Name = response.User.Name, Id = response.User.Id });
                window.ShowDialog();

                //Thread t = new Thread(() =>
                //{
                //    var window = new ChatWindow(_client, new ChatUser { Login = response.User.Login, Name = response.User.Name, Id = response.User.Id });
                //    //Application.Current.MainWindow.Hide();
                //    window.ShowDialog();
                //});
                //t.SetApartmentState(ApartmentState.STA);
                //t.Start();
                //while (true)
                //{
                //    if(!run)
                //    {
                //        t.Abort();
                //        return;
                //    }
                //}
                //t.Abort();
                //t.Join();
                //if (!run)
                //{
                //    t.Abort();
                //}

                //Task tsk = new Task(() =>
                //{
                //    var window = new ChatWindow(_client, new ChatUser { Login = response.User.Login, Name = response.User.Name, Id = response.User.Id });
                //    //Application.Current.MainWindow.Hide();
                //    window.ShowDialog();
                //});
                ////tsk.SetApartmentState(ApartmentState.STA);
                //tsk.Start();

                //var window = new ChatWindow(_client, new ChatUser { Login = response.User.Login, Name = response.User.Name, Id=response.User.Id});
                //Application.Current.MainWindow.Hide();
                //window.ShowDialog();
            }
            else
            {
                InfoMessage = response.Error;
                OnPropertyChanged(nameof(InfoMessage));
                MessageBox.Show($"Error.\n{response.Error}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }, x => _login.Length>0);

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
                MessageBox.Show($"Error.\n {response.Error}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        NameRegister = String.Empty;
        LoginRegister = String.Empty;
        PasswordRegister = String.Empty;
        PasswordConformationRegister = String.Empty;
        OnPropertyChanged(nameof(NameRegister));
        OnPropertyChanged(nameof(LoginRegister));
        OnPropertyChanged(nameof(PasswordRegister));
        OnPropertyChanged(nameof(PasswordConformationRegister));

    }, x => _loginRegister.Length > 0 && _nameRegister.Length > 0 && _passwordRegister.Length>0 && _passwordRegister.Equals(_passwordConformationRegister));
}
