using AutoSizeManager;
using CarChecker.Models;
using Models.Serials;
using NonProtocol.Managers;
using NonProtocol.NonProtocols;
using PacketDecoder;
using SerialManager;
using SerialManager.Managers;
using SqlSugarTester.Helper;
using SqlSugarTester.Models;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static NonProtocol.Enums.CarProtocolEunm;

namespace CarChecker
{
    public partial class Form1 : Form
    {
        #region 全局變數
        private Dictionary<string, GridStyle> gridList = new Dictionary<string, GridStyle>();
        private Dictionary<int, GridProperty> GridProfile = new Dictionary<int, GridProperty>();
        private List<ResultDataBindingModel> BindingList = new List<ResultDataBindingModel>();
        #endregion

        #region 建構子
        public Form1()
        {
            InitializeComponent();
            //設定背景風格
            uiTableLayoutPanel1.Style = UIStyle.Black;
            uiTableLayoutPanel2.Style = UIStyle.Black;
            //啟動通訊
            InitCommunication();
            //綁定表格與按鈕ID
            RegisterBindingData();
            //註冊表格
            RegisterGridData();
            //註冊表格身分
            RegisterTableList();
            //表格初始化
            GridInit();

        }
        #endregion

        #region 通訊初始化
        /// <summary>
        /// 初始化通訊設備
        /// </summary>
        public void InitCommunication()
        {
            SerialTasker.Instance.RegisterCOM(new Serial("檢測設備1", "COM3,115200,8,N,1"));
            SerialTasker.Instance.SerialTaskerReceivedEvent += this.Instance_SerialTaskerReceivedEvent;
            SerialTasker.Instance.OpenAll();
        }
        #endregion

        #region 接收事件
        private void Instance_SerialTaskerReceivedEvent(SerialPortProfileModel spModel, byte[] dataBlock)
        {
            string msg = "";
            for (int i = 0; i < dataBlock.Length; i++)
            {
                msg += ToolManager.Instance.DecToHex(dataBlock[i]) + " ";
            }
            //Machine1Decoder
            switch (spModel.Name)
            {
                case "檢測設備1":
                    {
                        Machine1Decoder(spModel, dataBlock);
                        this.BeginInvoke(new Action(() => {
                            packetText_label.Text = "報文內容 : " +  msg;
                        }));
                    }
                    break;
            }
        }
        #endregion

        #region 按鈕事件
        private void StartReadStatus_btn_Click(object sender, EventArgs e)
        {
            if (SerialTasker.Instance.GetMachin("檢測設備1" , out Serial m1))
            {
                if (m1.IsOpen)
                {
                    var result = CarProtocol.Instance.StartReadStatusCommand(true , ReadButtonArea.Block1);
                    m1.Send(result);
                }
                else
                {
                    MessageBox.Show(String.Format("{0}當前處於離線狀態", m1.Profile.Name));
                }
            }
        }
        private void StopReadStatus_btn_Click(object sender, EventArgs e)
        {
            if (SerialTasker.Instance.GetMachin("檢測設備1", out Serial m1))
            {
                if (m1.IsOpen)
                {
                    var result = CarProtocol.Instance.StartReadStatusCommand(false , ReadButtonArea.Block2);
                    m1.Send(result);
                }
                else
                {
                    MessageBox.Show(String.Format("{0}當前處於離線狀態", m1.Profile.Name));
                }
            }
        }

        private void uiButton3_Click(object sender, EventArgs e)
        {
            using(var context = DBContext.GetMsSqlInstance())
            {
                var result = ConvertDataToSql();
                context.Insertable(result).ExecuteCommand();
                AddRecodeLine(result);
                //取消選取
                RawResultAdvance_Grid.ClearSelection();
            }
        }

        #endregion

        #region 點擊事件
        /// <summary>
        /// 利用日期查詢資料
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton4_Click(object sender, EventArgs e)
        {
            using (var context = DBContext.GetMsSqlInstance())
            {
                var result =
                    context.Queryable<ButtonSqlModel>().Where(
                    x =>
                    x.DataTime.Year == uiDatePicker1.Year &&
                    x.DataTime.Month == uiDatePicker1.Month &&
                    x.DataTime.Day == uiDatePicker1.Day
                    ).ToList();
                SetRcodeDateTitle("查詢日期 : " + string.Format("{0}年/{1}月/{2}日", uiDatePicker1.Year, uiDatePicker1.Month, uiDatePicker1.Day));  //寫入查詢Barcode到Title
                ClearRecodeRows();      //清除記錄表格
                ClearRawResultRows();   //清除RawResult表格
                result.ForEach(x => {
                    AddRecodeLine(x);
                });
            }
        }
        /// <summary>
        /// 利用Barcode查詢
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton5_Click(object sender, EventArgs e)
        {
            using (var context = DBContext.GetMsSqlInstance())
            {
                var result =
                    context.Queryable<ButtonSqlModel>().Where(
                    x =>
                    x.Barcode == QuereBarcode_tBox.Text
                    ).ToList();

                SetRcodeDateTitle("查詢Barcode : " + QuereBarcode_tBox.Text);  //寫入查詢Barcode到Title
                ClearRecodeRows();      //清除記錄表格
                ClearRawResultRows();   //清除RawResult表格
                //遍歷資料輸出到表格
                result.ForEach(x => {
                    AddRecodeLine(x);
                });
            }
        }
        /// <summary>
        /// Barcode TextBox 點擊事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Barcode_tBox_Click(object sender, EventArgs e)
        {
            Barcode_tBox.SelectAll();
        }
        /// <summary>
        /// Barcode Query TextBox 點擊事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void QuereBarcode_tBox_Click(object sender, EventArgs e)
        {
            QuereBarcode_tBox.SelectAll();
        }
        #endregion

        #region 檢測設備1
        /// <summary>
        /// 檢測設備1回調事件
        /// </summary>
        /// <param name="spModel"></param>
        /// <param name="dataBlock"></param>
        public void Machine1Decoder(SerialPortProfileModel spModel, byte[] dataBlock)
        {
            //調用解碼器
            var result = MainDecoder.Fixed_ETX_Decoder( dataBlock , "0D" );
            foreach (var item in result)
            {
                Console.WriteLine(item.Message);
                
                if (item.Type != ReadButtonType.None)
                {
                    CheckBinding(Right_Grid, item.ButtonID , item.Type , item.Area);
                }
            }            
        }
        #endregion

        #region 測試用
        private void button1_Click(object sender, EventArgs e)
        {
            //44 01 00 01 00 00 00 00 00 00 00 00 00 00 00 0D
            string fake1 = "44 01 01 00 00 00 00 00 00 00 00 00";
            var data = ToolManager.Instance.GetDataByte(fake1);
            //調用解碼器
            var result = MainDecoder.Fixed_ETX_Decoder(data.ToArray(), "0D");
        }
        public void Test()
        {
            Task.Run(async () => {
                Console.WriteLine("44檢測開始!");
                StartReadStatus_btn.PerformClick();
                await Task.Delay(10000);
                Console.WriteLine("44檢測結束!");
                StopReadStatus_btn.PerformClick();

                Console.WriteLine("22檢測結束!");
                await Task.Delay(10000);
                Console.WriteLine("22檢測結束!");
            });
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // 00 00 00 00 0D
            string fake1 = " 00 00 00 0D 44 01 00 02 00 00 00 00 00 00 00 00 00 00 00 0D";
            var data = ToolManager.Instance.GetDataByte(fake1);
            //調用解碼器
            var result = MainDecoder.Fixed_ETX_Decoder(data.ToArray(), "0D");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //22 01 00 01 00 00 00 00 00 00 00 00 00 00 00 0D

            string fake1 = "22 01 00 01 00 00 00 00 00 00 00 00 00 00 00 0D 22 01 00 00 00";
            var data = ToolManager.Instance.GetDataByte(fake1);
            //調用解碼器
            var result = MainDecoder.Fixed_ETX_Decoder(data.ToArray(), "0D");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //22 01 00 00 00 00 00 00 01 00 00 00 00 00 00 0D

            string fake1 = " 00 00 00 01 00 00 00 00 00 00 0D";
            var data = ToolManager.Instance.GetDataByte(fake1);
            //調用解碼器
            var result = MainDecoder.Fixed_ETX_Decoder(data.ToArray(), "0D");
        }
        public void Test2()
        {
            Task.Run(async() => {
                while (true)
                {
                    CarProtocol.Instance.StartReadStatusCommand(true, ReadButtonArea.Block1);
                    await Task.Delay(10);
                    CarProtocol.Instance.StartReadStatusCommand(true, ReadButtonArea.Block2);
                    await Task.Delay(10);
                }
            }) ;
        }
        #endregion

        #region 窗體載入事件

        private ControlsAutoSize cas = new ControlsAutoSize();
        private void Form1_Load(object sender, EventArgs e)
        {
            cas.controllInitializeSize(this);
            cas.controlAutoSize(this, 1920, 1040);

            ColorCheck(Right_Grid);
            GetTodayRecode();

        }
        #endregion

        #region 窗體大小變更事件
        private void Form1_Resize(object sender, EventArgs e)
        {
            cas.controlAutoSize(this, 1920, 1040);
        }
        #endregion

        #region SQL操作相關
        /// <summary>
        /// 轉換數據成SQL格式
        /// </summary>
        /// <returns></returns>
        public ButtonSqlModel ConvertDataToSql()
        {
            StringBuilder rawData = new StringBuilder();
            bool totalResult = false;
            for (int i = 0; i < Right_Grid.Rows.Count; i++)
            {
                //sqlList.Add (Right_Grid.Rows[])
                var rowID = i;
                int id = int.Parse(Right_Grid.Rows[rowID].Cells[0].Value.ToString());
                string title = Right_Grid.Rows[rowID].Cells[1].Value.ToString();
                string checkResult = Right_Grid.Rows[rowID].Cells[2].Value.ToString();
                if (checkResult == "NG") totalResult = true;
                rawData.Append(string.Format("{0}:{1}:{2},", id, title, checkResult));
                Console.WriteLine(string.Format("RowID:{0} ID:{1} Title:{2} Result:{3}", rowID, id, title, checkResult));
            }
            var model = new ButtonSqlModel();
            model.Uuid = Guid.NewGuid();
            model.DataTime = DateTime.Now;
            model.Status = totalResult == true ? "OK" : "NG";
            model.Barcode = Barcode_tBox.Text;
            model.RawResult = rawData.ToString();
            model.Remark = "None";

            return model;
        }
        /// <summary>
        /// 解析數據庫RawResult數據
        /// </summary>
        private List<RawResultModel> DecodeRawResult(string rawData)
        {
            List<RawResultModel> rawResultModels = new List<RawResultModel>();

            List<string> data = new List<string>();
            string[] areas = rawData.Split(',');
            foreach (string s in areas)
            {
                var sp = s.Split(':');
                if (sp.Length > 2)
                {
                    var id = sp[0];
                    var title = sp[1];
                    var result = sp[2];
                    rawResultModels.Add(new RawResultModel() { ID = id, Title = title, Result = result });
                }
            }
            return rawResultModels;
        }
        #endregion
        
        #region 表格初始化操作相關
        /// <summary>
        /// 註冊按鈕與表格綁定
        /// </summary>
        private void RegisterBindingData()
        {
            BindingList.Add(new ResultDataBindingModel() { BindingGridID = 1, Area = ReadButtonArea.Block1, CheckType = ReadButtonType.Down, BindingButtonID = 1 });
        }
        /// <summary>
        /// 註冊表格項目內容
        /// </summary>
        private void RegisterGridData()
        {
            //Key為RowID
            //Value為GridProperty模型
            GridProfile.Add(0, new GridProperty() { ID = 1, Title = "中控鎖", Result = "NG" });
            GridProfile.Add(1, new GridProperty() { ID = 2, Title = "禮貌燈開關", Result = "NG" });
            GridProfile.Add(2, new GridProperty() { ID = 3, Title = "開門拉手燈條", Result = "NG" });
            GridProfile.Add(3, new GridProperty() { ID = 4, Title = "扶手氛圍燈", Result = "NG" });
            GridProfile.Add(4, new GridProperty() { ID = 5, Title = "裝飾條氣氛燈", Result = "NG" });
            GridProfile.Add(5, new GridProperty() { ID = 6, Title = "油箱蓋板開關", Result = "NG" });
            GridProfile.Add(6, new GridProperty() { ID = 7, Title = "加熱按壓", Result = "NG" });
            GridProfile.Add(7, new GridProperty() { ID = 8, Title = "通風按壓", Result = "NG" });
            GridProfile.Add(8, new GridProperty() { ID = 9, Title = "按鍵1按壓", Result = "NG" });
            GridProfile.Add(9, new GridProperty() { ID = 10, Title = "按鍵2按壓", Result = "NG" });
            GridProfile.Add(10, new GridProperty() { ID = 11, Title = "按鍵3按壓", Result = "NG" });
            GridProfile.Add(11, new GridProperty() { ID = 12, Title = "按鍵4按壓", Result = "NG" });
            GridProfile.Add(12, new GridProperty() { ID = 13, Title = "左後視鏡按壓", Result = "NG" });


            GridProfile.Add(13, new GridProperty() { ID = 14, Title = "右後視鏡按壓", Result = "NG" });
            GridProfile.Add(14, new GridProperty() { ID = 15, Title = "主駕駛車窗降下", Result = "NG" });
            GridProfile.Add(15, new GridProperty() { ID = 16, Title = "主駕駛車窗升起", Result = "NG" });
            GridProfile.Add(16, new GridProperty() { ID = 17, Title = "主駕駛車窗自動", Result = "NG" });

            GridProfile.Add(17, new GridProperty() { ID = 18, Title = "副駕駛車窗降下", Result = "NG" });
            GridProfile.Add(18, new GridProperty() { ID = 19, Title = "副駕駛車窗升起", Result = "NG" });
            GridProfile.Add(19, new GridProperty() { ID = 20, Title = "副駕駛車窗自動", Result = "NG" });

            GridProfile.Add(20, new GridProperty() { ID = 21, Title = "後排右車窗降下", Result = "NG" });
            GridProfile.Add(21, new GridProperty() { ID = 22, Title = "後排右車窗升起", Result = "NG" });
            GridProfile.Add(22, new GridProperty() { ID = 23, Title = "後排右車窗自動", Result = "NG" });

            GridProfile.Add(23, new GridProperty() { ID = 24, Title = "後排左車窗降下", Result = "NG" });
            GridProfile.Add(24, new GridProperty() { ID = 25, Title = "後排左車窗升起", Result = "NG" });
            GridProfile.Add(25, new GridProperty() { ID = 26, Title = "後排左車窗自動", Result = "NG" });

            GridProfile.Add(26, new GridProperty() { ID = 27, Title = "後視鏡調節上按壓", Result = "NG" });
            GridProfile.Add(27, new GridProperty() { ID = 28, Title = "後視鏡調節下按壓", Result = "NG" });
            GridProfile.Add(28, new GridProperty() { ID = 29, Title = "後視鏡調節左按壓", Result = "NG" });
            GridProfile.Add(29, new GridProperty() { ID = 30, Title = "後視鏡調節右按壓", Result = "NG" });

        }
        /// <summary>
        /// 表格列表註冊
        /// </summary>
        private void RegisterTableList()
        {
            gridList.Add("Left", new GridStyle() { Grid = Left_Grid, Style = UIStyle.Black });
            gridList.Add("Right", new GridStyle() { Grid = Right_Grid, Style = UIStyle.DarkBlue });
            gridList.Add("Recode", new GridStyle() { Grid = Recode_Grid, Style = UIStyle.Black });
            gridList.Add("RawResult", new GridStyle() { Grid = RawResultAdvance_Grid, Style = UIStyle.DarkBlue });
        }
        /// <summary>
        /// 所有表格初始化(內碼表格除外)
        /// </summary>
        public void GridInit()
        {
            gridList.ToList().ForEach(grid => {
                if (grid.Key == "Left" || grid.Key == "Right")
                {
                    grid.Value.Grid.DataSource = GetData();
                }
                if (grid.Key == "Recode")
                {
                    //初始化Recode Column
                    InitRecodeColumn();
                }
                if (grid.Key == "RawResult")
                {
                    InitRawResultAdvanceColumns();
                }
                GridStyleInit(grid.Value);
            });

        }
        /// <summary>
        /// 表格風格設置
        /// </summary>
        /// <param name="gs"></param>
        private void GridStyleInit(GridStyle gs)
        {
            var gdv = gs.Grid;
            var style = gs.Style;


            for (var i = 0; i < gdv.Columns.Count; i++)
            {
                //取消自動排列
                gdv.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                //ReadOnly
                gdv.Columns[i].ReadOnly = true;

                //內容置中
                gdv.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            //設置表格風格
            gdv.Style = style;

            gdv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            //文字大小
            gdv.ColumnHeadersDefaultCellStyle.Font = new Font("標楷體", 20, FontStyle.Regular);
            gdv.DefaultCellStyle.Font = new Font("標楷體", 18, FontStyle.Regular);


            //選取一整列
            gdv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            //隱藏前面小三角型
            //gdv.RowHeadersVisible = false;
            //隱藏第一列新增按鈕
            gdv.RowHeadersVisible = false;
            //取消標題列調整
            gdv.AllowUserToResizeColumns = false;
            //取消欄位調整
            gdv.AllowUserToResizeRows = false;
            //取消最後一行
            gdv.AllowUserToAddRows = false;
            //gdv.ColumnHeadersVisible = false;
            //取消多選
            gdv.MultiSelect = false;
            gdv.RowTemplate.Height = 25;
            gdv.ColumnHeadersHeight = 50;

            //gdv.DefaultCellStyle.SelectionBackColor = Color.Transparent;
            //gdv.DefaultCellStyle.SelectionForeColor = Color.Blue;

            //gdv.DefaultCellStyle.SelectionBackColor = Color.Orange;
            gdv.Refresh();

        }
        #endregion

        #region 待測表格相關

        #endregion

        #region Reocde Grid 相關
        /// <summary>
        /// 初始化Recode Grid Columns
        /// </summary>
        private void InitRecodeColumn()
        {
            // Add columns.            
            Recode_Grid.Columns.Add("Uuid", "Uuid");
            Recode_Grid.Columns.Add("紀錄時間", "紀錄時間");
            Recode_Grid.Columns.Add("狀態", "狀態");
            Recode_Grid.Columns.Add("標籤碼", "標籤碼");
            Recode_Grid.Columns.Add("詳細內容", "詳細內容");
            Recode_Grid.Columns.Add("Remark", "Remark");
        }
        /// <summary>
        /// 清除Recode Grid Rows
        /// </summary>
        private void ClearRecodeRows()
        {
            Recode_Grid.Rows.Clear();
        }
        /// <summary>
        /// 新增一行數據到Recode Grid
        /// </summary>
        /// <param name="model"></param>
        private void AddRecodeLine(ButtonSqlModel model)
        {
            Recode_Grid.Rows.Add(model.Uuid, model.DataTime, model.Status, model.Barcode, model.RawResult, model.Remark);

        }

        /// <summary>
        /// 取得今天的紀錄數據
        /// </summary>
        private void GetTodayRecode()
        {
            using (var context = DBContext.GetMsSqlInstance())
            {
                var result =
                context.Queryable<ButtonSqlModel>().Where(
                    x =>
                    x.DataTime.Year == DateTime.Now.Year &&
                    x.DataTime.Month == DateTime.Now.Month &&
                    x.DataTime.Day == DateTime.Now.Day
                    ).ToList();
                result.ForEach(x => {
                    AddRecodeLine(x);
                });
                //設置紀錄表格的時間
                SetRcodeDateTitle("查詢日期 : " + DateTime.Now.ToString("yyyy年 /M月/ d日"));
            }
        }
        #endregion

        #region RawResult Grid 相關

        /// <summary>
        /// 初始化RawResultAdvance Grid Columns
        /// </summary>
        private void InitRawResultAdvanceColumns()
        {
            RawResultAdvance_Grid.Columns.Add("ID", "ID");
            RawResultAdvance_Grid.Columns.Add("Title", "Title");
            RawResultAdvance_Grid.Columns.Add("Result", "Result");
        }
        /// <summary>
        /// 清除RawResult Grid Rows
        /// </summary>
        private void ClearRawResultRows()
        {
            RawResultAdvance_Grid.Rows.Clear();
        }
        #endregion

        #region 取消選取
        private void Right_Grid_Click(object sender, EventArgs e)
        {
            var obj = (UIDataGridView)sender;
            obj.ClearSelection();
        }

        private void Right_Grid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            var obj = (UIDataGridView)sender;
            obj.ClearSelection();
        }

        #endregion

        #region 結構體
        public struct RawResultModel
        {
            public string ID;
            public string Title;
            public string Result;
        }
        #endregion

        #region CellClickEvent
        /// <summary>
        /// Recode Cell點擊事件
        /// 事件觸發後把詳細測試內容輸出到RawResultGrid上
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Recode_Grid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1) return; //表示使用者點擊Column

            var rawData = Recode_Grid.Rows[e.RowIndex].Cells[4].Value.ToString();
            if (rawData.Length > 0)
            {
                var result = DecodeRawResult(rawData);
                RawResultAdvance_Grid.Rows.Clear();
                RawResultAdvance_Grid.SuspendLayout();
                foreach (var row in result)
                {
                    DataGridViewRow cell = new DataGridViewRow();

                    cell.Cells.Add(new DataGridViewTextBoxCell() { Value = row.ID });
                    cell.Cells.Add(new DataGridViewTextBoxCell() { Value = row.Title });
                    cell.Cells.Add(new DataGridViewTextBoxCell() { Value = row.Result });


                    if (row.Result == "NG")
                    {
                        cell.Cells[2].Style.BackColor = Color.Red;
                    }
                    if (row.Result == "OK")
                    {
                        cell.Cells[2].Style.BackColor = Color.Green;
                    }
                    RawResultAdvance_Grid.Rows.Add(cell);
                }
                RawResultAdvance_Grid.ResumeLayout();
            }
        }

        #endregion

        #region 功能性
        /// <summary>
        /// 檢測結果
        /// </summary>
        /// <param name="gdv"></param>
        /// <param name="btnID"></param>
        /// <param name="checkType"></param>
        private void CheckBinding(UIDataGridView gdv, int btnID, ReadButtonType checkType , ReadButtonArea checkArea)
        {
            var result = BindingList.Where(x => x.BindingButtonID == btnID && x.CheckType == checkType && x.Area == checkArea).FirstOrDefault();
            if (result != null)
            {
                var gridRowID = GridProfile.Where(x => x.Value.ID == result.BindingGridID).FirstOrDefault();
                if (gridRowID.Value != null)
                {
                    var RowID = gridRowID.Key;
                    try
                    {

                        gdv.Rows[RowID].Cells[2].Style.BackColor = Color.Green;
                        gdv.Rows[RowID].Cells[2].Value = "OK";

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("找不到Row!!  " + ex.Message);
                    }
                }
                else
                {
                    MessageBox.Show("找不到對應ID");
                }
            }
        }

        /// <summary>
        /// 初始化待測試內容列表
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private DataTable GetData()
        {
            DataTable table = new DataTable();

            // Add columns.
            table.Columns.Add("序號", typeof(string));
            table.Columns.Add("待測內容", typeof(string));
            table.Columns.Add("結果 ", typeof(string));

            foreach (var x in GridProfile)
            {
                var model = x.Value;
                table.Rows.Add(model.ID, model.Title, model.Result);
            }
            return table;
        }

        /// <summary>
        /// 設置右上較查詢的標題
        /// </summary>
        /// <param name="str"></param>
        private void SetRcodeDateTitle(string str)
        {
            QueryDate_label.Text = str;
        }

        /// <summary>
        /// 檢測OK NG 顯示顏色
        /// </summary>
        /// <param name="gdv"></param>
        private void ColorCheck(UIDataGridView gdv)
        {
            for (int i = 0; i < gdv.Rows.Count; i++)
            {
                string value = gdv.Rows[i].Cells[2].Value.ToString();
                if (value == "NG")
                {
                    gdv.Rows[i].Cells[2].Style.BackColor = Color.Red;
                    //gdv.Rows[i].Cells[2].Value = "GG";
                }
                if (value == "OK")
                {
                    gdv.Rows[i].Cells[2].Style.BackColor = Color.Green;
                }
            }
            gdv.ClearSelection();
        }

        #endregion

    }
}
