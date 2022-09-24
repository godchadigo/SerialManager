using CarChecker.Models.Sqlite;
using SqlSugarTester.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarChecker.Managers
{
    public class PropertysManager
    {
        public static PropertysManager Instance = new PropertysManager();

        public List<PropertyModel> Propertys_List = new List<PropertyModel>();

        public void Init()
        {
            using (var context = DBContext.GetSqLiteInstance())
            {
                var propertys = context.Queryable<PropertyModel>().ToList();
                Propertys_List.AddRange(propertys);
            }
        }
    }
}
