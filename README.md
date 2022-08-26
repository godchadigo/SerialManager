# SerialManager
串口管理器

## 初始化設定

``` c#
        public Form1()
        {
            InitializeComponent();
            
            //實例化串口
            Serial machin3 = new Serial("COM10我什麼都沒有~", "COM10,9600,8,N,1");
            //-1為自動重連，目前只有這個選項，之後在新增
            machin3.Reconnect = -1;

            //註冊串口 Com3
            SerialTasker.Instance.RegisterCOM(new Serial("COM3我可愛的Vigor PLC", "COM3,115200,8,N,1"));
            //註冊串口 Com9
            SerialTasker.Instance.RegisterCOM(new Serial("COM9我可愛的JLink", "COM9,19200,8,N,1"));
            //註冊串口 Com10
            //SerialTasker.Instance.RegisterCOM(machin3);
            //綁定接收事件
            SerialTasker.Instance.SerialTaskerReceivedEvent += Instance_SerialTaskerReceivedEvent;
            //註冊的串口全開
            SerialTasker.Instance.OpenAll();

            //可自行過濾選項，打開對應的串口，搭配IsOpen做保護            
            /*
            var machins = SerialTasker.Instance.GetAllMachin();
            foreach (var machin in machins)
            {
                //過濾選項，指定機台打開
                if (machin.Profile.Name == "????????")
                {
                    if (!machin.IsOpen)
                        machin.OpenAsync();
                }                                
            }
            */
        }
```

## 接收事件
- 目前不支援黏包解析，純raw data回傳，若需要可提出，但是要我有空處理
``` c#
        private void Instance_SerialTaskerReceivedEvent(Models.Serials.SerialPortProfileModel spModel, byte[] dataBlock)
        {
            string msg = "";                        
            foreach (var bytes in dataBlock)
            {
                msg += bytes + " ";
            }
            Console.WriteLine(string.Format("標籤: {0} 接收到的訊息: {1}" , spModel.Name , msg));
        }
```

## 廣播串口
- 如果你的設備都是同一種，並且通訊協定也一樣的話，可以使用廣播輪巡隊每個串口發送同樣的指令
``` c#
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
                if (machin.IsOpen)
                {
                    machin.SendAsync(data);
                    Console.WriteLine(String.Format("串口標籤: {0} 串口狀態: {1}", machin.Profile.Name, machin.IsOpen == true ? "打開" : "關閉"));
                }                
            }
        }
```

## 指定機台發送數據
- 針對單一機台發送數據
``` c#
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
```

