using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NonProtocol.Enums.CarProtocolEunm;

namespace CarChecker.Interface
{
    public interface ICheckModel
    {
        int ButtonID { get; set; }
        ReadButtonArea CheckArea { get; set; }
    }
}
