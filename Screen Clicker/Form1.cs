using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;


namespace Screen_Clicker
{


    public partial class MainWin : Form
    {
        public MainWin()
        {
            InitializeComponent();

            MyController cont = new MyController();
            Thread thread = new Thread(new ThreadStart(cont.Screen_Scan));
            thread.IsBackground = true;
            thread.Name = "Checking!";
            thread.Start();
        }
        
        public void IncrementCounter()
        {
            if (CounterLabel.InvokeRequired)
            {
                CounterLabel.Invoke(new MethodInvoker(delegate
                {
                    int CurrCounterNum = int.Parse(CounterLabel.Text);
                    int NewCounterNum = CurrCounterNum + 1;
                    CounterLabel.Text = NewCounterNum.ToString();

                }));
            }
        }
        
    }
    public class MyController
    {
        public int runs = 0;
        int posY = 0;
        int posX = 0;
        

        public void Screen_Scan() // this code loops forever on it's own thread.
        {

            Bitmap objective = Properties.Resources.bin; //Set to an image in the resources.resx and resources folder.

            while (true)
            {
                Bitmap screenScan = new Bitmap(Screen.PrimaryScreen.Bounds.Width,
                                               Screen.PrimaryScreen.Bounds.Height); // Defines the screen

                Graphics g = Graphics.FromImage(screenScan);


                g.CopyFromScreen(Screen.PrimaryScreen.Bounds.X,
                                 Screen.PrimaryScreen.Bounds.Y,
                                 0, 0, screenScan.Size, CopyPixelOperation.SourceCopy);

                //screenScan.Save("D:/c# fun/Auto Twitch Farm/OUT.bmp");  //DEBUG

                bool imageScanResult = Compare_Scan(objective, screenScan);
                if (imageScanResult == true)
                {

                    Mouse.MoveTo(posX + objective.Width / 2, posY + objective.Height / 2);
                    Mouse.LeftClick();
                    Mouse.MoveTo(960, 540);

                }

                
                //Best way to change UI element based on the cycles of the method.
                MainWin win = (MainWin)Application.OpenForms["MainWin"];
                win.IncrementCounter();
                runs++;
                if (runs >= 5)
                {
                    GC.Collect(); //GC needs to be called because memory usage will go up into the gigabytes.
                    runs = 0;
                }

                Thread.Sleep(50);
            }
        }
        private bool Compare_Scan(Bitmap searchFor, Bitmap searchIn)
        {
            try //searchFor casuses ArgumentOutOfRangeException but isn't fatal.
            {
                for (int x = 0; x < searchIn.Width; x++)
                {
                    for (int y = 0; y < searchIn.Height; y++)
                    {
                        bool invalid = false;
                        int k = x, l = y;
                        for (int a = 0; a < searchFor.Width; a++)
                        {
                            l = y;
                            for (int b = 0; b < searchFor.Height; b++)
                            {
                                if (searchFor.GetPixel(a, b) != searchIn.GetPixel(k, l))
                                {
                                    posX = k;
                                    posY = l;
                                    invalid = true;
                                    break;
                                }
                                else
                                    l++;
                            }
                            if (invalid)
                                break;
                            else
                                k++;
                        }
                        if (!invalid)
                            return true;
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
    public class Mouse
    {
        [DllImport("user32.dll")]
        static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);
        private const int MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const int MOUSEEVENTF_LEFTUP = 0x0004;
        private const int MOUSEEVENTF_MOVE = 0x0001;
        private const int MOUSEEVENTF_ABSOLUTE = 0x8000;
        public static void LeftClick()
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN, System.Windows.Forms.Control.MousePosition.X, System.Windows.Forms.Control.MousePosition.Y, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, System.Windows.Forms.Control.MousePosition.X, System.Windows.Forms.Control.MousePosition.Y, 0, 0);
        }
        public static void MoveTo(float x, float y)
        {
            float min = 0;
            float max = 65535;


            int mappedX = (int)Remap(x, 0.0f, 1920.0f, min, max);
            int mappedY = (int)Remap(y, 0.0f, 1080.0f, min, max);

            mouse_event(MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE, mappedX, mappedY, 0, 0);
        }

       
        private static float Remap(float value, float from1, float to1, float from2, float to2)
        {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }
    }

}