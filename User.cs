namespace ARP_Simulation;

public class User
{
    public int ID { get; set; }
    public byte[] IP { get; set; }
    public byte[] Mac { get; set; }
    private Dictionary<byte[], byte[]> ArpTable { get; set; }
    private List<User> NearUsers { get; set; }
    private Random rand = new Random();
    private List<User> _priv;
    private Arp arp;

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

    public void CreateArpMeassage(byte[] recIp)
    {
        var broadcast = new byte[6] {255, 255, 255, 255, 255, 255};
        arp = new Arp(new byte[] {0, 1}, Mac, IP, broadcast, recIp);
    }

    public void SendArpMessage(byte[] recIp, Arp data)
    {
        var broadcast = new byte[6] {255, 255, 255, 255, 255, 255};
        var ethernet = new Ethernet(data, broadcast, Mac);
        Ethernet answer = null;
        foreach (var user in NearUsers)
        {
            user._priv.Add(this);
            answer = user.ReceiveMessage(ethernet);
        }

        //ArpTable.Add(recIp, answer.sndAddr);
    }

    public void SendAnswerMessage(Ethernet e)
    {
        Ethernet answer = null;
        foreach (var user in NearUsers)
        {
            user._priv.Clear();
            user._priv.Add(this);
            answer = user.ReceiveMessage(e);
            ArpTable.TryAdd(answer.GetAddrSnd()[0], answer.sndAddr);
        }
        //ArpTable.TryAdd(answer.GetAddrSnd()[0], answer.sndAddr);
    }

    public Ethernet ReceiveMessage(Ethernet e)
    {
        //Arp arp = new Arp();
        //byte[] mac = new byte[6] {0, 0, 0, 0, 0, 0};
        if (e.eq(IP))
        {
            return e;
        }
        if (ArpTableContained(e.GetIp()))
        {
            var mac = GetMacFromArp(e.GetIp());
            var ip = GetIpFromArp(e.GetIp());
            var arp = new Arp(new byte[] {0, 2}, mac , ip , e.GetAddrSnd()[1], e.GetAddrSnd()[0]);
            var ethernet = new Ethernet(arp, e.GetAddrSnd()[1], mac);
            SendAnswerMessage(ethernet);
        }
        else
        {
            if (e.eq(new byte[] {255, 255, 255, 255, 255, 255}))
            {
                foreach (var user in NearUsers)
                {
                    user._priv.Add(this);
                    if (!_priv.Contains(user))
                    {
                        //var ethernet = new Ethernet(e.data, e.recAddr, Mac);
                        user.SendArpMessage(e.GetIp());
                    }
                }
            }

        }
        return e;
    }

    public bool comp(byte[] a, byte[] b)
    {
        return !a.Where((t, i) => t != b[i]).Any();
    }
    public byte[] GetMacFromArp(byte[] a)
    {
        foreach (var line in ArpTable)
        {
            if (comp(line.Key, a)) return line.Value;
        }
        return null;
    }
    public byte[] GetIpFromArp(byte[] a)
    {
        foreach (var line in ArpTable)
        {
            if (comp(line.Key, a)) return line.Key;
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
            Console.WriteLine($"IP: {line.Key[0]}:{line.Key[1]}:{line.Key[2]}:{line.Key[3]} | MAC {line.Value[0]}:{line.Value[1]}:{line.Value[2]}:{line.Value[3]}:{line.Value[4]}:{line.Value[5]}");
        }
    }
}