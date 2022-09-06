using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugarTester.Models
{
    [SugarTable("door_history_result")]
    public class DoorHistoryResult
    {
        //[SugarColumn(IsPrimaryKey = true)]
        //public Guid Uuid { get; set; }
        //public DateTime DataTime { get; set; }
        //public string Status { get; set; }
        //public string Barcode { get; set; }
        //public string RawResult { get; set; }        
        //public string Remark { get; set; }
  

        [SugarColumn(ColumnName = "id")]
        public string Id { get; set; }

        [SugarColumn(ColumnName = "product_no")]
        public string ProductNo { get; set; }

        [SugarColumn(ColumnName = "detection_time")]
        public DateTime DetectionTime { get; set; }

        [SugarColumn(ColumnName = "status")]
        public string Status { get; set; }

        [SugarColumn(ColumnName = "barcode")]
        public string Barcode { get; set; }

        [SugarColumn(ColumnName = "raw_result")]
        public string RawResult { get; set; }

        [SugarColumn(ColumnName = "remark")]
        public string Remark { get; set; }

        [SugarColumn(ColumnName = "create_man")]
        public string CreateMan { get; set; }

        [SugarColumn(ColumnName = "create_dt")]
        public DateTime CreateDt { get; set; }
    }
}
