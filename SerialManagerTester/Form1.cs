using SerialManager;
using SerialManager.Managers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SerialManagerTester
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            
            
            Serial machin3 = new Serial("COM10我什麼都沒有~", "COM10,9600,8,N,1");
            machin3.Reconnect = -1;//自動重連

            SerialTasker.Instance.RegisterCOM(new Serial("COM3我可愛的Vigor PLC", "COM3,115200,8,N,1"));
            SerialTasker.Instance.RegisterCOM(new Serial("COM9我可愛的JLink", "COM9,19200,8,N,1"));
            //SerialTasker.Instance.RegisterCOM(machin3);
            SerialTasker.Instance.SerialTaskerReceivedEvent += Instance_SerialTaskerReceivedEvent;
            //全開
            SerialTasker.Instance.OpenAll();
            /*
            var machins = SerialTasker.Instance.GetAllMachin();
            foreach (var machin in machins)
            {
                //過濾選項，指定機台打開
                if (machin.Profile.Name == "????????")
                {
                    machin.OpenAsync();
                }                                
            }
            */
        }

        private void Instance_SerialTaskerReceivedEvent(Models.Serials.SerialPortProfileModel spModel, byte[] dataBlock)
        {
            string msg = "";                        
            foreach (var bytes in dataBlock)
            {
                msg += bytes + " ";
            }
            Console.WriteLine(string.Format("標籤: {0} 接收到的訊息: {1}" , spModel.Name , msg));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //10 02 00 0A 00 13 00 00 01 02 03 04 05 06 08 10 03 33 41

            List<byte> data = new List<byte>();
            data.Add(16);
            data.Add(02);
            data.Add(00);
            data.Add(10);
            data.Add(00);
            data.Add(13);
            data.Add(00);
            data.Add(00);
            data.Add(01);
            data.Add(02);
            data.Add(03);
            data.Add(04);
            data.Add(05);
            data.Add(06);
            data.Add(08);
            data.Add(16);
            data.Add(03);
            data.Add(33);
            data.Add(41);

            //廣播
            var machins = SerialTasker.Instance.GetAllMachin();
            foreach (var machin in machins)
            {
                machin.SendAsync(data);
                Console.WriteLine(String.Format("串口標籤: {0} 串口狀態: {1}", machin.Profile.Name, machin.IsOpen == true ? "打開" : "關閉"));
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            List<byte> data = new List<byte>();
            data.Add(16);
            data.Add(02);
            data.Add(00);
            data.Add(10);
            data.Add(00);
            data.Add(13);
            data.Add(00);
            data.Add(00);
            data.Add(01);
            data.Add(02);
            data.Add(03);
            data.Add(04);
            data.Add(05);
            data.Add(06);
            data.Add(08);
            data.Add(16);
            data.Add(03);
            data.Add(33);
            data.Add(41);

            //指定機台
            if (SerialTasker.Instance.GetMachin("COM3我可愛的Vigor PLC" , out Serial serial))
            {
                Task.Run(async () => {
                    for (int i = 0; i < 100; i++)
                    {
                        if (serial.IsOpen)
                        {
                            serial.SendAsync(data);
                        }                        
                        await Task.Delay(1);
                    }
                });                                
            }
        }
    }
}
