using System.Drawing;

namespace ARP_Simulation;

public class Arp
{
    public byte[] Type { get; set; }
    public byte[] Protocol { get; set; }
    public byte MacSize { get; set; }
    public byte IpSize { get; set; }
    public byte[] QA { get; set; }
    public byte[] SndMacAddr { get; set; }
    public byte[] SndIpAddr { get; set; }
    public byte[] RecMacAddr { get; set; }
    public byte[] RecIpAddr { get; set; }

    public Arp()
    {
        
    }
    public Arp(
        byte[] type, 
        byte[] sma,
        byte[] sia,
        byte[] rma,
        byte[] ria
        )
    {
        var rand = new Random();
        Type = new byte[] {0, 1};
        Protocol = new byte[] {8, 0};
        MacSize = 6;
        IpSize = 4;
        QA = type;
        SndMacAddr = sma; 
        SndIpAddr = sia;
        RecMacAddr = rma;
        RecIpAddr = ria;
    }

    public byte[] GetAsByte()
    {
        var size = 8 + 2 * (MacSize + IpSize);
        var asByte = new byte[size];
        for (var i = 0; i < 2; i++)
        {
            asByte[i] = Type[i];
            asByte[i + 2] = Protocol[i];
            asByte[i + 6] = QA[i];
        }

        asByte[4] = MacSize;
        asByte[5] = IpSize;
        for (var i = 0; i < 6; i++)
        {
            asByte[8 + i] = SndMacAddr[i];
            asByte[18 + i] = RecMacAddr[i];

        }

        for (var i = 0; i < 4; i++)
        {
            asByte[14 + i] = SndIpAddr[i];
            asByte[24 + i] = RecIpAddr[i];
        }
        return asByte;
    }

}