using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace MouseApplication.MouseHelper
{
    public class MouseImpl
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);
        //Mouse actions
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;

        public void DoMouseClick()
        {
            ////Call the imported function with the cursor's current position
            //uint X = (uint)Cursor.Position.X;
            //uint Y = (uint)Cursor.Position.Y;

            ////mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, X, Y, 0, 0);
            ////move to coordinates
            //Point pt = (Point)pc.ConvertFromString(X + "," + Y);
            //Cursor.Position = pt;

            ////perform click            
            //mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
            //mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        }
    }
}
