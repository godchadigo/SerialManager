using Models.Serials;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerialManager.Managers
{
    public class SerialTasker
    {
        private static SerialTasker _Instance = new SerialTasker();
        public static SerialTasker Instance = _Instance;

        public static ConcurrentQueue<Serial> MachinList = new ConcurrentQueue<Serial>();
        public delegate void SerialTaskerDelegate(SerialPortProfileModel spModel , byte[] dataBlock);
        public event SerialTaskerDelegate SerialTaskerReceivedEvent;
        public bool RegisterCOM(Serial serial)
        {
            Console.WriteLine(serial.Profile.ToString());             
            if (MachinList.Contains(serial)) return false;
            MachinList.Enqueue(serial);            
            return true;
        }
        public ConcurrentQueue<Serial> GetAllMachin()
        {
            return MachinList;
        }
        public bool GetMachin(string name , out Serial serial)
        {            
            foreach (var machin in MachinList)
            {
                if (machin.Profile.Name == name)
                {                    
                    serial = machin;
                    return true;
                }
            }
            serial = null;
            return false;
        }
        public void OpenAll()
        {
            foreach (var machin in MachinList)
            {
                machin.OpenAsync();
            }
        }
        public bool Open(Serial s)
        {
            return s.Open();
        }
        public void OpenAsync(Serial s)
        {
            s.OpenAsync();
        }
        public void CallBackHandler(SerialPortProfileModel spModel , SerialDataReceivedEventArgs e)
        {
            //資料處理
            Byte[] buffer = new Byte[1024];
            Int32 length = spModel.sp.Read(buffer, 0, buffer.Length);
            Array.Resize(ref buffer, length);
            //回調事件
            SerialTaskerReceivedEvent?.Invoke(spModel, buffer);
        }
    }
}
