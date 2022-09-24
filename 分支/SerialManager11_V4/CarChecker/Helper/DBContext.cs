using CarChecker.Models.Sqlite;
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
       //public static string ConnectionString = "Database=doorplank_test;Data Source=127.0.0.1;User Id=root;Password=12345;";
        public static string ConnectionString = "Database=ButtonSql;server=127.0.0.1;port=3307;User Id=root;Password=Asd279604823;";        
        public static string ConnectionSqliteString = "Data Source=c:\\mydb.db;Version=3;";        
        public static string GetCurrentProjectPath
        {
            get
            {
                return Environment.CurrentDirectory.Replace(@"\bin\Debug", "");//获取具体路径
            }
        }
        public static string ConnectionString1 = @"DataSource=" + GetCurrentProjectPath + @"\bin\Debug\Sqlite\Config.sqlite";
        //public static string ConnectionString1 = @"Data Source=D:\1111.db";
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
        
        public static SqlSugarClient GetSqLiteInstance()
        {
            
            SqlSugarClient db = new SqlSugarClient(new ConnectionConfig()
            {
                
                //DbType = DbType.SqlServer,
                DbType = DbType.Sqlite,
                IsAutoCloseConnection = true,
                InitKeyType = InitKeyType.Attribute,
                ConnectionString = ConnectionString1,
            });
            db.DbMaintenance.CreateDatabase();
            if (!db.DbMaintenance.IsAnyTable("Property"))
            {
                db.CodeFirst.InitTables<PropertyModel>();
                SqliteCodeFirst(db);
            }                
            return db;
        }

        private static void SqliteCodeFirst(SqlSugarClient db)
        {            
            List<PropertyModel> data = new List<PropertyModel>();
            data.Add(new PropertyModel() { Id = 1, PropertyName = "车窗锁", PropertyValue = false });
            data.Add(new PropertyModel() { Id = 2, PropertyName = "后备箱開關", PropertyValue = true });
            data.Add(new PropertyModel() { Id = 3, PropertyName = "油箱蓋板開關1", PropertyValue = true });
            data.Add(new PropertyModel() { Id = 4, PropertyName = "油箱蓋板開關2", PropertyValue = true });
            data.Add(new PropertyModel() { Id = 5, PropertyName = "油箱蓋板開關3", PropertyValue = true });
            data.Add(new PropertyModel() { Id = 6, PropertyName = "加熱按壓", PropertyValue = false });
            data.Add(new PropertyModel() { Id = 7, PropertyName = "通風按壓", PropertyValue = true });
            data.Add(new PropertyModel() { Id = 8, PropertyName = "按鍵M按壓", PropertyValue = true });
            data.Add(new PropertyModel() { Id = 9, PropertyName = "按鍵1按壓", PropertyValue = true });
            data.Add(new PropertyModel() { Id = 10, PropertyName = "按鍵2按壓", PropertyValue = true });
            data.Add(new PropertyModel() { Id = 11, PropertyName = "按鍵3按壓", PropertyValue = true });
            data.Add(new PropertyModel() { Id = 12, PropertyName = "左後視鏡按壓", PropertyValue = true });
            data.Add(new PropertyModel() { Id = 13, PropertyName = "右後視鏡按壓", PropertyValue = true });
            data.Add(new PropertyModel() { Id = 14, PropertyName = "主駕駛車窗降下", PropertyValue = true });
            data.Add(new PropertyModel() { Id = 15, PropertyName = "主駕駛車窗升起", PropertyValue = true });
            data.Add(new PropertyModel() { Id = 16, PropertyName = "主駕駛車窗自動", PropertyValue = true });
            data.Add(new PropertyModel() { Id = 17, PropertyName = "副駕駛車窗降下", PropertyValue = true });
            data.Add(new PropertyModel() { Id = 18, PropertyName = "副駕駛車窗升起", PropertyValue = true });
            data.Add(new PropertyModel() { Id = 19, PropertyName = "副駕駛車窗自動", PropertyValue = true });
            data.Add(new PropertyModel() { Id = 20, PropertyName = "後排右車窗降下", PropertyValue = true });
            data.Add(new PropertyModel() { Id = 21, PropertyName = "後排右車窗升起", PropertyValue = true });
            data.Add(new PropertyModel() { Id = 22, PropertyName = "按鍵R按壓", PropertyValue = true });
            data.Add(new PropertyModel() { Id = 23, PropertyName = "後排左車窗降下", PropertyValue = true });
            data.Add(new PropertyModel() { Id = 24, PropertyName = "後排左車窗升起", PropertyValue = true });
            data.Add(new PropertyModel() { Id = 25, PropertyName = "後排左車窗自動", PropertyValue = true });
            data.Add(new PropertyModel() { Id = 26, PropertyName = "後視鏡調節上按壓", PropertyValue = true });
            data.Add(new PropertyModel() { Id = 27, PropertyName = "後視鏡調節下按壓", PropertyValue = true });
            data.Add(new PropertyModel() { Id = 28, PropertyName = "後視鏡調節左按壓", PropertyValue = true });
            data.Add(new PropertyModel() { Id = 29, PropertyName = "後視鏡調節右按壓", PropertyValue = true });
            
            db.Insertable(data).ExecuteCommand();
        }
    }
}
