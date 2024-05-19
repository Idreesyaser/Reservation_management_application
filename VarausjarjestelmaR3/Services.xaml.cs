using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using VarausjarjestelmaR3.Classes;

namespace VarausjarjestelmaR3
{
    /// <summary>
    /// Interaction logic for Services.xaml
    /// </summary>
    public partial class Services : UserControl
    {

        Repository repo = new Repository();

        public ObservableCollection<Service> ServicesOC { get; set; }

        public ObservableCollection<Office> Offices { get; set; }

        public Services()
        {
            InitializeComponent();
            Offices = repo.GetAllOffices();
            ServicesOC = repo.GetAllServices(Offices);

            ServicesDataGrid.ItemsSource = ServicesOC;

            ComOffices.ItemsSource = Offices;

        }

        /// <summary>
        /// Poistaa valitun rivin
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoveServiceBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ServicesDataGrid.SelectedItems.Count == 0)
            {
                return;
            }

            Service selectedservice = ServicesDataGrid.SelectedItem as Service;

            repo.RemoveService(selectedservice);

            ServicesOC.Remove(selectedservice);

        }

        /// <summary>
        /// Tallentaa rivit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveServiceBtn_Click(object sender, RoutedEventArgs e)
        {
            var services = (ObservableCollection<Service>)ServicesDataGrid.ItemsSource;

            repo.SaveServices(services);

            ServicesOC = repo.GetAllServices(Offices);
            ServicesDataGrid.ItemsSource = ServicesOC;

        }
    }
}
