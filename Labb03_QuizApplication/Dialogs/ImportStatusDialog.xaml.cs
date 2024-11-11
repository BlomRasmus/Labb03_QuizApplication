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
using System.Windows.Shapes;

namespace Labb03_QuizApplication.Dialogs
{
    /// <summary>
    /// Interaction logic for ImportStatusDialog.xaml
    /// </summary>
    public partial class ImportStatusDialog : Window
    {
        public ImportStatusDialog()
        {
            InitializeComponent();
            DataContext = (App.Current.MainWindow as MainWindow).DataContext;
        }
    }
}
