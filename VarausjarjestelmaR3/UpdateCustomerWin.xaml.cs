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
using VarausjarjestelmaR3.Classes;

namespace VarausjarjestelmaR3
{
    /// <summary>
    /// Interaction logic for UpdateCustomerWin.xaml
    /// </summary>
    public partial class UpdateCustomerWin : Window
    {
        Repository repo;
        public UpdateCustomerWin(Customer customer)
        {
            InitializeComponent();
            this.DataContext = customer;
        }

        private void UpdateCustomer(object sender, RoutedEventArgs e)
        {
            var customer = (Customer)DataContext;
            var repo = new Repository();
            repo.UpdateCustomer(customer);

            DialogResult = true;
        }
    }
}
