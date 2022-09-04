using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NonProtocol.Managers
{
    public class ToolManager
    {
        public static ToolManager Instance = new ToolManager();

        /// <summary>
        /// Hex To Dec
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public int HexToDec(string value)
        {
            var result = int.Parse(value, System.Globalization.NumberStyles.HexNumber);
            return result;
        }
        /// <summary>
        /// Hex To Bytes
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public byte HexToByte(string hex)
        {
            int value = Convert.ToInt32(hex, 16);
            return (byte)value;
        }
        /// <summary>
        /// Dec To Hex
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string DecToHex(int value)
        {
            var result = Convert.ToString(value, 16);
            return result;
        }
        /// <summary>
        /// 資料串解碼
        /// 解空格字串
        /// </summary>
        /// <param name="rawDataStr"></param>
        /// <returns></returns>
        public List<byte> GetDataByte(string rawDataStr)
        {
            List<byte> dataByte = new List<byte>();
            var data = Regex.Split(rawDataStr, @"\s");
            foreach (var str in data)
            {
                if (str.Length != 0)
                    dataByte.Add(HexToByte(str));
            }
            return dataByte;
        }
        public bool BitStatus(int value)
        {
            return value == 1 ? true : false;
        }
    }
}
