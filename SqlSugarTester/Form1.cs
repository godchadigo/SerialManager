using SqlSugarTester.Helper;
using SqlSugarTester.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SqlSugarTester
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            //var a = DBContext.GetMsSqlInstance();
            //a.Insertable<ButtonSqlModel>(new ButtonSqlModel() { Uuid = Guid.NewGuid() , Name = "Chadigo"}).ExecuteCommand();
            Authrization();
        }
        private bool Authrization()
        {
            // 授权示例 Authorization example
            if (!HslCommunication.Authorization.SetAuthorizationCode("ed1415f8-e06a-43ad-95f7-c04f7ae93b41"))
            {
                Console.WriteLine("Authorization failed! The current program can only be used for 8 hours!");
                return false;   // 激活失败应该退出系统
            }
            else
            {
                Console.WriteLine("授權成功!");
                return true;
            }
        }

    }
}
