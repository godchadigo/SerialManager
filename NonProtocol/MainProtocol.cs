using System;
using System.Collections.Generic;
using System.Linq;

namespace NonProtocol
{
    public class MainProtocol
    {
        private List<byte> STX = new List<byte>() { 165 };
        private List<byte> ETX = new List<byte>() { 90 };
        private List<byte> STATION = new List<byte>() { 1 };
        public void GeneratorPack(List<byte> data)
        {
            List<byte> pack = new List<byte>();
            /*
            data.Add(01);
            data.Add(01);
            data.Add(244);
            data.Add(00);
            data.Add(100);

            data.Add(00);
            data.Add(12);
            data.Add(00);
            data.Add(01);
            data.Add(03);
            */
            var crc = CRC16(STATION.Concat(data).ToArray());
            pack.AddRange(STX);            
            pack.AddRange(STATION);
            pack.AddRange(data);
            pack.AddRange(crc);
            pack.AddRange(ETX);
            
            Console.WriteLine(pack);
            string msg = "";
            pack.ForEach(x => { 
                msg += DecToHex(x) + " ";
            });
            Console.WriteLine(msg);
            
        }
        public void SetValue(short v1, short v2 , short v3 , short v4)
        {
            List<byte> data = new List<byte>();            
            data.AddRange(ShortToBytes(v1 , true));
            data.AddRange(ShortToBytes(v2 , true));
            data.AddRange(ShortToBytes(v3 , true));
            data.AddRange(ShortToBytes(v4 , true));
            
            GeneratorPack(data);
        }
        public List<short> DecodeValue(byte[] data)
        {
            List<short> result = new List<short>();
            byte[] newData = new byte[data.Length - 5];
            var count = 0;
            List<byte> temp = new List<byte>();
            for (int i = 0; i < data.Length - 5; i++)
            {                
                newData[i] = data[i+2];                
            }
            for (int i = 0;i < newData.Length; i++)
            {
                temp.Add(newData[i]);
                ++count;
                if (count >= 2)
                {
                    result.Add((short)((temp[0] * 256) + temp[1]));
                    temp.Clear();
                    count = 0;
                }                
            }

            return result;
        }
        public List<byte> CRC16(byte[] pDataBytes)
        {
            List<byte> result = new List<byte>();
            ushort crc = 0xffff;
            ushort polynom = 0xA001;

            for (int i = 0; i < pDataBytes.Length; i++)
            {
                crc ^= pDataBytes[i];
                for (int j = 0; j < 8; j++)
                {
                    if ((crc & 0x01) == 0x01)
                    {
                        crc >>= 1;
                        crc ^= polynom;
                    }
                    else
                    {
                        crc >>= 1;
                    }
                }
            }
            result.AddRange(ShortToBytes((short)crc));
            return result;
        }
        /// <summary>
        /// Single Short To Bytes
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public List<byte> ShortToBytes(short value , bool isRev = false)
        {
            List<byte> numberBytes = new List<byte>();        
            if (isRev)
                numberBytes = BitConverter.GetBytes(value).Reverse().ToList();
            else
                numberBytes = BitConverter.GetBytes(value).ToList();
            return numberBytes;
        }
        public string DecToHex(int value)
        {
            var result = Convert.ToString(value, 16);
            return result;
        }
    }
}
