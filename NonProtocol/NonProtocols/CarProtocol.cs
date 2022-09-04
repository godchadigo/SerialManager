using NonProtocol.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static NonProtocol.Enums.CarProtocolEunm;

namespace NonProtocol.NonProtocols
{
    public class CarProtocol
    {
        public static CarProtocol Instance = new CarProtocol();
        /// <summary>
        /// 指定讀取狀態是否開啟
        /// </summary>
        /// <param name="status">true為開啟，false為關閉</param>
        /// <returns></returns>
        public byte[] StartReadStatusCommand(bool status , ReadButtonArea area)
        {
            //44 01 00 00 00 00 00 00 00 00 00 00 00 00 00 0D
            string command = String.Empty;
            switch (area)
            {
                case ReadButtonArea.Block1:
                    {
                        if (status)
                            command = "44 01 00 00 00 00 00 00 00 00 00 00 00 00 00 0D";
                        else
                            command = "44 00 00 00 00 00 00 00 00 00 00 00 00 00 00 0D";
                    }
                    break;
                case ReadButtonArea.Block2:
                    {
                        if (status)
                            command = "22 01 00 00 00 00 00 00 00 00 00 00 00 00 00 0D";
                        else
                            command = "22 00 00 00 00 00 00 00 00 00 00 00 00 00 00 0D";
                    }
                    break;
            }

            
            var result = ToolManager.Instance.GetDataByte(command);
            return result.ToArray();
        }
        
    }
}
