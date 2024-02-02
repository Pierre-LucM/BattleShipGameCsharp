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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace jesuisunemerde.Views
{
    /// <summary>
    /// Logique d'interaction pour Pseudo.xaml
    /// </summary>
    public partial class Pseudo : UserControl
    {
        private readonly MainWindow mainWindow;

        public Pseudo(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
        }

        private void btn_StartPlaying(object sender, RoutedEventArgs e)
        {
            mainWindow.showMenu();
        }
    }
}
