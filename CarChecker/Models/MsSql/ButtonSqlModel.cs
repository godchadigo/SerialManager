using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugarTester.Models
{
    [SugarTable("ButtonSql")]
    public class ButtonSqlModel
    {
        [SugarColumn(IsPrimaryKey = true)]
        public Guid Uuid { get; set; }
        public DateTime DataTime { get; set; }
        public string Status { get; set; }
        public string Barcode { get; set; }
        public string RawResult { get; set; }        
        public string Remark { get; set; }        
    }
}
