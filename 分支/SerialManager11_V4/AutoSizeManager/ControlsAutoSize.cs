using Sunny.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoSizeManager
{
    public class ControlsAutoSize
    {
        public struct controlRect
        {
            public Control Controls;
            public int Left;
            public int Top;
            public int Width;
            public int Height;
            public Font Fonts;
        }
        public struct controlRectRaw
        {
            public int Width;
            public int Height;
        }
        public Control Controls;
        public List<controlRect> AutoSizeList = new List<controlRect>();
        public void controllInitializeSize(Control mForm)
        {                        
            List<Control> AllControls = GetAllControls(mForm);
            Controls = mForm;
            foreach (Control control in AllControls)
            {                
                AutoSizeList.Add(GetModel(control));
            }                    
        }
        public controlRect GetModel(Control con)
        {
            controlRect model = new controlRect();
            model.Left = con.Left;
            model.Top = con.Top;
            model.Width = con.Width;
            model.Height = con.Height;
            model.Controls = con;
            model.Fonts = con.Font;
            return model;
        }

        public void controlAutoSize(Control mForm , int width , int height)
        {

            double wScale = (double)mForm.Width / (double)width;//新舊窗體之間的比例，與最早的舊窗體  
            double hScale = (double)mForm.Height / (double)height;//.Height;  

            mForm.SuspendLayout();
            foreach (controlRect con in AutoSizeList)
            {
                
                //double lScale = (double)mForm.Left / con.Left;

                int ctrLeft0, ctrTop0, ctrWidth0, ctrHeight0;

                ctrLeft0 = con.Left;
                ctrTop0 = con.Top;
                ctrWidth0 = con.Width;
                ctrHeight0 = con.Height;
                if (con.Controls.Dock == DockStyle.None)                
                {
                    con.Controls.Left = (int)(ctrLeft0 * wScale);
                    con.Controls.Top = (int)(ctrTop0 * hScale);
                    con.Controls.Width = (int)(ctrWidth0 * wScale);
                    con.Controls.Height = (int)(ctrHeight0 * hScale);

                    Font f = con.Fonts;
                    //var s = f.Size *  wScale * hScale;
                    var s = f.Size * wScale ;
                    //if (hScale != 1)
                    con.Controls.Font = new Font(f.FontFamily, (float)s , f.Style);
                }
            }
            mForm.ResumeLayout(false);
            
        }

        public static List<Control> GetAllControls(Control form)
        {
            return GetAllControls(ToList(form.Controls));
        }

        public static List<Control> ToList(Control.ControlCollection controls)
        {
            List<Control> controlList = new List<Control>();
            foreach (Control control in controls)
                controlList.Add(control);
            return controlList;
        }

        public static List<Control> GetAllControls(List<Control> inputList)
        {
            //複製inputList到outputList
            List<Control> outputList = new List<Control>(inputList);

            //取出inputList中的容器
            IEnumerable<Control> containers = from control in inputList
                                              where
                                                 control is GroupBox |
                                                 control is TabControl |
                                                 control is Panel |
                                                 control is FlowLayoutPanel |
                                                 control is TableLayoutPanel |
                                                 control is ContainerControl |
                                                 control is UIPanel |
                                                 control is UITableLayoutPanel |
                                                 control is UIButton |
                                                 control is UserControl
                                              select control;

            foreach (Control container in containers)
            {
                //遞迴加入容器內的容器與控制項
                outputList.AddRange(GetAllControls(ToList(container.Controls)));
            }
            return outputList;
        }
    }
}
