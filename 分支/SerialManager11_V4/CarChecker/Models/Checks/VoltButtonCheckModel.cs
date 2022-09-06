using CarChecker.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NonProtocol.Enums.CarProtocolEunm;

namespace CarChecker.Models.Checks
{
    public class VoltButtonCheckModel : ICheckModel
    {
        public ReadButtonArea CheckArea { get; set; }
        public int VoltValue { get; set; }        
        public int ButtonID { get; set; }
    }
}
