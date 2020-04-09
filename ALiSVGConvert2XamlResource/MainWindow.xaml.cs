using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ALiSVGConvert2XamlResource
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
   
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var input = OldTextBox.Text;
            if (!IsSvgStr(input.ToString())) return;
            NewTextBox.Text = GetXaml(input.ToString());

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

            var ret= clipboardText.StartsWith("<svg") ;
            var tet2=clipboardText.EndsWith("svg>");
            return ret;
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
            var strs = Regex.Matches(clipboardText, svgTextPattern);

            foreach (var o in strs)
            {
                ret = ret + o;
            }

            return $" <Geometry x:Key=\"\">{ret}</Geometry>";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var input = OldTextBox.Text;
            if (!IsSvgStr(input.ToString())) return;
            NewTextBox.Text = GetXaml(input.ToString());
        }
    }
}
