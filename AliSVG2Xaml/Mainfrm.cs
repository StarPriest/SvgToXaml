using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AliSVG2Xaml
{
    public partial class Mainfrm : Form
    {
        public Mainfrm()
        {
            InitializeComponent();
            AddClipboardFormatListener(this.Handle);  
        }

        [DllImport("user32.dll")]
        public static extern bool AddClipboardFormatListener(IntPtr hwnd);

        [DllImport("user32.dll")]
        public static extern bool RemoveClipboardFormatListener(IntPtr hwnd);

        private static int WM_CLIPBOARDUPDATE = 0x031D;

        private void Mainfrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            RemoveClipboardFormatListener(this.Handle);  
        }

        protected override void DefWndProc(ref Message m)
        {
            if (m.Msg == WM_CLIPBOARDUPDATE)
            {
                var clipboardText = Clipboard.GetText();
                if (!IsSvgStr(clipboardText)) return;
                var xamlStr = GetXaml(clipboardText);
                Clipboard.Clear();
                Clipboard.SetDataObject(xamlStr);
                notifyIcon1.BalloonTipTitle = @"SVG提取工具";
                notifyIcon1.BalloonTipText = @"SVG转Xaml完成！";
                notifyIcon1.ShowBalloonTip(1000);
            }
            else
            {
                base.DefWndProc(ref m);
            }
        }

        /// <summary>
        ///     检测剪贴板内是否是阿里云图库的svg图标字符串
        ///     仅做了简单检测
        /// </summary>
        /// <param name="clipboardText">剪贴板内的字符串</param>
        /// <returns></returns>
        private static bool IsSvgStr(string clipboardText)
        {
            if (clipboardText == null) return false;


            return clipboardText.StartsWith("<svg") && clipboardText.EndsWith("</svg>");
        }

        /// <summary>
        ///     通过正则表达式获取需要的部分并转化为xaml资源
        /// </summary>
        /// <param name="clipboardText"></param>
        /// <returns></returns>
        private static string GetXaml(string clipboardText)
        {
            var ret = string.Empty;
            const string svgTextPattern = "M.*?z";
            ret = Regex.Matches(clipboardText, svgTextPattern).Cast<object>()
                .Aggregate(ret, (current, SvgText) => current + SvgText);


            return $" <Geometry x:Key=\"\">{ret}</Geometry>";
        }

        private void Mainfrm_Load(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
          //  this.ShowInTaskbar = false;
        }

        private void 退出XToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Mainfrm_MinimumSizeChanged(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                notifyIcon1.Visible = true;
            }
        }
    }
}
