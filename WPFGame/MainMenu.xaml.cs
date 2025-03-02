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

namespace WPFGame
{
    public partial class MainMenu : UserControl
    {
        public event Action StartGameClicked;
        public event Action ShowStatsClicked;
        public event Action ShowSettingsClicked;
        public event Action ExitClicked;

        public MainMenu()
        {
            InitializeComponent();
        }

        private void StartGame_Click(object sender, RoutedEventArgs e)
        {
            StartGameClicked?.Invoke();
        }

        private void ShowStats_Click(object sender, RoutedEventArgs e)
        {
            ShowStatsClicked?.Invoke();
        }

        private void ShowSettings_Click(object sender, RoutedEventArgs e)
        {
            ShowSettingsClicked?.Invoke();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            ExitClicked?.Invoke();
        }
    }
}