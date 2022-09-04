using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NonProtocol.Enums.CarProtocolEunm;

namespace CarChecker.Models
{
    public class ResultDataBindingModel
    {
        /// <summary>
        /// 綁定表格ID
        /// </summary>
        public int BindingGridID { get; set; }
        public ReadButtonArea Area { get; set; }
        public ReadButtonType CheckType { get; set; }
        /// <summary>
        /// 綁定觸發按鈕ID
        /// </summary>
        public int BindingButtonID { get; set; }
        

    }
}
