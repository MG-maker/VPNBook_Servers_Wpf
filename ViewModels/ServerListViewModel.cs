using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VPNBook_Servers_Wpf.Base;
using VPNBook_Servers_Wpf.Models;

namespace VPNBook_Servers_Wpf.ViewModels
{
    public class ServerListViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<ServerModel> Servers { get; set; }

        private ServerModel _selectedServer = null!;
        public ServerModel SelectedServer
        {
            get { return _selectedServer; }
            set
            {
                _selectedServer = value;
            }
        }

        private string _connectionStatus = "Disconnected";

        public string ConnectionStatus
        {
            get { return _connectionStatus; }
            set
            {
                _connectionStatus = value;
            }
        }

        private string _serverLogin = "vpnbook";
        private string _serverPassword = "dd4e58m";// you should change it on your password

        public event PropertyChangedEventHandler? PropertyChanged;

        public RelayCommand ConnectCommand { get; set; }
        public RelayCommand DisconnectCommand { get; set; }

        public ServerListViewModel() 
        {
            Servers = new ObservableCollection<ServerModel>();

            Servers.Add(new ServerModel
            {
                ServerImage= "https://i.imgur.com/tX2FzGr.png",
                ServerName = "us1.vpnbook.com",
                Country="USA"
            });   
            Servers.Add(new ServerModel
            {
                ServerImage= "https://i.imgur.com/bhZ7aBe.png",
                ServerName = "de4.vpnbook.com",
                Country="Germany"
            }); 
            Servers.Add(new ServerModel
            {
                ServerImage= "https://i.imgur.com/OampSZ0.jpeg",
                ServerName = "PL226.vpnbook.com",
                Country="Poland"
            }); 
            Servers.Add(new ServerModel
            {
                ServerImage= "https://i.imgur.com/r0v4Jkp.png",
                ServerName = "fr1.vpnbook.com",
                Country="France"
            });   
            Servers.Add(new ServerModel
            {
                ServerImage= "https://i.imgur.com/pFww7UB.png",
                ServerName = "ca222.vpnbook.com",
                Country="Canada"
            });

        ConnectCommand = new RelayCommand(o =>
            {
                if(ConnectionStatus == "Connected")
                    MessageBox.Show("You should disconnect current connection and then connect to another VPN server.", "Connection Server", MessageBoxButton.OK, MessageBoxImage.Warning);
                else
                Task.Run(() =>
                {
                    ConnectionStatus = "Connecting...";
                    var process = new Process();
                    process.StartInfo.FileName = "cmd.exe";
                    process.StartInfo.WorkingDirectory = Directory.GetCurrentDirectory();

                    switch (SelectedServer.Country)
                    {
                        case "USA":
                            
                            process.StartInfo.ArgumentList.Add(@$"/c rasdial MyServerVPN_US {_serverLogin} {_serverPassword} /phonebook:./VPN/us1.vpnbook.com.pbk");
                            break;
                        case "France":
                            process.StartInfo.ArgumentList.Add(@$"/c rasdial MyServerVPN_FR {_serverLogin} {_serverPassword} /phonebook:./VPN/fr1.vpnbook.com.pbk");
                            break;
                        case "Germany":
                            process.StartInfo.ArgumentList.Add(@$"/c rasdial MyServerVPN_DE {_serverLogin} {_serverPassword} /phonebook:./VPN/DE4.vpnbook.com.pbk");
                            break;
                        case "Canada":
                            process.StartInfo.ArgumentList.Add(@$"/c rasdial MyServerVPN_CAN {_serverLogin} {_serverPassword} /phonebook:./VPN/ca222.vpnbook.com.pbk");
                            break;
                        case "Poland":
                            process.StartInfo.ArgumentList.Add(@$"/c rasdial MyServerVPN_POL {_serverLogin} {_serverPassword} /phonebook:./VPN/PL226.vpnbook.com.pbk");
                            break;
                    }

                    process.Start();
                    process.WaitForExit();

                    switch (process.ExitCode)
                    {
                        case 0:
                            MessageBox.Show($"Success! You connected to {SelectedServer.Country} server.", "Connection Server", MessageBoxButton.OK, MessageBoxImage.Information);
                            ConnectionStatus = "Connected";
                            break;
                        case 691:
                            MessageBox.Show("Wrong credentials!");
                            ConnectionStatus = "Wrong credentials!";
                            break;
                        default:
                            MessageBox.Show($"Error: {process.ExitCode}.", "Process Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            break;
                    }
                });
            });

        DisconnectCommand = new RelayCommand(o =>
            {
                if (ConnectionStatus == "Connected")
                {
                    Task.Run(() =>
                    {
                        var process = new Process();
                        process.StartInfo.FileName = "cmd.exe";
                        //  rasdial / d - disconnect vpn server
                        process.StartInfo.ArgumentList.Add(@"/c rasdial/d");
                        process.Start();
                        process.WaitForExit();
                        MessageBox.Show("Connection is disconnected! You can select VPN server to connection.", "Connection", MessageBoxButton.OK, MessageBoxImage.Information);
                        ConnectionStatus = "Disconnected";
                    });
                }
                else
                    MessageBox.Show("Connection is not created. Select VPN Server from List and click Connect to VPN Server.", "Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
            });

       //create file .pbk with vpn server options
       // ServerBuilder();
    }

        private void ServerBuilder()
        {
                var address = "us1.vpnbook.com";
            // var address = "fr1.vpnbook.com";
           //  var address = "DE4.vpnbook.com";
         //  var address = "ca222.vpnbook.com";
           // var address = "PL226.vpnbook.com";
            var FolderPath = $"{Directory.GetCurrentDirectory()}/VPN";
            var PbkPath = $"{FolderPath}/{address}.pbk";

            if (!Directory.Exists(FolderPath))
                Directory.CreateDirectory(FolderPath);

            if (File.Exists(PbkPath))
            {
                MessageBox.Show("Connection already exists!");
                return;
            }

            var sb = new StringBuilder();
                   sb.AppendLine("[MyServerVPN_US]");
             // sb.AppendLine("[MyServerVPN_FR]");
           //  sb.AppendLine("[MyServerVPN_DE]");
           // sb.AppendLine("[MyServerVPN_CAN]");
         //   sb.AppendLine("[MyServerVPN_POL]");
            sb.AppendLine("MEDIA=rastapi");
            sb.AppendLine("Port=VPN2-0");
            // sb.AppendLine("Device=WAN Miniport (IKEv2)");//error 720 No PPP control protocols configured.
            sb.AppendLine("Device=WAN Miniport (PPTP)");
            sb.AppendLine("DEVICE=vpn");
            sb.AppendLine($"PhoneNumber={address}");
            File.WriteAllText(PbkPath, sb.ToString());
        }

    }
}
