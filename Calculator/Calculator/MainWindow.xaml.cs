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

namespace Calculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string DEFAULTRESULTS = "0";
        private const string NEGATIVERESULTS = "-";
        public MainWindow()
        {
            InitializeComponent();
        }

        public void NumbClick(object sender, RoutedEventArgs e)
        {
            var buttonClick = (sender as Button);

            if (Results.Text.ToString().Equals(DEFAULTRESULTS))
                Results.Text = buttonClick.Content.ToString();
            else
                Results.Text += buttonClick.Content.ToString();
        }

        public void ClearClick(object sender, RoutedEventArgs e)
        {
            Results.Text = DEFAULTRESULTS;
        }

        public void NegativeClick(object sender, RoutedEventArgs e)
        {
            if (Results.Text.Equals(DEFAULTRESULTS))
                return;
            if (Results.Text.Contains(NEGATIVERESULTS))
                Results.Text = Results.Text.Remove(0,1);
            else
                Results.Text = Results.Text.Insert(0, NEGATIVERESULTS);
        }
    }
}
