using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarChecker.Models.Sqlite
{
    [SqlSugar.SugarTable("Property")]
    public class PropertyModel
    {
        [SugarColumn(ColumnName = "ID")]
        public int Id { get; set; }
        [SugarColumn(ColumnName = "PropertyName")]
        public string PropertyName { get; set; }
        [SugarColumn(ColumnName = "PropertyValue")]
        public bool PropertyValue { get; set; }
    }
}
