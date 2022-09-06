using NonProtocol.Managers;
using NonProtocol.Models;
using System;
using System.Collections.Generic;
using static NonProtocol.Enums.CarProtocolEunm;

namespace PacketDecoder
{
    public class MainDecoder
    {
        public MainDecoder()
        {

        }
        private static List<byte> Fixed_ETX_Buffer = new List<byte>();
        private static List<byte> Fixed_STX_ETX_Buffer = new List<byte>();
        /// <summary>
        /// 固定包尾解碼器
        /// </summary>
        /// <param name="source"></param>
        /// <param name="etx"></param>
        /// <returns></returns>
        public static List<CarModel> Fixed_ETX_Decoder(byte[] source , string etx)
        {
            List<CarModel> modelList = new List<CarModel>();    
            var asciiETX = ToolManager.Instance.HexToDec(etx);
            
            for (int i = 0; i < source.Length; i++)
            {                
                Fixed_ETX_Buffer.Add(source[i]);
                if (source[i] == asciiETX)
                {                    
                    var btnID = DecoderPushButtonID(Fixed_ETX_Buffer.ToArray());
                    modelList.Add(new CarModel() 
                        {
                            Area = btnID.ButtonArea,
                            Status = btnID.Status ,
                            ButtonID = btnID.ButtonID ,
                            Type = btnID.ButtonType ,
                            TS = DateTimeOffset.Now.ToUnixTimeSeconds(),
                            RawData = Fixed_ETX_Buffer.ToArray(),
                            Message = btnID.Message,
                        }
                    );
                    Fixed_ETX_Buffer.Clear();
                }
            }            
            return modelList;
        }
        #region 測試階段
        /// <summary>
        /// 單純解析固定包尾，並回傳解析完成的完整封包
        /// </summary>
        /// <param name="source"></param>
        /// <param name="etx"></param>
        /// <returns></returns>
        public  List<byte[]> Fixed_ETX_Decoder2(byte[] source, string etx)
        {            
            var asciiETX = ToolManager.Instance.HexToDec(etx);
            var result = new List<byte[]>();
            for (int i = 0; i < source.Length; i++)
            {
                Fixed_ETX_Buffer.Add(source[i]);
                if (source[i] == asciiETX)
                {
                    //var result = Decoder_Fixed_ETX_Byte(Fixed_ETX_Buffer);
                    //if (result.Count > 0) modelList.AddRange(result);
                    // result = Fixed_ETX_Buffer;
                    result.Add(Fixed_ETX_Buffer.ToArray());
                    Fixed_ETX_Buffer.Clear();
                }
            }
            
            return result;
            //return modelList;
        }
        /// <summary>
        /// 一般按鈕解碼器
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static List<CarModel> Decoder_Fixed_ETX_Byte(List<byte> data)
        {
            List<CarModel> modelList = new List<CarModel>();            
            var btnID = DecoderPushButtonID(data.ToArray());
            modelList.Add(new CarModel()
            {
                Area = btnID.ButtonArea,
                Status = btnID.Status,
                ButtonID = btnID.ButtonID,
                Type = btnID.ButtonType,
                TS = DateTimeOffset.Now.ToUnixTimeSeconds(),
                RawData = data.ToArray(),
                Message = btnID.Message,
            }
            );
            return modelList;
        }

        #endregion
        /// <summary>
        /// 固定 包頭 包尾 解碼器
        /// </summary>
        /// <param name="source"></param>
        /// <param name="stx"></param>
        /// <param name="etx"></param>
        public static void Fixed_STX_ETX_Decoder(byte[] source , byte[] stx , byte[] etx)
        {

        }
        /// <summary>
        /// 按鈕狀態實體
        /// </summary>
        public struct ButtonProfile
        {
            public int ButtonID;
            public ReadButtonArea ButtonArea;
            public ReadButtonStatus Status;
            public ReadButtonType ButtonType;
            public string Message;
        }
        /// <summary>
        /// 解碼按鈕ID
        /// </summary>
        /// <param name="data"></param>
        /// <returns>999    代表封包格式異常</returns>
        /// <returns>998    代表找不到對應的按鍵</returns>
        /// <returns>1~13   代表對應的按鍵</returns>
        public static ButtonProfile DecoderPushButtonID(byte[] data)
        {
        
            ButtonProfile bp;
            if (data.Length < 16)
            {
                bp.ButtonArea = ReadButtonArea.None;
                bp.ButtonType = ReadButtonType.Error;
                bp.ButtonID = 999;
                bp.Status = ReadButtonStatus.None;
                bp.Message = "封包長度錯誤!";
                return bp;
            }
            //檢測是哪一個按鍵
            for (int i=1;i<=13;i++)
            {
                if (data[1 + i] > 0)
                {                    
                    byte block1 = (byte)(ToolManager.Instance.HexToDec("44"));
                    byte block2 = (byte)(ToolManager.Instance.HexToDec("22"));
                    //扩展
                    byte block4= (byte)(ToolManager.Instance.HexToDec("99"));
                    ReadButtonArea area = ReadButtonArea.None;

                    //解析該封包屬於哪個區域
                    if (data[0] == block1) area = ReadButtonArea.Block1;
                    if (data[0] == block2) area = ReadButtonArea.Block2;
                    //扩展
                    if (data[0] == block4) area = ReadButtonArea.Block4;

                    var type = DecoderButtonType(data[1 + i] , i , area);
                    bp.ButtonArea = area;
                    bp.ButtonType = type;
                    bp.ButtonID = i;
                    bp.Status = data[1] == 1 ? ReadButtonStatus.Start : ReadButtonStatus.Stop;
                    bp.Message = string.Format("按鍵區域{0}，按鍵編號{1}已被觸發，觸發模式為{2}" , area , i , type);
                    return bp;
                }
            }
            
            bp.ButtonArea= ReadButtonArea.None;
            bp.ButtonType = ReadButtonType.Error;
            bp.ButtonID = 998;
            bp.Status = ReadButtonStatus.None;
            bp.Message = "未知的按鍵";
            return bp;            
        }
        
        /// <summary>
        /// 檢測按鈕按下的模式
        /// 註:btnID 3 4 5 6為車窗按紐，對應的Type不同
        /// </summary>
        /// <param name="tID">TypeID</param>
        /// <param name="btnID">ButtonID</param>
        /// <returns></returns>
        public static ReadButtonType DecoderButtonType(byte tID , int btnID , ReadButtonArea area)
        {
            switch (tID)
            {
                case 0:
                    {
                        return ReadButtonType.None;
                    }                   
                case 1:
                    {
                        if (area == ReadButtonArea.Block1)
                        {
                            if (btnID == 3 || btnID == 4 || btnID == 5 || btnID == 6)
                                return ReadButtonType.WindowUP;
                            if (btnID == 1 || btnID == 2)
                                return ReadButtonType.Down;
                            return ReadButtonType.Touch;
                        }
                        if (area == ReadButtonArea.Block4) {

                            return ReadButtonType.Down;
                        }
                       return ReadButtonType.Touch;
                    }                    
                case 2:
                    {
                        if (area == ReadButtonArea.Block1)
                        {
                            if (btnID == 3 || btnID == 4 || btnID == 5 || btnID == 6)
                                return ReadButtonType.WindowDown;
                            return ReadButtonType.Down;
                        }
                        return ReadButtonType.Down;
                    }                    
            }
            return ReadButtonType.Error;
        }
    }
}
