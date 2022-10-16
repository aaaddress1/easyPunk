using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace easyPunk
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        [DllImport("user32.dll")]
        private static extern bool PostMessage(int hhwnd, uint msg, IntPtr wparam, IntPtr lparam);

        [DllImport("user32.dll")]
        private static extern IntPtr LoadKeyboardLayout(string pwszKLID, uint Flags);


        private static uint WM_INPUTLANGCHANGEREQUEST = 0x0050;
        private static int HWND_BROADCAST = 0xffff;
        private static string en_US = "00000409"; //英文
        private static string cn_ZH = "00000804";
        private static string zh_TW = "00000404";
        private static uint KLF_ACTIVATE = 1;

        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);


        private void Form1_Load(object sender, EventArgs e)
        {
            
            (new Thread(() =>
            {
                IntPtr lastCheckWindow = IntPtr.Zero;
                for (; ; Thread.Sleep(300) )
                {
                    if (lastCheckWindow != GetForegroundWindow())
                    {
                        StringBuilder Buff = new StringBuilder(256);
                        lastCheckWindow = GetForegroundWindow();
                        GetWindowText(lastCheckWindow, Buff, 256);
                  
                        if (Buff.ToString().StartsWith("Cyberpunk 2077 (C)"))
                            // disable IME
                            PostMessage(HWND_BROADCAST, WM_INPUTLANGCHANGEREQUEST, IntPtr.Zero, LoadKeyboardLayout(en_US, KLF_ACTIVATE));
                        else
                            // recover IME (zh_tw)
                            PostMessage(HWND_BROADCAST, WM_INPUTLANGCHANGEREQUEST, IntPtr.Zero, LoadKeyboardLayout(zh_TW, KLF_ACTIVATE));

                        Invoke(new Action(() => { this.label1.Text = "當前掃描到視窗: " + Buff.ToString(); }));
                    }
   
                }

            })
            { IsBackground = true }).Start();

        }

        private void 關閉EasyPunkToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Visible = false;
            notifyIcon1.ShowBalloonTip(1000);
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Visible = true;
        }
    }
}
