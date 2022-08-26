using Models.Enum;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;

namespace Models.Serials
{
    public class SerialPortProfileModel
    {
        public string Name { get; set; }
        public int Reconnect { get; set; }
        public SerialPort sp  { get; set; }
        public override string ToString()
        {
            //base.ToString()
            string msg = string.Format("Name: {0} PortName:{1} Baudrate:{2} DataBits:{3} Parity:{4} StopBits:{5}" , 
                Name ,
                sp.PortName ,
                sp.BaudRate ,
                sp.DataBits ,
                sp.Parity ,
                sp.StopBits);            
            return msg;
        }
    }
}
