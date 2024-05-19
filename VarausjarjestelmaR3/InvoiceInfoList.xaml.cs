using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
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
    /// Interaction logic for InvoiceInfoList.xaml
    /// </summary>
    public partial class InvoiceInfoList : Window
    {
        private string connectionString = "Server=127.0.0.1; Port=3306; User ID=opiskelija; Pwd=opiskelija1; Database=vuokratoimistot;";
        private Invoice currentInvoice;
        private Invoices invoicesControl;
        public ObservableCollection<Room> Huoneet { get; set; }
        public ObservableCollection<Service> Palvelut { get; set; }
        public ObservableCollection<string> Laskutustavat { get; set; }

        public InvoiceInfoList(Invoice invoice, Invoices invoicesControl)
        {
            InitializeComponent();
            currentInvoice = invoice;
            this.invoicesControl = invoicesControl;
            DataContext = this;
            ObservableCollection<Invoice> invoiceDetails = new ObservableCollection<Invoice> { invoice };
            invoiceDetailsListView.ItemsSource = invoiceDetails;
            LoadHuoneet();
            LoadPalvelut();
            LoadLaskutustavat();
            UpdateTotalAmount();

        }

        private void LoadHuoneet()
        {
            Huoneet = new ObservableCollection<Room>();
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM huoneet";
                var command = new MySqlCommand(query, connection);
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Huoneet.Add(new Room
                    {
                        HuoneenNumeroID = reader.GetInt32("huoneen_numeroID"),
                        Nimi = reader.GetString("nimi"),
                        Hinta = reader.GetDouble("hinta"),
                        AlvProsentti = reader.GetDouble("alv_prosentti"),
                        HloMaara = reader.GetInt32("hlo_maara"),

                    });
                }
            }
        }

        private void LoadPalvelut()
        {
            Palvelut = new ObservableCollection<Service>();
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM palvelu";
                var command = new MySqlCommand(query, connection);
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Palvelut.Add(new Service
                    {
                        PalveluID = reader.GetInt32("palveluID"),
                        Tuote = reader.GetString("tuote"),
                        PalvelunHinta = reader.GetDouble("palvelun_hinta"),
                        AlvProsentti = reader.GetDouble("alv_prosentti"),
                        Maara = reader.GetInt32("maara"),

                    });
                }
            }
        }


        private void LoadLaskutustavat()
        {
            Laskutustavat = new ObservableCollection<string> { "Sähköpostilasku", "Paperilasku" };
        }

        //laskun loppusumman päivitysmenetelmä
        private void UpdateTotalAmount()
        {
            double totalExVAT = 0;
            double totalVAT = 0;

            Console.WriteLine("Yhteenvetopäivitys: Beginning");

            //laske ALV:ton summa ja huoneen ALV
            if (currentInvoice.Varaus.Huone != null)
            {
                double huoneHinta = currentInvoice.Varaus.Huone.Hinta;
                double huoneAlv = currentInvoice.Varaus.Huone.AlvProsentti / 100;
                totalExVAT += huoneHinta;
                totalVAT += huoneHinta * huoneAlv;
                Console.WriteLine($"Huone: {currentInvoice.Varaus.Huone.Nimi}, Hinta: {huoneHinta}, ALV: {huoneAlv}");
            }

            //laske ALV:ton summa ja palveluiden ALV
            if (currentInvoice.Varaus.VarauksenPalvelut != null)
            {
                foreach (var service in currentInvoice.Varaus.VarauksenPalvelut)
                {
                    double serviceTotal = service.Palvelu.PalvelunHinta * service.Kpl;
                    double serviceAlv = service.Palvelu.AlvProsentti / 100;
                    totalExVAT += serviceTotal;
                    totalVAT += serviceTotal * serviceAlv;
                    Console.WriteLine($"Palvelu: {service.Palvelu.Tuote}, Hinta: {service.Palvelu.PalvelunHinta}, Kpl: {service.Kpl}, ALV: {serviceAlv}");
                }
            }

            //laske kokonaismäärä ALV mukaan lukien
            double totalIncludingVAT = totalExVAT + totalVAT;

            //päivitä nykyinen lasku
            currentInvoice.VerotonSumma = totalExVAT;
            currentInvoice.AlvEuroina = totalVAT;
            currentInvoice.Loppusumma = totalIncludingVAT;

            Console.WriteLine($"Kokonaismäärä ilman ALV: {totalExVAT}, ALV: {totalVAT}, Loppusumma: {totalIncludingVAT}");

            //päivitä arvot käyttöliittymässä
            totalAmountTextBox.Text = totalIncludingVAT.ToString("N2");
            invoiceDetailsListView.Items.Refresh();
        }

        private void SaveInvoiceToDatabase(Invoice invoice)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        string query = @"UPDATE lasku SET laskutustapa = @Laskutustapa, alv_euroina = @AlvEuroina, veroton_summa = @VerotonSumma, 
                                       loppusumma = @Loppusumma, asiakasID = @AsiakasID, varausID = @VarausID WHERE laskuID = @Laskunumero";
                        var command = new MySqlCommand(query, connection, transaction);
                        command.Parameters.AddWithValue("@Laskutustapa", invoice.Laskutustapa);
                        command.Parameters.AddWithValue("@AlvEuroina", invoice.AlvEuroina);
                        command.Parameters.AddWithValue("@VerotonSumma", invoice.VerotonSumma);
                        command.Parameters.AddWithValue("@Loppusumma", invoice.Loppusumma);
                        command.Parameters.AddWithValue("@AsiakasID", invoice.AsiakasID);
                        command.Parameters.AddWithValue("@VarausID", invoice.VarausID);
                        command.Parameters.AddWithValue("@Laskunumero", invoice.Laskunumero);
                        command.ExecuteNonQuery();

                        string updateVarausQuery = @"UPDATE asiakkaan_varaus SET varaus_alkaa = @VarausAlkaa, varaus_paattyy = @VarausPaattyy, 
                                                   huoneen_numeroID = @HuoneenNumeroID WHERE varausID = @VarausID";
                        var varausCommand = new MySqlCommand(updateVarausQuery, connection, transaction);
                        varausCommand.Parameters.AddWithValue("@VarausAlkaa", invoice.Varaus.VarausAlkaa);
                        varausCommand.Parameters.AddWithValue("@VarausPaattyy", invoice.Varaus.VarausPaattyy);
                        varausCommand.Parameters.AddWithValue("@HuoneenNumeroID", invoice.Varaus.Huone.HuoneenNumeroID);
                        varausCommand.Parameters.AddWithValue("@VarausID", invoice.VarausID);
                        varausCommand.ExecuteNonQuery();

                        //kaikkien palvelujen poistaminen
                        string deleteServicesQuery = @"DELETE FROM varauksen_palvelut WHERE varausID = @VarausID";
                        var deleteServicesCommand = new MySqlCommand(deleteServicesQuery, connection, transaction);
                        deleteServicesCommand.Parameters.AddWithValue("@VarausID", invoice.VarausID);
                        deleteServicesCommand.ExecuteNonQuery();

                        //palvelujen lisääminen
                        foreach (var service in invoice.Varaus.VarauksenPalvelut)
                        {
                            string insertServiceQuery = @"INSERT INTO varauksen_palvelut (palveluID, kpl, varausID) VALUES (@PalveluID, @Kpl, @VarausID)";
                            var serviceCommand = new MySqlCommand(insertServiceQuery, connection, transaction);
                            serviceCommand.Parameters.AddWithValue("@PalveluID", service.Palvelu.PalveluID);
                            serviceCommand.Parameters.AddWithValue("@Kpl", service.Kpl);
                            serviceCommand.Parameters.AddWithValue("@VarausID", invoice.VarausID);
                            serviceCommand.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show($"Tapahtui virhe: {ex.Message}");
                    }
                }
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SaveChanges_Click(object sender, RoutedEventArgs e)
        {
            SaveInvoiceToDatabase(currentInvoice);
            LoadInvoiceDetails(currentInvoice.Laskunumero);
            invoicesControl.LoadSavedInvoices();
            MessageBox.Show("Muutokset tallennettu.");
        }

        private void LoadInvoiceDetails(int laskunumero)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                //peruskysely tilin ja siihen liittyvien entiteettien tietojen hakemiseksi
                string mainQuery = @" SELECT l.laskuID, l.laskutustapa, l.alv_euroina, l.veroton_summa, l.loppusumma,a.asiakasID, a.nimi AS asiakasNimi, a.puhelin,
                                   a.katuosoite, a.postinumero, a.postitoimipaikka, a.sahkoposti, av.varausID, av.varaus_alkaa, av.varaus_paattyy, av.huoneen_numeroID,
                                   av.varauspvm, av.lisatiedot, h.nimi AS huoneNimi, h.hinta AS huoneHinta, h.alv_prosentti AS huoneAlvProsentti, h.hlo_maara AS huoneHloMaara
                                   FROM lasku l
                                   INNER JOIN asiakkaan_varaus av ON l.varausID = av.varausID
                                   INNER JOIN asiakas a ON l.asiakasID = a.asiakasID
                                   INNER JOIN huoneet h ON av.huoneen_numeroID = h.huoneen_numeroID
                                   WHERE l.laskuID = @Laskunumero;";

                //liitännäispalveluja koskevia tietoja koskeva lisäpyyntö
                string servicesQuery = @"SELECT vp.as_palveluvarauksenID, vp.palveluID, vp.kpl, p.tuote AS palveluTuote, p.palvelun_hinta 
                                       AS palveluHinta, p.alv_prosentti AS palveluAlvProsentti
                                       FROM varauksen_palvelut vp
                                       INNER JOIN palvelu p ON vp.palveluID = p.palveluID
                                       WHERE vp.varausID = @VarausID;";

                //suorita pääkysely ja täytä nykyinen lasku
                using (var mainCommand = new MySqlCommand(mainQuery, connection))
                {
                    mainCommand.Parameters.AddWithValue("@Laskunumero", laskunumero);
                    using (var mainReader = mainCommand.ExecuteReader())
                    {
                        if (mainReader.Read())
                        {
                            currentInvoice = new Invoice
                            {
                                Laskunumero = mainReader.GetInt32("laskuID"),
                                Laskutustapa = mainReader.GetString("laskutustapa"),
                                VerotonSumma = mainReader.GetDouble("veroton_summa"),
                                AlvEuroina = mainReader.GetDouble("alv_euroina"),
                                Loppusumma = mainReader.GetDouble("loppusumma"),
                                AsiakasID = mainReader.GetInt32("asiakasID"),
                                VarausID = mainReader.GetInt32("varausID"),
                                Asiakas = new Customer
                                {
                                    AsiakasID = mainReader.GetInt32("asiakasID"),
                                    Nimi = mainReader.GetString("asiakasNimi"),
                                    Puhelin = mainReader.GetString("puhelin"),
                                    Katuosoite = mainReader.GetString("katuosoite"),
                                    Postinumero = mainReader.GetString("postinumero"),
                                    Postitoimipaikka = mainReader.GetString("postitoimipaikka"),
                                    Sahkoposti = mainReader.GetString("sahkoposti")
                                },
                                Varaus = new Reservation
                                {
                                    VarausID = mainReader.GetInt32("varausID"),
                                    VarausAlkaa = mainReader.GetDateTime("varaus_alkaa"),
                                    VarausPaattyy = mainReader.GetDateTime("varaus_paattyy"),
                                    Varauspaiva = mainReader.GetDateTime("varauspvm"),
                                    Lisatiedot = mainReader.GetString("lisatiedot"),
                                    Huone = new Room
                                    {
                                        HuoneenNumeroID = mainReader.GetInt32("huoneen_numeroID"),
                                        Nimi = mainReader.GetString("huoneNimi"),
                                        Hinta = mainReader.GetDouble("huoneHinta"),
                                        AlvProsentti = mainReader.GetDouble("huoneAlvProsentti"),
                                        HloMaara = mainReader.GetInt32("huoneHloMaara")
                                    }
                                }
                            };
                        }
                    }
                }

                //aseta varausID-parametri palvelupyyntöä varten
                int varausID = currentInvoice.VarausID;

                //suorita palvelukysely ja täytä palvelut
                using (var servicesCommand = new MySqlCommand(servicesQuery, connection))
                {
                    servicesCommand.Parameters.AddWithValue("@VarausID", varausID);
                    using (var servicesReader = servicesCommand.ExecuteReader())
                    {
                        currentInvoice.Varaus.VarauksenPalvelut = new ObservableCollection<ReservationService>();

                        while (servicesReader.Read())
                        {
                            var service = new ReservationService
                            {
                                PalveluvarausID = servicesReader.GetInt32("as_palveluvarauksenID"),
                                Kpl = servicesReader.GetInt32("kpl"),
                                Palvelu = new Service
                                {
                                    PalveluID = servicesReader.GetInt32("palveluID"),
                                    Tuote = servicesReader.GetString("palveluTuote"),
                                    PalvelunHinta = servicesReader.GetDouble("palveluHinta"),
                                    AlvProsentti = servicesReader.GetDouble("palveluAlvProsentti")
                                }
                            };

                            currentInvoice.Varaus.VarauksenPalvelut.Add(service);
                        }
                    }
                }

                //päivitä käyttöliittymä ja laske kokonaissumma
                invoiceDetailsListView.ItemsSource = new ObservableCollection<Invoice> { currentInvoice };
                UpdateTotalAmount();
            }
        }

        //palvelun lisääminen
        private void AddService_Click(object sender, RoutedEventArgs e)
        {
            if (currentInvoice.Varaus == null)
            {
                MessageBox.Show("Varaus ei ole valittu.");
                return;
            }

            var newService = new ReservationService
            {
                Palvelu = Palvelut.FirstOrDefault(),
                Kpl = 1,
                VarausID = currentInvoice.Varaus.VarausID
            };

            currentInvoice.Varaus.VarauksenPalvelut.Add(newService);
            UpdateTotalAmount();
            invoiceDetailsListView.Items.Refresh();
        }

        //palvelun poisto
        private void RemoveAllServices_Click(object sender, RoutedEventArgs e)
        {
            currentInvoice.Varaus.VarauksenPalvelut.Clear();
            UpdateTotalAmount();
        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {
            Invoice invoice = currentInvoice;

            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string fileName = $"Invoice_{invoice.Laskunumero}.txt";
            string filePath = System.IO.Path.Combine(desktopPath, fileName);
            

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                //Otsikko & laskunumero
                writer.WriteLine($"LASKU");
                writer.WriteLine();
                writer.WriteLine($"Laskunumero: {invoice.Laskunumero}");
                writer.WriteLine($"Laskutuspäivä: {DateTime.Now.ToString("dd.MM.yyyy")}");
                writer.WriteLine($"Eräpäivä: {DateTime.Now.AddDays(30).ToString("dd.MM.yyyy")}");

                //Laskutustapa
                writer.WriteLine($"Laskutustapa: {invoice.Laskutustapa}");
                writer.WriteLine();
                writer.WriteLine("----------------------------------------------------------------");
                writer.WriteLine();

                //Laskutettavan tiedot
                writer.WriteLine($"Laskun saaja: ");
                writer.WriteLine($"{invoice.Asiakas.Nimi}");
                writer.WriteLine($"{invoice.Asiakas.Katuosoite}");
                writer.Write($"{invoice.Asiakas.Postinumero} ");
                writer.WriteLine($"{invoice.Asiakas.Postitoimipaikka}");
                writer.WriteLine($"{invoice.Asiakas.Sahkoposti}");
                writer.WriteLine();
                writer.WriteLine($"Asiakasnumero: {invoice.Asiakas.AsiakasID}");
                writer.WriteLine();
                writer.WriteLine("----------------------------------------------------------------");
                writer.WriteLine();

                //Laskuttajan tiedot
                writer.WriteLine($"Laskuttaja: ");
                writer.WriteLine($"Vuokratoimistot Oy");
                writer.WriteLine($"Karjalankatu 3");
                writer.WriteLine($"80200 Joensuu");
                writer.WriteLine($"vuokratoimistot@vuokratoimistot.fi");
                writer.WriteLine();
                writer.WriteLine("----------------------------------------------------------------");
                writer.WriteLine();

                //Vuokratut tilat ja palvelut
                writer.WriteLine($"Varausnumero: {invoice.VarausID}");
                writer.WriteLine();

                writer.WriteLine($"Varauksen ajankohta: {invoice.Varaus.VarausAlkaa.ToString("dd.MM.yyyy")}-{invoice.Varaus.VarausPaattyy.ToString("dd.MM.yyyy")}");
                writer.WriteLine();

                writer.WriteLine($"Vuokrattu tila: ");
                writer.WriteLine($"{invoice.Varaus.Huone}, hinta/vrk: {invoice.Varaus.Huone.Hinta} €, alv: {invoice.Varaus.Huone.AlvProsentti} %");
                writer.WriteLine();

                var varauksenPalvelut = invoice.Varaus.VarauksenPalvelut;
                if (varauksenPalvelut.Count > 0)
                {
                    writer.WriteLine("Vuokratut välineet ja palvelut: ");
                    for (int i = 0; i < varauksenPalvelut.Count; i++)
                    {
                        var palvelu = varauksenPalvelut[i];
                        writer.WriteLine($"{palvelu.Palvelu.Tuote} {palvelu.Kpl} kpl, kappalehinta/vrk: {palvelu.Palvelu.PalvelunHinta} €, alv: {palvelu.Palvelu.AlvProsentti} %");

                    }
                    writer.WriteLine();
                }

                //Veroton summa, vero, loppusumma

                writer.WriteLine($"Veroton summa: {invoice.VerotonSumma} €");
                writer.WriteLine($"ALV: {invoice.AlvEuroina} €");
                writer.WriteLine();
                writer.WriteLine($"MAKSETTAVA SUMMA: {invoice.Loppusumma} €");
                writer.WriteLine();
                writer.WriteLine("----------------------------------------------------------------"); ;
                writer.WriteLine();

                MessageBox.Show($"Lasku tallennettu työpöydälle tekstitiedostoon nimellä: {fileName}", "Tallennus onnistui", MessageBoxButton.OK, MessageBoxImage.Information);  //Confirmation box
            }
        }
    }
}
