using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AliIcon2Xaml
{
    class Program
    {

        #region Definitions

        //Constants for API Calls...
        private const int WM_DRAWCLIPBOARD = 0x308;
        private const int WM_CHANGECBCHAIN = 0x30D;

        //Handle for next clipboard viewer...
        private IntPtr mNextClipBoardViewerHWnd;

        //API declarations...
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SetClipboardViewer(IntPtr hWndNewViewer);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool ChangeClipboardChain(IntPtr HWnd, IntPtr HWndNext);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        #endregion

        static void Main(string[] args)
        {
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

        private static void func()
        {
            while (Clipboard.ContainsText(TextDataFormat.Text))
            {
                if (!IsSvgStr(Clipboard.GetText())) continue;
                var xamlStr = GetXaml(Clipboard.GetText());
                Console.WriteLine($"[{DateTime.Now}]:发现svg字符串! ");
                Clipboard.SetText(xamlStr);
                Console.WriteLine($"[{DateTime.Now}]:SVG字符串修改完成! ");
                Console.WriteLine($"[{DateTime.Now}]:{xamlStr} ");
            }
        }

        #region Message Process

        //Override WndProc to get messages...
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_DRAWCLIPBOARD:
                    {
                        //The clipboard has changed...
                        //##########################################################################
                        // Process Clipboard Here :)........................
                        //##########################################################################
                        SendMessage(mNextClipBoardViewerHWnd, m.Msg, m.WParam.ToInt32(), m.LParam.ToInt32());

                        //显示剪贴板中的文本信息
                        if (Clipboard.ContainsText())
                        {

                            var ClipboardText = Clipboard.GetText();



                        }



                        break;
                    }
                case WM_CHANGECBCHAIN:
                    {
                        //Another clipboard viewer has removed itself...
                        if (m.WParam == mNextClipBoardViewerHWnd)
                            mNextClipBoardViewerHWnd = m.LParam;
                        else
                            SendMessage(mNextClipBoardViewerHWnd, m.Msg, m.WParam.ToInt32(), m.LParam.ToInt32());
                        break;
                    }
            }

            base.WndProc(ref m);
        }

        #endregion
    }
}
