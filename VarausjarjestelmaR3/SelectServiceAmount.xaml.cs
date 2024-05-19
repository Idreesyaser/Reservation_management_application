using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VarausjarjestelmaR3.Classes;

namespace VarausjarjestelmaR3
{
    /// <summary>
    /// Interaction logic for SelectServiceAmount.xaml
    /// </summary>
    public partial class SelectServiceAmount : Window
    {

        Repository repo = new Repository();

        public Service SelectedService { get; set; }

        public SelectServiceAmount(Service data)
        {
            InitializeComponent();
            SelectedService = data;

            AmountTB.Text = SelectedService.ValittuMaara.ToString();
        }

        /// <summary>
        /// Lisää palveluiden määrää
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(AmountTB.Text, out int parsedAmount))
            {
                AmountTB.Text = (parsedAmount + 1).ToString();

            }

        }

        /// <summary>
        /// Vähentää palveluiden määrää
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReduceBtn_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(AmountTB.Text, out int parsedAmount))
            {
                AmountTB.Text = (parsedAmount - 1).ToString();
            }
        }

        private void AcceptBtn_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(AmountTB.Text, out int parsedAmount))
            {
                SelectedService.Maara = parsedAmount;

            }

            Close();
        }

        private static readonly Regex _regex = new Regex("[^0-9]+");
        private static bool IsTextAllowed(string text)
        {
            return _regex.IsMatch(text);
        }

        private void AmountTB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private void AmountTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!int.TryParse(AmountTB.Text, out int parsedAmount))
            {
                return;
            }

            if (parsedAmount + 1 > SelectedService.Maara)
            {
                AddBtn.IsEnabled = false;
            }
            else
            {
                AddBtn.IsEnabled = true;
            }

            if (parsedAmount == 1)
            {
                ReduceBtn.IsEnabled = false;
            }
            else
            {
                ReduceBtn.IsEnabled = true;
            }
        }
    }
}
