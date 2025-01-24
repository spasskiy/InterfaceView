using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceView.Model
{
    public class CellDataXml
    {
        public string? Category { get; set; }
        public int Address { get; set; }
        public string? NumberReg { get; set; }
        public string? NameDevice { get; set; }
        public string? Name { get; set; }
        public string? Format { get; set; }
        public string? Type { get; set; }
        public Uelement? Uelement { get; set; }
    }

    public class Uelement
    {
        public int Value { get; set; }
    }
}
