using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using Wpf.Ui.Controls;

namespace intsis.Views
{
    /// <summary>
    /// Логика взаимодействия для WebBrowser.xaml
    /// </summary>
    public partial class WebBrowser : Page
    {
        public WebBrowser()
        {
            InitializeComponent();
            InitAsync();
        }
        async void InitAsync()
        {
            await WebView.EnsureCoreWebView2Async(null);
            WebView.Source = new Uri("https://wise-choice.ru");
        }

       
    
    }
}
