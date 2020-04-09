using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;

namespace AliSVG2XamlResources
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var th = new Thread(func);
            th.SetApartmentState(ApartmentState.STA);
            th.Start();
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
                var ClipboardText = System.Windows.Clipboard.GetText();
                if (!IsSvgStr(ClipboardText)) continue;
                var xamlStr = GetXaml(ClipboardText);
                Console.WriteLine($"[{DateTime.Now}]:发现svg字符串! ");
                Clipboard.Clear();
                Clipboard.SetDataObject(xamlStr);
                Console.WriteLine($"[{DateTime.Now}]:SVG字符串修改完成! ");
                Console.WriteLine($"[{DateTime.Now}]:{xamlStr} ");
            }
        }

   
    }
}