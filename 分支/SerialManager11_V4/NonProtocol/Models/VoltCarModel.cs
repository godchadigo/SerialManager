using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NonProtocol.Enums.CarProtocolEunm;

namespace NonProtocol.Models
{
    public class VoltCarModel
    {
        public ReadButtonArea Area { get; set; }        
        public int ButtonID { get; set; }
        public int VoltValue { get; set; }
        public long TS { get; set; }
        public byte[] RawData { get; set; }
        public string Message { get; set; }
    }
}
