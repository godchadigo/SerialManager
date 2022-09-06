using SqlSugar;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlSugarTester.Helper
{
    public class DBContext
    {
        //public static string ConnectionString = ConfigurationManager.AppSettings["SugarConnectString"];
        //    public static string ConnectionString = "Server=127.0.0.1;Database=Sugar;User Id=sa;Password=Asd279604823";
       public static string ConnectionString = "Database=doorplank_test;Data Source=127.0.0.1;User Id=root;Password=12345;";
        //public static string ConnectionString = "Database=ButtonSql;server=127.0.0.1;port=3307;User Id=root;Password=Asd279604823;";
        public static SqlSugarClient GetMsSqlInstance()
        {
            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = ConnectionString,
                //DbType = DbType.SqlServer,
                DbType = DbType.MySql,
                IsAutoCloseConnection = true
            });
            db.Ado.IsEnableLogEvent = true;
            return db;
        }
    }
}
