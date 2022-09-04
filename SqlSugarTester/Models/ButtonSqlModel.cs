using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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
        public string Name { get; set; }
    }
}
