using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;


namespace Auto_Twitch_Farm
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
        public void start_MyController()
        {

            MyController cont = new MyController();
            Thread thread = new Thread(new ThreadStart(cont.Screen_Scan));
            thread.IsBackground = true;
            thread.Name = "Checking!";
            thread.Start();
        }
        //static bool checkBoxState = true; //DEBUG


        

    
    public class MyController
    {
        public int runs = 0;
        public int trueruns = 0;
        int posY = 0;
        int posX = 0;

        public void Screen_Scan() // this code runs forever on it's own thread.
        {

            Bitmap objective = Properties.Resources.twitch;
            
            while (true)
            {
                Bitmap screenScan = new Bitmap(Screen.PrimaryScreen.Bounds.Width, 
                                               Screen.PrimaryScreen.Bounds.Height); // Defines the screen

                Graphics g = Graphics.FromImage(screenScan);


                g.CopyFromScreen(Screen.PrimaryScreen.Bounds.X,
                                 Screen.PrimaryScreen.Bounds.Y,
                                 0, 0,screenScan.Size, CopyPixelOperation.SourceCopy);

                //screenScan.Save("D:/c# fun/Auto Twitch Farm/OUT.bmp");  //DEBUG

                bool imageScanResult = Compare_Scan(objective, screenScan);
                if (imageScanResult == true)
                {

                    Mouse.Move_To(posX + objective.Width / 2, posY + objective.Height / 2);
                    Mouse.Left_Click();
                    Mouse.Move_To(960, 540);
                    
                }

                runs++;
                trueruns++;
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



        }
        public class Mouse
        {
            [DllImport("user32.dll")]
            static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);
            private const int MOUSEEVENTF_LEFTDOWN = 0x0002;
            private const int MOUSEEVENTF_LEFTUP = 0x0004;
            private const int MOUSEEVENTF_MOVE = 0x0001;
            private const int MOUSEEVENTF_ABSOLUTE = 0x8000;
            public static void Left_Click()
            {
                mouse_event(MOUSEEVENTF_LEFTDOWN, System.Windows.Forms.Control.MousePosition.X, System.Windows.Forms.Control.MousePosition.Y, 0, 0);
                mouse_event(MOUSEEVENTF_LEFTUP, System.Windows.Forms.Control.MousePosition.X, System.Windows.Forms.Control.MousePosition.Y, 0, 0);
            }
            public static void Move_To(float x, float y)
            {
                float min = 0;
                float max = 65535;


                int mappedX = (int)Remap(x, 0.0f, 1920.0f, min, max);
                int mappedY = (int)Remap(y, 0.0f, 1080.0f, min, max);

                mouse_event(MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE, mappedX, mappedY, 0, 0);
            }

            //This remaps the input into a more uniform 1920x1080
            private static float Remap(float value, float from1, float to1, float from2, float to2)
            {
                return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
            }
        }

    }
}


/*
[DllImport("user32.dll")]
static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);
            private const int MOUSEEVENTF_MOVE = 0x0001;
            private const int MOUSEEVENTF_ABSOLUTE = 0x8000;
            public static void MoveTo(float x, float y)
            {
                float min = 0;
                float max = 65535;

                int mappedX = (int)Remap(x, 0.0f, 1920.0f, min, max);
                int mappedY = (int)Remap(y, 0.0f, 1080.0f, min, max);

                mouse_event(MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE, mappedX, mappedY, 0, 0);
            }
            public static float Remap(float value, float from1, float to1, float from2, float to2)
            {
                return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
            }
*/