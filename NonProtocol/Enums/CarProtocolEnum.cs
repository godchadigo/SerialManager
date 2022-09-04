using NonProtocol.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NonProtocol.Enums
{
    public class CarProtocolEunm
    {
        public enum ReadButtonArea 
        {
            None = 0,   //無組別
            Block1 = 1, //44
            Block2 = 2, //22
        }
        public enum ReadButtonStatus
        {
            None = 0,   //未知狀態(可能是關閉，也可能是離線)
            Start = 1,  //連續讀取中
            Stop = 2,   //停止中
        }
        public enum ReadButtonType
        {
            None = 0,       //鬆開
            Touch = 1,      //觸摸
            Down = 2,       //按下
            WindowUP= 3,    //車窗上升
            WindowDown = 4, //車窗下降
            Error= 999,     //未知的模式
        }
        public enum ReadButtonDoorType
        {
            LeftTop = 1,    //左前
            LeftDown = 2,   //左後
            RightTop = 3,   //右前
            RightDown = 4,  //右後
        }
    }
}
