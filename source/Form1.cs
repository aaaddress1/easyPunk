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

        public delegate bool WndEnumProc(
           UInt32 hWnd,
           IntPtr lParam
        );

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

        [DllImport("user32.dll")]
        static extern int GetWindowTextLengthW(IntPtr hWnd);

        [DllImport("user32.dll")]
        static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);
        public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        private void refreshComboBoxItems()
        {
            Invoke(new Action(() => {
                this.comboBox1.Items.Clear();


                EnumWindows(delegate (IntPtr hwnd, IntPtr param) {
                    StringBuilder Buff = new StringBuilder(256);
                    if (IsWindowVisible(hwnd) && GetWindowTextLengthW(hwnd) != 0)
                    {
                        GetWindowText(hwnd, Buff, 256);
                        this.comboBox1.Items.Add(Buff.ToString());
                    }

                    return true;
                }, IntPtr.Zero);
            }));
        }


        private void Form1_Load(object sender, EventArgs e)
        {

            (new Thread(() =>
            {
            IntPtr lastCheckWindow = IntPtr.Zero;
            String comboBoxSelected = null;

            refreshComboBoxItems();

            for (; ; Thread.Sleep(300))
            {
                

                Invoke(new Action(() => {
                    if (comboBox1.Focused && comboBox1.GetItemText(comboBox1.SelectedItem) != "")
                    {
                        comboBoxSelected = comboBox1.GetItemText(comboBox1.SelectedItem);
                        Invoke(new Action(() => { this.label3.Text = "當前匹配視窗字串：" + comboBoxSelected; }));
                    }
                }));


                if (lastCheckWindow != GetForegroundWindow())
                {
                    refreshComboBoxItems();
                    StringBuilder Buff = new StringBuilder(256);
                    lastCheckWindow = GetForegroundWindow();
                    GetWindowText(lastCheckWindow, Buff, 256);

                    if (comboBoxSelected == "" || comboBoxSelected == null)
                        comboBoxSelected = "Cyberpunk 2077 (C)";


                    if (Buff.ToString().StartsWith(comboBoxSelected))
                        // disable IME
                        PostMessage(HWND_BROADCAST, WM_INPUTLANGCHANGEREQUEST, IntPtr.Zero, LoadKeyboardLayout(en_US, KLF_ACTIVATE));
                    else
                        // recover IME (zh_tw)
                        PostMessage(HWND_BROADCAST, WM_INPUTLANGCHANGEREQUEST, IntPtr.Zero, LoadKeyboardLayout(zh_TW, KLF_ACTIVATE));

                    Invoke(new Action(() => { this.label1.Text = "當前掃描到視窗: " + Buff.ToString(); }));

                    Invoke(new Action(() => { this.label3.Text = "當前匹配視窗字串：" + comboBoxSelected; }));
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

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }
    }
}
