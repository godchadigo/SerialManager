using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NonProtocol.Enums.CarProtocolEunm;

namespace NonProtocol.Models
{
    public class CarModel
    {
        public ReadButtonArea Area { get; set; }
        public ReadButtonStatus Status { get; set; }
        public ReadButtonType Type { get; set; }
        public int ButtonID { get; set; }
        public long TS { get; set; }
        public byte[] RawData { get; set; }
        public string Message { get; set; }
    }
}
