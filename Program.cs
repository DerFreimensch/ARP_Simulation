// See https://aka.ms/new-console-template for more information

using ARP_Simulation;

var user1 = new User(1);
var user2 = new User(2);
var user3 = new User(3);
var user4 = new User(4);
var user5 = new User(5);
var user6 = new User(6);
var user7 = new User(7);
var user8 = new User(8);

user1.AddConnect(new List<User>{user2});
user2.AddConnect(new List<User>{user1, user3, user5, user7});
user3.AddConnect(new List<User>{user2, user4});
user4.AddConnect(new List<User>{user3, user5, user6});
user5.AddConnect(new List<User>{user2, user4, user6});
user6.AddConnect(new List<User>{user4, user5, user7});
user7.AddConnect(new List<User>{user2, user6, user8});
user8.AddConnect(new List<User>{user7});


var users = new List<User>
{
    user1, user2, user3,
    user4, user5, user6, 
    user7, user8
};
users.ForEach(x=>x.ShowCurrentArpTable());

Console.WriteLine();
Console.WriteLine();

user1.ShowCurrentArpTable();
user3.ShowCurrentArpTable();
Console.WriteLine();
user3.CreateArpMeassage(user1.IP);
Console.WriteLine();Console.WriteLine();
user3.SendArpMessage();
Console.WriteLine();Console.WriteLine();
user1.ShowCurrentArpTable();
user3.ShowCurrentArpTable();

Console.WriteLine();
user3.CreateArpMeassage(user8.IP);
Console.WriteLine();Console.WriteLine();
user3.SendArpMessage();
Console.WriteLine();Console.WriteLine();
user8.ShowCurrentArpTable();
user3.ShowCurrentArpTable();





