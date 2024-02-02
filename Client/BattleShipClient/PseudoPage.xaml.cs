using BattleShipClient;
using System.Text;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


namespace BattleShipClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public sealed partial class PseudoPage 
    {
        Window window;
        public PseudoPage()
        {
            this.window = Window.Current;
            
            //     DataContext = new ViewModels.PseudoModel();
            //Pseudo pseudo = new Pseudo(this);
        }

        public void showMenu()
        {
            //DataContext = new ViewModels.MenuModel();
            this.window.Content = new Page();
        }
    }
}