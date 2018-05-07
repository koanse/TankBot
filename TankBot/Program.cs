using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;

namespace TankBot
{
    
    static class Program
    {
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);
        [STAThread]
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll")]
        static extern int PostMessage(IntPtr hWnd, int msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        static extern int PostThreadMessage(int id, int msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

        const int WM_KEYDOWN = 0x100;
        const int WM_KEYUP = 0x101;
        const int WM_CHAR = 0x105;
        const int WM_SYSKEYDOWN = 0x104;
        const int WM_SYSKEYUP = 0x105;
        public static void SendKey(Keys key, int repeat)
        {
            IntPtr hWnd = GetForegroundWindow();
            int pid;
            GetWindowThreadProcessId(hWnd, out pid);
            using (Process p = Process.GetProcessById(pid))
            {
                if (p.ProcessName != "opera" && p.ProcessName != "iexplore")
                {
                    return;
                }
                for (int i = 0; i < repeat;i++ )
            {
                //try
                {
                    //PostThreadMessage(p.Threads[i].Id, WM_KEYDOWN, Convert.ToInt32(key), 0);                    
                    
                    PostMessage(hWnd, WM_KEYDOWN, Convert.ToInt32(key), 0x40000001);
                    PostMessage(hWnd, WM_KEYUP, Convert.ToInt32(key), 0x40000001);
                }
                //catch { }
            }
            }
            
        }
        public static void SendSysKey(string wName, Keys key)
        {
            IntPtr hWnd = FindWindow(null, wName); 
            PostMessage(hWnd, WM_SYSKEYDOWN, Convert.ToInt32(key), 0);
            PostMessage(hWnd, WM_SYSKEYUP, Convert.ToInt32(key), 0);
        }
        public static void SendChar(string wName, char c)
        {
            IntPtr hWnd = FindWindow(null, wName); 
            PostMessage(hWnd, WM_CHAR, (int)c, 0);
        }
        
        static void Main()
        {
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
           Bitmap bmpOld=null;

           int iter = 0,lenCount=0;
            Keys kDir=Keys.Up;
            bool isDirKeyDown = false, isSpaceDown = false ;
            double coeff = 0.5, lenSum=0,lenSum2=0;
            Keys key1=Keys.Left, key2=Keys.Z;
            while (true)
           {
                Thread.Sleep(300);
                if (iter / coeff < 5)
                    KeyUp(Keys.Space);
                else
                    KeyDown(Keys.Space);
                if(iter/coeff<30)
               {
                   KeyDown(Keys.Up);
                   KeyUp(Keys.Down);
               }
               else if (iter/coeff < 93)
               {

                       KeyUp(Keys.Up);

                   Screen currentScreen = Screen.PrimaryScreen;
                   Bitmap bmpScreenShot = new Bitmap(currentScreen.Bounds.Width,
                                             currentScreen.Bounds.Height,
                                             PixelFormat.Format32bppArgb);
                   Graphics gScreenShot = Graphics.FromImage(bmpScreenShot);
                   gScreenShot.CopyFromScreen(currentScreen.Bounds.X,
                                              currentScreen.Bounds.Y,
                                              0, 0,
                                              currentScreen.Bounds.Size,
                                              CopyPixelOperation.SourceCopy);
                   if (bmpOld == null)
                   {
                       bmpOld = bmpScreenShot;
                       continue;
                   }
                   long sumLeft = 0, sumRight = 0;
                   for (int i = 0; i < bmpOld.Width; i += 20)
                   {

                       for (int j = 0; j < bmpOld.Height; j += 20)
                       {

                           if (i < bmpOld.Width / 2)
                               sumLeft +=
                                   Math.Abs(bmpOld.GetPixel(i, j).R - bmpScreenShot.GetPixel(i, j).R)
                                   +
                               Math.Abs(bmpOld.GetPixel(i, j).G - bmpScreenShot.GetPixel(i, j).G)
                               +
                               Math.Abs(bmpOld.GetPixel(i, j).B - bmpScreenShot.GetPixel(i, j).B)
                               ;
                           else
                               sumRight +=
                                   Math.Abs(bmpOld.GetPixel(i, j).R - bmpScreenShot.GetPixel(i, j).R)
                                   +
                                   Math.Abs(bmpOld.GetPixel(i, j).G - bmpScreenShot.GetPixel(i, j).G)
                                   +
                               Math.Abs(bmpOld.GetPixel(i, j).B - bmpScreenShot.GetPixel(i, j).B)
                               ;
                       }
                   }
                   KeyUp(key1);
                   KeyUp(key2);
                   if (sumRight != 0 && sumLeft != 0 && sumLeft / sumRight <= 5 && sumRight / sumLeft <= 5)
                   {
                       double len;
                       if (sumLeft > sumRight)
                       {
                           len = sumLeft / sumRight;
                           key1 = Keys.Left;
                           key2 = Keys.Z;
                       }
                       else
                       {
                           len = sumRight / sumLeft;
                           key1 = Keys.Right;
                           key2 = Keys.X;
                       }
                       KeyDown(Keys.Up);
                       KeyDown(key2);
                       KeyDown(key1);
                       len = Math.Min(len, 20);
                       lenSum += len;
                       lenSum2 += len * len;
                       lenCount++;
                       double lenAv = lenSum / lenCount,len
                       Thread.Sleep((int)(200 * len));
                       if (len < lenSum / lenCount)
                       {
                           KeyUp(key1);
                           KeyUp(key2);
                       }
                       //Thread.Sleep(200);
                   }
               }
               else if(iter/coeff<97)
               {
                   KeyUp(Keys.Up);
                   KeyDown(Keys.Down);
                   KeyUp(key1);
                   KeyUp(key2);
               }
                else
                {
                    KeyUp(Keys.Down);

                    Screen currentScreen = Screen.PrimaryScreen;
                    Bitmap bmpScreenShot = new Bitmap(currentScreen.Bounds.Width,
                                              currentScreen.Bounds.Height,
                                              PixelFormat.Format32bppArgb);
                    Graphics gScreenShot = Graphics.FromImage(bmpScreenShot);
                    gScreenShot.CopyFromScreen(currentScreen.Bounds.X,
                                               currentScreen.Bounds.Y,
                                               0, 0,
                                               currentScreen.Bounds.Size,
                                               CopyPixelOperation.SourceCopy);
                    if (bmpOld == null)
                    {
                        bmpOld = bmpScreenShot;
                        continue;
                    }
                    long sumLeft = 0, sumRight = 0;
                    for (int i = 0; i < bmpOld.Width; i += 20)
                    {

                        for (int j = 0; j < bmpOld.Height; j += 20)
                        {

                            if (i < bmpOld.Width / 2)
                                sumLeft +=
                                    Math.Abs(bmpOld.GetPixel(i, j).R - bmpScreenShot.GetPixel(i, j).R)
                                    +
                                Math.Abs(bmpOld.GetPixel(i, j).G - bmpScreenShot.GetPixel(i, j).G)
                                    +
                                    Math.Abs(bmpOld.GetPixel(i, j).B - bmpScreenShot.GetPixel(i, j).B)
                                ;
                            else
                                sumRight +=
                                     Math.Abs(bmpOld.GetPixel(i, j).R - bmpScreenShot.GetPixel(i, j).R)
                                    +
                                    Math.Abs(bmpOld.GetPixel(i, j).G - bmpScreenShot.GetPixel(i, j).G)
                                    +
                                    Math.Abs(bmpOld.GetPixel(i, j).B - bmpScreenShot.GetPixel(i, j).B)
                                ;
                        }
                    }
                    KeyUp(key1);
                    KeyUp(key2);
                    if (sumRight != 0 && sumLeft != 0 && sumLeft / sumRight <= 5 && sumRight / sumLeft <= 5)
                    {
                        double len;
                        if (sumLeft > sumRight)
                        {
                            len = sumLeft / sumRight;
                            key1 = Keys.Left;
                            key2 = Keys.Z;
                        }
                        else
                        {
                            len = sumRight/sumLeft;
                            key1 = Keys.Right;
                            key2 = Keys.X;
                        }
                        KeyDown(Keys.Up);
                        KeyDown(key2);
                        KeyDown(key1);
                        len = Math.Min(len, 20);
                        lenSum += len;
                        lenCount++;
                        Thread.Sleep((int)(200*len));
                        if (len < lenSum / lenCount)
                        {
                            KeyUp(key1);
                            KeyUp(key2);
                        }
                        //Thread.Sleep(200);
                    }
                }
                iter++;
               if(iter/coeff==100)
               {
                   iter=0;
                   Random rnd = new Random();
                   int dir=rnd.Next(0,2);
                   if (dir == 0)
                       kDir = Keys.Up;
                   else
                       kDir = Keys.Down;
               }
           }
        }
        static void KeyDown(Keys key)
        {
            IntPtr hWnd = GetForegroundWindow();
            int pid;
            GetWindowThreadProcessId(hWnd, out pid);
            using (Process p = Process.GetProcessById(pid))
            {
                if (p.ProcessName != "opera" && p.ProcessName != "iexplore")
                    return;
            }
            byte code = 0;
            Keys[] arrK = new Keys[] { Keys.Up, Keys.Left, Keys.Right, Keys.Down, Keys.Space, Keys.Z, Keys.X };
            int[] arrC=new int[] {0x26,0x25,0x27,0x28,0x20,0x5A,0x58};
            int i;
            for (i = 0; i < arrK.Length; i++)
            {
                if (arrK[i] == key)
                    break;
            }
            code = (byte)arrC[i];
            keybd_event(code, 0, 0, 0);
            //keybd_event((byte)0x26, 0, 0x02, 0);
        }
        static void KeyPress(Keys key, int ms)
        {
            IntPtr hWnd = GetForegroundWindow();
            int pid;
            GetWindowThreadProcessId(hWnd, out pid);
            using (Process p = Process.GetProcessById(pid))
            {
                if (p.ProcessName != "opera" && p.ProcessName != "iexplore")
                    return;
            }
            byte code = 0;
            Keys[] arrK = new Keys[] { Keys.Up, Keys.Left, Keys.Right, Keys.Down, Keys.Space, Keys.Z, Keys.X };
            int[] arrC = new int[] { 0x26, 0x25, 0x27, 0x28, 0x20, 0x5A, 0x58 };
            int i;
            for (i = 0; i < arrK.Length; i++)
            {
                if (arrK[i] == key)
                    break;
            }
            code = (byte)arrC[i];
            keybd_event(code, 0, 0, 0);
            Thread.Sleep(ms);
            keybd_event(code, 0, 0x02, 0);
        }
        static void KeyUp(Keys key)
        {
            IntPtr hWnd = GetForegroundWindow();
            int pid;
            GetWindowThreadProcessId(hWnd, out pid);
            using (Process p = Process.GetProcessById(pid))
            {
                if (p.ProcessName != "opera" && p.ProcessName != "iexplore")
                    return;
            }
            byte code = 0;
            Keys[] arrK = new Keys[] { Keys.Up, Keys.Left, Keys.Right, Keys.Down, Keys.Space, Keys.Z, Keys.X };
            int[] arrC = new int[] { 0x26, 0x25, 0x27, 0x28, 0x20, 0x5A, 0x58 };
            int i;
            for (i = 0; i < arrK.Length; i++)
            {
                if (arrK[i] == key)
                    break;
            }
            code = (byte)arrC[i];
            keybd_event(code, 0, 0x02, 0);
        }
    }
}
