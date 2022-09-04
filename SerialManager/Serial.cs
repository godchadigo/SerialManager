using Models.Serials;
using SerialManager.Managers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace SerialManager
{    
    public class Serial
    {
        
        public const string DefaultProfile = "COM100,19200,8,N,1";        
        public SerialPortProfileModel Profile { get; set; }
        public bool IsOpen { get; set; } = false;
        public bool BeforeDisconnected { get; set; } = false;
        public bool IsFirstConnect { get; set; } = true;
        public int Reconnect { get; set; }
        public delegate void RecevieEventHandler(SerialPortProfileModel spModel , object sender , SerialDataReceivedEventArgs e);
        public event RecevieEventHandler ReceiveEvent;
        //封包隊列
        //public static ConcurrentQueue<byte[]> QueuePacketList = new ConcurrentQueue<byte[]>();
        public static ConcurrentBag<byte[]> QueuePacketList = new ConcurrentBag<byte[]>();
        public static ConcurrentBag<byte[]> FailedQueuePacketList = new ConcurrentBag<byte[]>();


        public Serial(string name , string profile = DefaultProfile , int reconnect = 0)
        {
            SerialPort sp = GetSerial(profile);
            Profile = new SerialPortProfileModel();
            Profile.Name = name;
            Profile.Reconnect = reconnect;
            Profile.sp = sp;
            sp.DataReceived += Sp_DataReceived;
            
        }

        private void Sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {

            SerialTasker.Instance.CallBackHandler(Profile , e );
        }

        ~Serial()
        {

        }
        public SerialPort GetSerial(string profile)
        {
            string[] profileSp = profile.Split(',');

            Parity parity = new Parity();
            StopBits stopBits = new StopBits();
            switch (profileSp[3])
            {
                case "N":
                case "n":
                    parity = Parity.None;
                    break;

            }

            switch (profileSp[4])
            {
                case "1":
                    stopBits = StopBits.One;
                    break;
                case "2":
                    stopBits &= ~StopBits.Two;
                    break;
            }
            SerialPort sp = new SerialPort();
            sp.PortName = profileSp[0].ToString();
            sp.BaudRate = int.Parse(profileSp[1].ToString());
            sp.DataBits = int.Parse(profileSp[2].ToString());
            sp.Parity = parity;
            sp.StopBits = stopBits;
            return sp;
        }
        public bool Open()
        {            
            try
            {                           
                Profile.sp.Open();
                //IsOpen = true;
                return true;
            }
            catch (Exception ex)
            {
                //throw new Exception("串口錯誤");
                if (Reconnect == -1)
                {

                }
                //IsOpen = false;
                return false;
            }                        
        }

        public void OpenAsync()
        {
            ReconnectCommandChecker();
            ReconnectChecker();
        }
        public void ReconnectChecker() 
        {
            
            Thread th = new Thread(() => {
                while (true)
                {
                    if (!IsOpen)
                    {
                        var result = Open();
                        IsFirstConnect = false;
                        CommandScaner();                        
                    }                        
                    Thread.Sleep(200);
                }                
            });

            th.IsBackground = true;
            th.Start();
        }
        public void ReconnectCommandChecker() 
        {
            Thread th = new Thread(() => { 
                while (true)
                {
                    try
                    {                                                
                        IsOpen = Profile.sp.IsOpen;
                        if (!IsOpen && !IsFirstConnect) BeforeDisconnected = true;
                        Thread.Sleep(1);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("串口離線" + ex);  
                    }                    
                }
            });
            th.IsBackground = true;
            th.Start();
        }

        public bool Close()
        {
            try
            {
                Profile.sp.Close();
                //IsOpen = false;
                return true;
            }
            catch (Exception ex)
            {
                //throw new Exception("串口錯誤");
                //IsOpen = false;
                return false;
            }
        }
        public void SendAsync(byte[] data)
        {
            Thread th = new Thread(() => {
                if (IsOpen)
                {
                    Profile.sp.Write(data, 0, data.Length);                 
                }                
            });
            th.IsBackground=true;
            th.Start();
        }
        public void SendAsync(byte[] data , int offset)
        {
            Thread th = new Thread(() => {
                if (IsOpen)
                {
                    Profile.sp.Write(data, offset, data.Length);
                }
            });
            th.IsBackground = true;
            th.Start();
        }

        private bool IsScannerOpen = false;
        private byte[] buffer;
        public void CommandScaner()
        {
            if (IsScannerOpen) return;
            Task.Run(async() => {
                while (true)
                {
                    //斷線續連
                    if (FailedQueuePacketList.Count > 0)
                    {
                        foreach (byte[] item in FailedQueuePacketList)
                        {
                            if (IsOpen)
                            {
                                FailedQueuePacketList.TryTake(out buffer);
                                SendAsync(buffer.ToList());
                                await Task.Delay(1);
                            }
                        }
                    }

                    if (QueuePacketList.Count > 0)
                    {
                        foreach (byte[] item in QueuePacketList)
                        {
                            if (IsOpen)
                            {
                                QueuePacketList.TryTake(out buffer);
                                SendAsync(buffer.ToList());                                
                                await Task.Delay(1);
                            }                                                     
                        }
                    }
                    await Task.Delay(1);
                    IsScannerOpen = true;
                }                
            });
        }
        public void AddQueuePacket(List<byte> data)
        {
            //QueuePacketList.Enqueue(data.ToArray());
            QueuePacketList.Add(data.ToArray());
        }
        public int count = 0;
        /// <summary>
        /// 主要測試
        /// </summary>
        /// <param name="data"></param>
        public void SendAsync(List<byte> data)
        {
            
            //AddQueuePacket(data);
            Task.Run(() => {                                
                try
                {                    
                    Profile.sp.Write(data.ToArray(), 0, data.Count);
                    string msg = "";
                    data.ForEach(x => {
                        msg += x + " ";                            
                    });
                    Console.WriteLine(Profile.sp.PortName + "==傳送了封包==> " + msg + "  " + count);
                    count++;                                                                    
                }
                catch(Exception ex)
                {
                    //IsOpen = false;
                    Console.WriteLine("發送錯誤" + ex);
                    FailedQueuePacketList.Add(buffer.ToArray());
                }
                
            });
            
        }
        public void SendAsync(List<byte> data , int offset)
        {
            Thread th = new Thread(() => {
                if (IsOpen)
                {
                    Profile.sp.Write(data.ToArray(), offset, data.Count);
                    string msg = "";
                    data.ForEach(x => {
                        msg += x + " ";
                    });
                    //Console.WriteLine(Profile.sp.PortName + "==傳送了封包==> " + msg);
                }
            });
            th.IsBackground = true;
            th.Start();
        }
        public bool Send(byte[] data)
        {
            if (IsOpen)
            {
                Profile.sp.Write(data, 0, data.Length);
                return true;
            }
            return false;
        }
        public bool Send(byte[] data , int offset)
        {
            if (IsOpen)
            {
                Profile.sp.Write(data, offset, data.Length);
                return true;
            }
            return false;
        }
        public bool Send(List<byte> data)
        {
            if (IsOpen)
            {
                Profile.sp.Write(data.ToArray(), 0, data.Count);
                return true;
            }
            return false;
        }
        public bool Send(List<byte> data , int offset)
        {
            if (IsOpen)
            {
                Profile.sp.Write(data.ToArray(), offset, data.Count);
                return true;
            }
            return false;            
        }        
    }
}
