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
            var a = DBContext.GetMsSqlInstance();
            a.Insertable<ButtonSqlModel>(new ButtonSqlModel() { Uuid = Guid.NewGuid() , Name = "Chadigo"}).ExecuteCommand();
            
        }
    }
}
