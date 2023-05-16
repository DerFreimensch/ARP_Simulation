namespace ARP_Simulation;

public class User
{
    public int ID { get; set; }
    public byte[] IP { get; set; }
    public byte[] Mac { get; set; }
    private Dictionary<byte[], byte[]> ArpTable { get; set; }
    public List<User> NearUsers { get; set; }
    private Random rand = new Random();
    private List<User> _priv;
    private byte[] Arp { get; set; }
    private Ethernet LastEthernet { get; set; }
    private byte[] broadcast = new byte[] { 255, 255, 255, 255, 255, 255 };
    public static bool Flag = false;
    

    public User(int id)
    {
        ArpTable = new Dictionary<byte[], byte[]>();
        ID = id;
        IP = new byte[4];
        Mac = new byte[6];
        rand.NextBytes(IP);
        rand.NextBytes(Mac);
        NearUsers = new List<User>();
        ArpTable.Add(IP, Mac);
        _priv = new List<User>();
    }

    public void AddConnect(List<User> c)
    {
        c.ForEach(x => NearUsers.Add(x));
    }

    private string ToString(byte[] a)
    {
        var o = "";
        for (var i = 0; i < a.Length-1; i++)
        {
            o += a[i] + ":";
        }

        return o + a[^1];
    }
    
    public void CreateArpMeassage(byte[] recIp)
    {
        Console.WriteLine($"Создан ARP пакет");
        Arp = new Arp(new byte[] { 0, 1 }, Mac, IP, broadcast, recIp).GetAsByte();
        LastEthernet = new Ethernet(Arp, broadcast, Mac);
    }
    
    
    public void SendArpMessage()
    {
        var ethernet = new Ethernet(Arp, broadcast, Mac);
        foreach (var user in NearUsers)
        {
            if (_priv.Contains(user)) continue;
            if (user.LastEthernet!=null) continue;
            user._priv.Add(this);
            foreach (var userUser in _priv) user._priv.Add(userUser);
            _priv.Add(user);
            Console.WriteLine($"Пакет запроса отправлен от {ID}:({ToString(Mac)}) к {user.ID}:({ToString(user.Mac)})");
            user.LastEthernet ??= ethernet;
            user.ReceiveMessage(ethernet);
            if (Flag) return;
        }

        //ArpTable.Add(recIp, answer.sndAddr);
    }

    public void SendAnswerMessage()
    {
        _priv.Clear();
        foreach (var user in NearUsers)
        {
            if (LastEthernet!=null && Comp(user.Mac, LastEthernet.sndAddr))
            {
                var ethernet = new Ethernet(Arp, LastEthernet.GetAddrSnd()[1], Mac);
                Console.WriteLine($"Пакет ответа отправлен от {ID}:({ToString(Mac)}) к {user.ID}:({ToString(user.Mac)})");
                var l = user.ArpTable.Count;
                user.ReceiveMessage(ethernet);
                if (Flag) return;
                LastEthernet = null;
                if (l < user.ArpTable.Count)
                {
                    Flag = true;
                    return;
                }
            }
        }
        
    }

    public void ReceiveMessage(Ethernet e)
    {
        Console.WriteLine($"Пакет принят {ID}:({ToString(Mac)}) от {ToString(e.sndAddr)}");
        //Arp arp = new Arp();
        //byte[] mac = new byte[6] {0, 0, 0, 0, 0, 0};
        if (Comp(Mac, e.recAddr))
        {
            Console.WriteLine($"Найден запрашивающий пользователь {ID}:({ToString(Mac)})");
            ArpTable.TryAdd(e.GetAddrSnd()[0], e.GetAddrSnd()[1]);
            return;
        }

        if (ArpTableContained(e.GetIp()))
        {
            var mac = GetMacFromArp(e.GetIp());
            var ip = GetIpFromArp(e.GetIp());
            Arp = new Arp(new byte[] { 0, 2 }, mac, ip, e.GetAddrSnd()[1], e.GetAddrSnd()[0]).GetAsByte();
            LastEthernet = e;
            //var ethernet = new Ethernet(Arp, e.GetAddrSnd()[1], mac);
            Console.WriteLine($"Найден искомый пользователь {ToString(ip)}: {ToString(mac)}");
            SendAnswerMessage();
        }
        else
        {
            if (e.eq(new byte[] { 255, 255, 255, 255, 255, 255 }))
            {
                Arp = e.data;
                Console.WriteLine("Пользователь не найден, продолжаем");
                SendArpMessage();
            }
            else
            {
                Arp = e.data;
                SendAnswerMessage();
            }
        }
    }

    public bool Comp(byte[] a, byte[] b)
    {
        return !a.Where((t, i) => t != b[i]).Any();
    }

    public byte[] GetMacFromArp(byte[] a)
    {
        foreach (var line in ArpTable)
        {
            if (Comp(line.Key, a)) return line.Value;
        }

        return null;
    }

    public byte[] GetIpFromArp(byte[] a)
    {
        foreach (var line in ArpTable)
        {
            if (Comp(line.Key, a)) return line.Key;
        }

        return null;
    }

    public bool ArpTableContained(byte[] a)
    {
        foreach (var line in ArpTable)
        {
            for (var i = 0; i < 4; i++)
            {
                if (line.Key[i] != a[i]) return false;
            }
        }

        return true;
    }

    public void ClearArpTable()
    {
        ArpTable.Clear();
        Console.WriteLine($"ARP таблица пользователя {ID}  очищена");
    }

    public void ShowCurrentArpTable()
    {
        Console.WriteLine($"ARP таблица пошльзователя {ID}");
        foreach (var line in ArpTable)
        {
            Console.WriteLine(
                $"IP: {line.Key[0]}:{line.Key[1]}:{line.Key[2]}:{line.Key[3]} | MAC {line.Value[0]}:{line.Value[1]}:{line.Value[2]}:{line.Value[3]}:{line.Value[4]}:{line.Value[5]}");
        }
    }

    public override string ToString()
    {
        return ID.ToString();
    }
}