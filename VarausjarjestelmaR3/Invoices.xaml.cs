using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
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
using VarausjarjestelmaR3;
using VarausjarjestelmaR3.Classes;

namespace VarausjarjestelmaR3
{
    public partial class Invoices : UserControl
    {
        private string connectionString = "Server=127.0.0.1; Port=3306; User ID=opiskelija; Pwd=opiskelija1; Database=vuokratoimistot;";
        private string currentPaymentMethod;

        public Invoices()
        {
            InitializeComponent();
            LoadData();
            amountExVATTextBox.TextChanged += AmountExVATTextBox_TextChanged;
            invoicesListView.ItemsSource = null;
        }

        private void LoadData()
        {
            LoadReservations();
            LoadSavedInvoices();
            SetNextInvoiceNumber();
        }

        private void LoadReservations()
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM asiakkaan_varaus";
                var command = new MySqlCommand(query, connection);
                var reader = command.ExecuteReader();
                var reservations = new ObservableCollection<Reservation>();

                while (reader.Read())
                {
                    var reservation = new Reservation
                    {
                        VarausID = reader.GetInt32("varausID"),
                        VarausAlkaa = reader.GetDateTime("varaus_alkaa"),
                        VarausPaattyy = reader.GetDateTime("varaus_paattyy"),
                        Varauspaiva = reader.GetDateTime("varauspvm"),
                        Lisatiedot = reader.GetString("lisatiedot"),
                        Asiakas = LoadCustomer(reader.GetInt32("asiakasID")),
                        Huone = LoadRoom(reader.GetInt32("huoneen_numeroID")),
                        VarauksenPalvelut = LoadReservationServices(reader.GetInt32("varausID"))
                    };
                    reservations.Add(reservation);
                }

                invoicesListView.ItemsSource = reservations;
            }
        }

        private Customer LoadCustomer(int customerId)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM asiakas WHERE asiakasID = @customerId";
                var command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@customerId", customerId);
                var reader = command.ExecuteReader();

                if (reader.Read())
                {
                    return new Customer
                    {
                        AsiakasID = reader.GetInt32("asiakasID"),
                        Nimi = reader.GetString("nimi"),
                        Puhelin = reader.GetString("puhelin"),
                        Katuosoite = reader.GetString("katuosoite"),
                        Postinumero = reader.GetString("postinumero"),
                        Postitoimipaikka = reader.GetString("postitoimipaikka"),
                        Sahkoposti = reader.GetString("sahkoposti")
                    };
                }
                return null;
            }
        }

        private Room LoadRoom(int roomId)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM huoneet WHERE huoneen_numeroID = @roomId";
                var command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@roomId", roomId);
                var reader = command.ExecuteReader();

                if (reader.Read())
                {
                    return new Room
                    {
                        HuoneenNumeroID = reader.GetInt32("huoneen_numeroID"),
                        Nimi = reader.GetString("nimi"),
                        Hinta = reader.GetDouble("hinta"),
                        AlvProsentti = reader.GetDouble("alv_prosentti"),
                        HloMaara = reader.GetInt32("hlo_maara"),
                        Toimipiste = LoadOffice(reader.GetInt32("toimipisteID"))
                    };
                }
                return null;
            }
        }

        private Office LoadOffice(int officeId)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM toimipiste WHERE toimipisteID = @officeId";
                var command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@officeId", officeId);
                var reader = command.ExecuteReader();

                if (reader.Read())
                {
                    return new Office
                    {
                        ToimipisteID = reader.GetInt32("toimipisteID"),
                        ToimipisteNimi = reader.GetString("toimipiste_nimi"),
                        Paikkakunta = reader.GetString("paikkakunta"),
                        Katuosoite = reader.GetString("katuosoite"),
                        Postinumero = reader.GetString("postinumero"),
                        Postitoimipaikka = reader.GetString("postitoimipaikka"),
                        Puhelin = reader.GetString("puhelin")
                    };
                }
                return null;
            }
        }

        //lataa varaukseen liittyvät palvelut varauksen ID perusteella.
        private ObservableCollection<ReservationService> LoadReservationServices(int reservationId)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM varauksen_palvelut WHERE varausID = @reservationId";
                var command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@reservationId", reservationId);
                var reader = command.ExecuteReader();
                var services = new ObservableCollection<ReservationService>();

                while (reader.Read())
                {
                    services.Add(new ReservationService
                    {
                        PalveluvarausID = reader.GetInt32("as_palveluvarauksenID"),
                        Palvelu = LoadService(reader.GetInt32("palveluID")),
                        Kpl = reader.GetInt32("kpl"),
                        VarausID = reader.GetInt32("varausID")
                    });
                }

                return services;
            }
        }

        //lataa palvelun tiedot palvelun ID mukaan
        private Service LoadService(int serviceId)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM palvelu WHERE palveluID = @serviceId";
                var command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@serviceId", serviceId);
                var reader = command.ExecuteReader();

                if (reader.Read())
                {
                    return new Service
                    {
                        PalveluID = reader.GetInt32("palveluID"),
                        Tuote = reader.GetString("tuote"),
                        PalvelunHinta = reader.GetDouble("palvelun_hinta"),
                        AlvProsentti = reader.GetDouble("alv_prosentti"),
                        Maara = reader.GetInt32("maara"),
                        Toimipiste = LoadOffice(reader.GetInt32("toimipisteID"))
                    };
                }
                return null;
            }
        }

        //lataa kaikki tallennetut laskut tietokannasta
        public void LoadSavedInvoices()
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM lasku";
                var command = new MySqlCommand(query, connection);
                var reader = command.ExecuteReader();
                var invoices = new ObservableCollection<Invoice>();

                while (reader.Read())
                {
                    var invoice = new Invoice
                    {
                        Laskunumero = reader.GetInt32("laskuID"),
                        Laskutustapa = reader.GetString("laskutustapa"),
                        VerotonSumma = reader.GetDouble("veroton_summa"),
                        AlvEuroina = reader.GetDouble("alv_euroina"),
                        Loppusumma = reader.GetDouble("loppusumma"),
                        AsiakasID = reader.GetInt32("asiakasID"),
                        VarausID = reader.GetInt32("varausID"),
                        Asiakas = LoadCustomer(reader.GetInt32("asiakasID")),
                        Varaus = LoadReservation(reader.GetInt32("varausID"))
                    };
                    invoices.Add(invoice);
                }

                savedInvoicesListView.ItemsSource = invoices;
            }
        }

        //altaa varaustiedot varauksen ID mukaan
        private Reservation LoadReservation(int reservationId)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM asiakkaan_varaus WHERE varausID = @reservationId";
                var command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@reservationId", reservationId);
                var reader = command.ExecuteReader();

                if (reader.Read())
                {
                    return new Reservation
                    {
                        VarausID = reader.GetInt32("varausID"),
                        VarausAlkaa = reader.GetDateTime("varaus_alkaa"),
                        VarausPaattyy = reader.GetDateTime("varaus_paattyy"),
                        Varauspaiva = reader.GetDateTime("varauspvm"),
                        Lisatiedot = reader.GetString("lisatiedot"),
                        Asiakas = LoadCustomer(reader.GetInt32("asiakasID")),
                        Huone = LoadRoom(reader.GetInt32("huoneen_numeroID")),
                        VarauksenPalvelut = LoadReservationServices(reader.GetInt32("varausID"))
                    };
                }
                return null;
            }
        }

        //asettaa seuraavan tilinumeron ja lisää nykyistä enimmäismäärää yhdellä
        private void SetNextInvoiceNumber()
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT MAX(laskuID) FROM lasku";
                var command = new MySqlCommand(query, connection);
                var result = command.ExecuteScalar();
                int nextInvoiceNumber = (result != DBNull.Value) ? Convert.ToInt32(result) + 1 : 1;
                invoiceNumberTextBox.Text = nextInvoiceNumber.ToString();
            }
        }

        //maksutavan valinta
        private void PaymentMethod_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton radioButton && radioButton.IsChecked == true)
            {
                currentPaymentMethod = radioButton.Content.ToString();
            }
        }

        //päivittää alv-arvon ja kokonaissumman, kun arvonlisäverotonta summaa muutetaan.
        private void AmountExVATTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateVATandTotal();
        }

        //laskee ja päivittää arvonlisäveron arvon ja kokonaissumman arvonlisäverottoman summan perusteella.
        private void UpdateVATandTotal()
        {
            if (double.TryParse(amountExVATTextBox.Text, out double amountExVAT))
            {
                double vat = amountExVAT * 0.24; // ALV 24%
                vatTextBox.Text = vat.ToString("N2");
                double totalAmount = amountExVAT + vat;
                totalAmountTextBox.Text = totalAmount.ToString("N2");
            }
            else
            {
                vatTextBox.Text = "0.00";
                totalAmountTextBox.Text = "0.00";
            }
        }

        private void SaveInvoice_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!int.TryParse(varausIDTextBox.Text, out int varausID))
                {
                    MessageBox.Show("Virheellinen syöte.");
                    ClearCustomerFields();
                    ClearFinancialFields();
                    return;
                }

                if (invoicesListView.SelectedItem is Reservation selectedReservation &&
                    double.TryParse(amountExVATTextBox.Text, out double amountExVAT) &&
                    double.TryParse(vatTextBox.Text, out double vat) &&
                    double.TryParse(totalAmountTextBox.Text, out double totalAmount))
                {
                    int nextInvoiceNumber = Convert.ToInt32(invoiceNumberTextBox.Text);

                    Invoice newInvoice = new Invoice
                    {
                        Laskunumero = nextInvoiceNumber,
                        VerotonSumma = amountExVAT,
                        AlvEuroina = vat,
                        Loppusumma = totalAmount,
                        Laskutustapa = currentPaymentMethod,
                        AsiakasID = selectedReservation.Asiakas.AsiakasID,
                        Asiakas = selectedReservation.Asiakas,
                        VarausID = varausID,
                        Varaus = selectedReservation
                    };

                    SaveInvoiceToDatabase(newInvoice);

                    if (savedInvoicesListView.ItemsSource is ObservableCollection<Invoice> savedInvoices)
                    {
                        savedInvoices.Add(newInvoice);
                        savedInvoicesListView.Items.Refresh();
                    }
                    else
                    {
                        savedInvoicesListView.ItemsSource = new ObservableCollection<Invoice> { newInvoice };
                    }

                    MessageBox.Show("Lasku tallennettiin ja lisättiin luetteloon.");
                }
                else
                {
                    MessageBox.Show("Virheellinen syöte.");
                    ClearFinancialFields();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Tapahtui virhe: {ex.Message}");
            }
        }

        //tallentaa uuden tilin tietokantaan
        private void SaveInvoiceToDatabase(Invoice invoice)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = @"INSERT INTO lasku (laskutustapa, alv_euroina, veroton_summa, loppusumma, asiakasID, varausID) 
                                 VALUES (@Laskutustapa, @AlvEuroina, @VerotonSumma, @Loppusumma, @AsiakasID, @VarausID)";
                var command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@Laskutustapa", invoice.Laskutustapa);
                command.Parameters.AddWithValue("@AlvEuroina", invoice.AlvEuroina);
                command.Parameters.AddWithValue("@VerotonSumma", invoice.VerotonSumma);
                command.Parameters.AddWithValue("@Loppusumma", invoice.Loppusumma);
                command.Parameters.AddWithValue("@AsiakasID", invoice.AsiakasID);
                command.Parameters.AddWithValue("@VarausID", invoice.VarausID);
                command.ExecuteNonQuery();
            }
        }

        //käsittelee ID-kentän muutokset lataamalla asianmukaiset tiedot
        private void VarausIDTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (int.TryParse(varausIDTextBox.Text, out int varausID))
                {
                    var reservation = LoadReservation(varausID);

                    if (reservation?.Asiakas != null)
                    {
                        customerNameTextBox.Text = reservation.Asiakas.Nimi;
                        customerPhoneTextBox.Text = reservation.Asiakas.Puhelin;
                        customerAddressTextBox.Text = reservation.Asiakas.Katuosoite;
                        customerPostalCodeTextBox.Text = reservation.Asiakas.Postinumero;
                        customerCityTextBox.Text = reservation.Asiakas.Postitoimipaikka;
                        customerEmailTextBox.Text = reservation.Asiakas.Sahkoposti;

                        invoicesListView.ItemsSource = new ObservableCollection<Reservation> { reservation };
                    }
                    else
                    {
                        MessageBox.Show("Määritetyllä ID varustettuja varauksia ei löytynyt.");
                        ClearCustomerFields();
                        ClearFinancialFields();
                        invoicesListView.ItemsSource = null;
                    }
                }
                else
                {
                    ClearCustomerFields();
                    ClearFinancialFields();
                    invoicesListView.ItemsSource = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Tapahtui virhe: {ex.Message}");
                ClearCustomerFields();
                ClearFinancialFields();
                invoicesListView.ItemsSource = null;
            }
        }

        //nätyä varauksen tiedot
        private void DisplayReservationDetails(Reservation reservation)
        {
            double totalExVAT = 0;
            double totalVAT = 0;

            if (reservation.Huone != null)
            {
                totalExVAT += reservation.Huone.Hinta;
                totalVAT += reservation.Huone.Hinta * (reservation.Huone.AlvProsentti / 100);
            }

            if (reservation.VarauksenPalvelut != null && reservation.VarauksenPalvelut.Count > 0)
            {
                foreach (var service in reservation.VarauksenPalvelut)
                {
                    double serviceTotal = service.Palvelu.PalvelunHinta * service.Kpl;
                    totalExVAT += serviceTotal;
                    totalVAT += serviceTotal * (service.Palvelu.AlvProsentti / 100);
                }
            }

            double totalIncludingVAT = totalExVAT + totalVAT;

            amountExVATTextBox.Text = totalExVAT.ToString("N2");
            vatTextBox.Text = totalVAT.ToString("N2");
            totalAmountTextBox.Text = totalIncludingVAT.ToString("N2");
        }

        //varauksen valinnan muutos ListView
        private void InvoicesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (invoicesListView.SelectedItem is Reservation selectedReservation)
            {
                customerNameTextBox.Text = selectedReservation.Asiakas.Nimi;
                customerPhoneTextBox.Text = selectedReservation.Asiakas.Puhelin;
                customerAddressTextBox.Text = selectedReservation.Asiakas.Katuosoite;
                customerPostalCodeTextBox.Text = selectedReservation.Asiakas.Postinumero;
                customerCityTextBox.Text = selectedReservation.Asiakas.Postitoimipaikka;
                customerEmailTextBox.Text = selectedReservation.Asiakas.Sahkoposti;

                DisplayReservationDetails(selectedReservation);
            }
            else
            {
                ClearCustomerFields();
                ClearFinancialFields();
            }
        }

        //taloustietokenttien tyhjennys
        private void ClearFinancialFields()
        {
            amountExVATTextBox.Text = "";
            vatTextBox.Text = "";
            totalAmountTextBox.Text = "";
        }

        //asiakastietokenttien tyhjennys
        private void ClearCustomerFields()
        {
            customerNameTextBox.Text = "";
            customerPhoneTextBox.Text = "";
            customerAddressTextBox.Text = "";
            customerPostalCodeTextBox.Text = "";
            customerCityTextBox.Text = "";
            customerEmailTextBox.Text = "";
        }

        //avaaa ikkunan, jossa on tilitiedot
        private void ViewInvoice_Click(object sender, RoutedEventArgs e)
        {
            if (savedInvoicesListView.SelectedItem is Invoice selectedInvoice)
            {
                InvoiceInfoList invoiceInfoWindow = new InvoiceInfoList(selectedInvoice, this); 
                invoiceInfoWindow.Show();
            }
            else
            {
                MessageBox.Show("Valitse lasku.");
            }
        }

        private void DeleteInvoice_Click(object sender, RoutedEventArgs e)
        {
            if (savedInvoicesListView.SelectedItem is Invoice selectedInvoice)
            {
                DeleteInvoiceFromDatabase(selectedInvoice.Laskunumero);
                LoadSavedInvoices();
                MessageBox.Show("Tili on poistettu.");
            }
        }

        //poista lasku tietokannasta
        private void DeleteInvoiceFromDatabase(int invoiceId)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "DELETE FROM lasku WHERE laskuID = @invoiceId";
                var command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@invoiceId", invoiceId);
                command.ExecuteNonQuery();
            }
        }
    }
}