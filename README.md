# VPNBook_Servers_Wpf
Wpf application for connect to VPNBook VPN Servers

First of all you need to creat .pbk files with vpn server options. Method ServerBuilder() in ServerListViewModel class allows you do it. 

1. You should use one variable "adress" and one server name
---for example:
var address = "us1.vpnbook.com"; 
sb.AppendLine("[MyServerVPN_US]");

2. Uncomment Method ServerBuilder() and launch project. After that .pbk file will be created.
3. To create another .pbk files with VPN server options you need to use another pairs of addresses and server names. 
For example:
var address = "fr1.vpnbook.com";
sb.AppendLine("[MyServerVPN_FR]");
.....etc....
