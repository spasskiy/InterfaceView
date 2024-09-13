using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceView.Model
{
    public struct IPAddress
    {
        public byte Octet1 { get; set; }
        public byte Octet2 { get; set; }
        public byte Octet3 { get; set; }
        public byte Octet4 { get; set; }

        public IPAddress(byte octet1, byte octet2, byte octet3, byte octet4)
        {
            Octet1 = octet1;
            Octet2 = octet2;
            Octet3 = octet3;
            Octet4 = octet4;
        }

        public IPAddress(string ipAddress)
        {
            var octets = ipAddress.Split('.');
            if (octets.Length != 4)
            {
                throw new ArgumentException("Invalid IP address format");
            }

            Octet1 = byte.Parse(octets[0]);
            Octet2 = byte.Parse(octets[1]);
            Octet3 = byte.Parse(octets[2]);
            Octet4 = byte.Parse(octets[3]);
        }

        public override string ToString()
        {
            return $"{Octet1}.{Octet2}.{Octet3}.{Octet4}";
        }
    }
}
