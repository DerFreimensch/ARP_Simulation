namespace ARP_Simulation;

public class Ethernet
{
    public byte[] preambul = new byte[8];
    public byte[] recAddr = new byte[6];
    public byte[] sndAddr = new byte[6];
    public byte[] management = new byte[2];
    public byte[] data { get; set; }
    private Random _rand = new Random();

    public Ethernet(Arp data, byte[] recaddr, byte[] sndaddr)
    {
        
        for (var i = 0; i < 7; i++)
        {
            preambul[i] = 170;
        }

        preambul[7] = 171;
        recAddr = recaddr;
        sndAddr = sndaddr;
        management[0] = 8;
        management[1] = 6;
        this.data = data.GetAsByte();
    }
    
    public Ethernet(byte[] data1, byte[] recaddr, byte[] sndaddr)
    {
        
        for (var i = 0; i < 7; i++)
        {
            preambul[i] = 170;
        }

        preambul[7] = 171;
        recAddr = recaddr;
        sndAddr = sndaddr;
        management[0] = 8;
        management[1] = 6;
        this.data = data1;
    }

    public bool eq(byte[] a)
    {
        for (var i = 0; i < 6; i++)
        {
            if (recAddr[i] != a[i]) return false;
        }

        return true;
    }
    
    public byte[] GetIp()
    {
        var o = new byte[4];
        for (var i = 0; i < 4; i++)
        {
            o[i] = data[24 + i];
        }

        return o;
    }
    
    public byte[] GetAsByte()
    {
        var size = 22 + data.Length;
        var asByte = new byte[size];
        for (var i = 0; i < 8; i++)
        {
            asByte[i] = preambul[i];
        }
        for (var i = 0; i < 6; i++)
        {
            asByte[8 + i ] = recAddr[i];
            asByte[14 + i ] = sndAddr[i];
        }

        asByte[20] = management[0];
        asByte[21] = management[1];
        for (var i = 0; i < data.Length; i++)
        {
            asByte[34 + i] = data[i];
        }
        return asByte;
    }

    public List<byte[]> GetAddrRec()
    {
        var ip = new byte[4];
        var mac = new byte[6];
        for (var i = 0; i < 6; i++)
        {
           mac[i] = data[18 + i];
        }
        for (var i = 0; i < 4; i++)
        {
            ip[i] = data[24 + i];
        }

        return new List<byte[]> {ip, mac};
    }
    
    public List<byte[]> GetAddrSnd()
    {
        var ip = new byte[4];
        var mac = new byte[6];
        for (var i = 0; i < 6; i++)
        {
            mac[i] = data[8 + i];
        }
        for (var i = 0; i < 4; i++)
        {
            ip[i] = data[14 + i];
        }

        return new List<byte[]> {ip, mac};
    }
}