using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Security.Policy;
using MySqlConnector;
using VarausjarjestelmaR3.Classes;
using System.Windows.Media;
using System.Data;

namespace VarausjarjestelmaR3
{
    internal class Repository
    {
        private string connectionString = "Server=127.0.0.1; Port=3306; User ID=opiskelija; Pwd=opiskelija1; Database=vuokratoimistot;";

        public static void PrintVisual(FrameworkElement pageToPrint)             //Metodi, joka avaa printti-ikkunan 
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                printDialog.PrintVisual(pageToPrint, "Print Information");
            }


            //Koodi jolla luodaan printti elementistä:
            //FrameworkElement pageToPrint = GetPageToPrint();
            //Nimeä tulostettava elementti, esim window tässä tapauksessa, tähän


            //Tämä lähettää sen PrintVisual-metodille, joka avaa tulostusikkunan.
            //Repository.PrintVisual(pageToPrint);
        }


        //Metodeja tietojen hakuun tietokannasta.

        //Haetaan yritys ID-numeron perusteella:
        public Company GetCompany(int yritysID)
        {
            Company yritys = new Company();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("SELECT * FROM yritys WHERE yritysID=@yritysID", conn);
                cmd.Parameters.AddWithValue("@yritysID", yritysID);

                var dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    yritys = new Company
                    {
                        YritysID = dr.GetInt32("yritysID"),
                        YritysNimi = dr.GetString("yrityksen_nimi"),
                        Katuosoite = dr.GetString("katuosoite"),
                        Postinumero = dr.GetString("postinumero"),
                        Postitoimipaikka = dr.GetString("postitoimipaikka"),
                        Sahkoposti = dr.GetString("email"),
                        Puhelin = dr.GetString("puhelin"),
                    };
                }

                return yritys;
            }
        }

        //Haetaan kaikki tietokannassa olevat yritykset listalle:
        public ObservableCollection<Company> GetAllCompanies()
        {
            var yritykset = new ObservableCollection<Company>();
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("SELECT * FROM yritys", conn);

                var dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    yritykset.Add(new Company
                    {
                        YritysID = dr.GetInt32("yritysID"),
                        YritysNimi = dr.GetString("yrityksen_nimi"),
                        Katuosoite = dr.GetString("katuosoite"),
                        Postinumero = dr.GetString("postinumero"),
                        Postitoimipaikka = dr.GetString("postitoimipaikka"),
                        Sahkoposti = dr.GetString("email"),
                        Puhelin = dr.GetString("puhelin"),
                    });
                }
            }

            return yritykset;
        }

        //Haetaan toimipiste ID-numeron perusteella:
        public Office GetOffice(int toimipisteID)
        {
            Office toimipiste = new Office();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("SELECT * FROM toimipiste WHERE toimipisteID=@toimipisteID", conn);
                cmd.Parameters.AddWithValue("@toimipisteID", toimipisteID);

                var dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    toimipiste = new Office
                    {
                        ToimipisteID = dr.GetInt32("toimipisteID"),
                        Paikkakunta = dr.GetString("paikkakunta"),
                        ToimipisteNimi = dr.GetString("toimipiste_nimi"),
                        Katuosoite = dr.GetString("katuosoite"),
                        Postinumero = dr.GetString("postinumero"),
                        Postitoimipaikka = dr.GetString("postitoimipaikka"),
                        Puhelin = dr.GetString("puhelin"),
                        Yritys = GetCompany(dr.GetInt32("yritysID")),
                    };
                }

                return toimipiste;
            }
        }

        //Haetaan kaikki toimipisteet listalle:
        public ObservableCollection<Office> GetAllOffices()
        {
            var toimipisteet = new ObservableCollection<Office>();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("SELECT * FROM toimipiste", conn);

                var dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    toimipisteet.Add(new Office
                    {
                        ToimipisteID = dr.GetInt32("toimipisteID"),
                        Paikkakunta = dr.GetString("paikkakunta"),
                        ToimipisteNimi = dr.GetString("toimipiste_nimi"),
                        Katuosoite = dr.GetString("katuosoite"),
                        Postinumero = dr.GetString("postinumero"),
                        Postitoimipaikka = dr.GetString("postitoimipaikka"),
                        Puhelin = dr.GetString("puhelin"),
                        Yritys = GetCompany(dr.GetInt32("yritysID")),
                    });
                }
            }

            return toimipisteet;
        }

        //Haetaan vuokrattava huone ID-numeron perusteella:
        public Room GetRoom(int huoneID)
        {
            Room huone = new Room();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("SELECT * FROM huoneet WHERE huoneen_numeroID=@huoneID", conn);
                cmd.Parameters.AddWithValue("@huoneID", huoneID);

                var dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    huone = new Room
                    {
                        HuoneenNumeroID = dr.GetInt32("huoneen_numeroID"),
                        Nimi = dr.GetString("nimi"),
                        Hinta = dr.GetDouble("hinta"),
                        AlvProsentti = dr.GetDouble("alv_prosentti"),
                        HloMaara = dr.GetInt32("hlo_maara"),
                        Toimipiste = GetOffice(dr.GetInt32("toimipisteID")),
                    };
                }

                return huone;
            }
        }

        //Haetaan kaikki vuokrattavat huoneet listalle:
        public ObservableCollection<Room> GetAllRooms()
        {
            var huoneet = new ObservableCollection<Room>();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("SELECT * FROM huoneet", conn);

                var dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    huoneet.Add(new Room
                    {
                        HuoneenNumeroID = dr.GetInt32("huoneen_numeroID"),
                        Nimi = dr.GetString("nimi"),
                        Hinta = dr.GetDouble("hinta"),
                        AlvProsentti = dr.GetDouble("alv_prosentti"),
                        HloMaara = dr.GetInt32("hlo_maara"),
                        Toimipiste = GetOffice(dr.GetInt32("toimipisteID")),
                    });
                }
            }

            return huoneet;
        }

        //Haetaan palvelu ID-numeron perusteella:
        public Service GetService(int palveluID)
        {
            Service palvelu = new Service();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("SELECT * FROM palvelu WHERE palveluID=@palveluID", conn);
                cmd.Parameters.AddWithValue("@palveluID", palveluID);

                var dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    palvelu = new Service
                    {
                        PalveluID = dr.GetInt32("palveluID"),
                        Tuote = dr.GetString("tuote"),
                        PalvelunHinta = dr.GetDouble("palvelun_hinta"),
                        AlvProsentti = dr.GetDouble("alv_prosentti"),
                        Maara = dr.GetInt32("maara"),
                        Toimipiste = GetOffice(dr.GetInt32("toimipisteID")),
                    };
                }
            }

            return palvelu;
        }

        //Haetaan kaikki palvelut listalle:
        public ObservableCollection<Service> GetAllServices(ObservableCollection<Office> offices = null)
        {
            var palvelut = new ObservableCollection<Service>();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("SELECT * FROM palvelu", conn);

                var dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    palvelut.Add(new Service
                    {
                        PalveluID = dr.GetInt32("palveluID"),
                        Tuote = dr.GetString("tuote"),
                        PalvelunHinta = dr.GetDouble("palvelun_hinta"),
                        AlvProsentti = dr.GetDouble("alv_prosentti"),
                        Maara = dr.GetInt32("maara"),
                        Toimipiste = offices?.Any(x => x.ToimipisteID == dr.GetInt32("toimipisteID")) == true ?
                        offices.First(x => x.ToimipisteID == dr.GetInt32("toimipisteID")) : GetOffice(dr.GetInt32("toimipisteID")),
                    });
                }
            }

            return palvelut;
        }

        //Haetaan työntekijä ID-numeron perusteella:
        public Employee GetEmployee(int tyontekijaID)
        {
            Employee tyontekija = new Employee();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("SELECT * FROM tyontekija WHERE tyontekijaID=@tyontekijaID", conn);
                cmd.Parameters.AddWithValue("@TyontekijaID", tyontekijaID);

                var dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    tyontekija = new Employee
                    {
                        TyontekijaID = dr.GetInt32("tyontekijaID"),
                        Nimi = dr.GetString("nimi"),
                        Osoite = dr.GetString("osoite"),
                        Puhelin = dr.GetString("puhelin"),
                        KayttajaID = dr.GetString("kayttajaID"),
                        Salasana = dr.GetString("salasana"),
                        KayttoOikeus = dr.GetInt32("kaytto_oikeus"),
                        Yritys = GetCompany(dr.GetInt32("yritysID")),
                    };
                }
            }

            return tyontekija;
        }

        //Haetaan kaikki työntekijät listalle:
        public ObservableCollection<Employee> GetAllEmployees()
        {
            var tyontekijat = new ObservableCollection<Employee>();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("SELECT * FROM tyontekija", conn);

                var dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    tyontekijat.Add(new Employee
                    {
                        TyontekijaID = dr.GetInt32("tyontekijaID"),
                        Nimi = dr.GetString("nimi"),
                        Osoite = dr.GetString("osoite"),
                        Puhelin = dr.GetString("puhelin"),
                        KayttajaID = dr.GetString("kayttajaID"),
                        Salasana = dr.GetString("salasana"),
                        KayttoOikeus = dr.GetInt32("kaytto_oikeus"),
                        Yritys = GetCompany(dr.GetInt32("yritysID")),
                    });
                }
            }

            return tyontekijat;
        }

        //Haetaan asiakas ID-numeron perusteella:
        public Customer GetCustomer(int asiakasID)
        {
            Customer asiakas = new Customer();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("SELECT * FROM asiakas WHERE asiakasID=@asiakasID", conn);
                cmd.Parameters.AddWithValue("@AsiakasID", asiakasID);

                var dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    asiakas = new Customer
                    {
                        AsiakasID = dr.GetInt32("asiakasID"),
                        Nimi = dr.GetString("nimi"),
                        Puhelin = dr.GetString("puhelin"),
                        Katuosoite = dr.GetString("katuosoite"),
                        Postinumero = dr.GetString("postinumero"),
                        Postitoimipaikka = dr.GetString("postitoimipaikka"),
                        Sahkoposti = dr.GetString("sahkoposti"),
                    };
                }

                return asiakas;
            }
        }

        //Haetaan kaikki asiakkaat listalle:
        public ObservableCollection<Customer> GetAllCustomers()
        {
            var asiakkaat = new ObservableCollection<Customer>();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("SELECT * FROM asiakas", conn);

                var dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    asiakkaat.Add(new Customer
                    {
                        AsiakasID = dr.GetInt32("asiakasID"),
                        Nimi = dr.GetString("nimi"),
                        Puhelin = dr.GetString("puhelin"),
                        Katuosoite = dr.GetString("katuosoite"),
                        Postinumero = dr.GetString("postinumero"),
                        Postitoimipaikka = dr.GetString("postitoimipaikka"),
                        Sahkoposti = dr.GetString("sahkoposti"),
                    });
                }
            }

            return asiakkaat;
        }

        //Haetaan varaus ID-numeron perusteella:
        public Reservation GetReservation(int varausID)
        {
            Reservation varaus = new Reservation();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("SELECT * FROM asiakkaan_varaus WHERE varausID=@varausID", conn);
                cmd.Parameters.AddWithValue("@varausID", varausID);

                var dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    varaus = new Reservation
                    {
                        VarausID = dr.GetInt32("varausID"),
                        VarausAlkaa = dr.GetDateTime("varaus_alkaa"),
                        VarausPaattyy = dr.GetDateTime("varaus_paattyy"),
                        Varauspaiva = dr.GetDateTime("varauspvm"),
                        Huone = GetRoom(dr.GetInt32("huoneen_numeroID")),
                        Lisatiedot = dr.GetString("lisatiedot"),
                        Asiakas = GetCustomer(dr.GetInt32("asiakasID")),
                        Tyontekija = GetEmployee(dr.GetInt32("tyontekijaID")),
                        VarauksenPalvelut = GetReservationServices(varausID),
                    };
                }

                return varaus;
            }
        }

        //Haetaan kaikki varauksen palvelut listalle:
        public ObservableCollection<ReservationService> GetReservationServices(int varausID)
        {
            var varauksenPalvelut = new ObservableCollection<ReservationService>();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("SELECT * FROM varauksen_palvelut WHERE varausID=@varausID", conn);
                cmd.Parameters.AddWithValue("@varausID", varausID);

                using (var dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        varauksenPalvelut.Add(new ReservationService
                        {
                            PalveluvarausID = dr.GetInt32("as_palveluvarauksenID"),
                            Palvelu = GetService(dr.GetInt32("palveluID")),
                            Kpl = dr.GetInt32("kpl"),
                            VarausID = dr.GetInt32("varausID"),
                        });
                    }
                }

                return varauksenPalvelut;
            }
        }

        //Haetaan kaikki varaukset listalle:
        public ObservableCollection<Reservation> GetAllReservations()
        {
            var varaukset = new ObservableCollection<Reservation>();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM asiakkaan_varaus", conn);

                var dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    varaukset.Add(new Reservation
                    {
                        VarausID = dr.GetInt32("varausID"),
                        VarausAlkaa = dr.GetDateTime("varaus_alkaa"),
                        VarausPaattyy = dr.GetDateTime("varaus_paattyy"),
                        Varauspaiva = dr.GetDateTime("varauspvm"),
                        Huone = GetRoom(dr.GetInt32("huoneen_numeroID")),
                        Lisatiedot = dr.GetString("lisatiedot"),
                        Asiakas = GetCustomer(dr.GetInt32("asiakasID")),
                        Tyontekija = GetEmployee(dr.GetInt32("tyontekijaID")),
                        VarauksenPalvelut = GetReservationServices(dr.GetInt32("varausID")),
                    });
                }
            }

            return varaukset;
        }
        public ObservableCollection<Reservation> GetAllReservationsforEmployee(int EmployeeId)
        {
            var varaukset = new ObservableCollection<Reservation>();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM asiakkaan_varaus where tyontekijaID = @tyontekijaID", conn);
                cmd.Parameters.AddWithValue("@tyontekijaID", Convert.ToInt32(EmployeeId));

                var dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    varaukset.Add(new Reservation
                    {
                        VarausID = dr.GetInt32("varausID"),
                        VarausAlkaa = dr.GetDateTime("varaus_alkaa"),
                        VarausPaattyy = dr.GetDateTime("varaus_paattyy"),
                        Varauspaiva = dr.GetDateTime("varauspvm"),
                        Huone = GetRoom(dr.GetInt32("huoneen_numeroID")),
                        Lisatiedot = dr.GetString("lisatiedot"),
                        Asiakas = GetCustomer(dr.GetInt32("asiakasID")),
                        Tyontekija = GetEmployee(dr.GetInt32("tyontekijaID")),
                        VarauksenPalvelut = GetReservationServices(dr.GetInt32("varausID")),
                    });
                }
            }

            return varaukset;
        }


        public ObservableCollection<Reservation> GetAllReservationsforCustomer (int CustomerID)
            {
            var varaukset = new ObservableCollection<Reservation>();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM asiakkaan_varaus where asiakasID = @asiakasID", conn);
                cmd.Parameters.AddWithValue("@asiakasID", Convert.ToInt32(CustomerID));

                var dr = cmd.ExecuteReader();

                while (dr.Read())
                    {
                    varaukset.Add(new Reservation
                        {
                        VarausID = dr.GetInt32("varausID"),
                        VarausAlkaa = dr.GetDateTime("varaus_alkaa"),
                        VarausPaattyy = dr.GetDateTime("varaus_paattyy"),
                        Varauspaiva = dr.GetDateTime("varauspvm"),
                        Huone = GetRoom(dr.GetInt32("huoneen_numeroID")),
                        Lisatiedot = dr.GetString("lisatiedot"),
                        Asiakas = GetCustomer(dr.GetInt32("asiakasID")),
                        Tyontekija = GetEmployee(dr.GetInt32("tyontekijaID")),
                        VarauksenPalvelut = GetReservationServices(dr.GetInt32("varausID")),
                        });
                    }
                }

            return varaukset;
            }


        //Haetaan lasku ID-numeron perusteella:
        public Invoice GetInvoice(int laskunumero)
        {
            Invoice lasku = new Invoice();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("SELECT * FROM lasku WHERE laskuID=@laskuID", conn);
                cmd.Parameters.AddWithValue("@laskuID", laskunumero);

                var dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    lasku = new Invoice
                    {
                        Laskunumero = dr.GetInt32("laskuID"),
                        Laskutustapa = dr.GetString("laskutustapa"),
                        VerotonSumma = dr.GetDouble("veroton_summa"),
                        AlvEuroina = dr.GetDouble("alv_euroina"),
                        Loppusumma = dr.GetDouble("loppusumma"),
                        AsiakasID = dr.GetInt32("asiakasID"),
                        Asiakas = GetCustomer(dr.GetInt32("asiakasID")),
                        Varaus = GetReservation(dr.GetInt32("varausID")),
                    };
                }

                return lasku;
            }
        }

        //Haetaan kaikki laskut listalle:
        public ObservableCollection<Invoice> GetAllInvoices()
        {
            var laskut = new ObservableCollection<Invoice>();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("SELECT * FROM lasku", conn);

                var dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    //laskut.Add(new Invoice
                    var invoice = new Invoice
                    {
                        Laskunumero = dr.GetInt32("laskuID"),
                        Laskutustapa = dr.GetString("laskutustapa"),
                        VerotonSumma = dr.GetDouble("veroton_summa"),
                        AlvEuroina = dr.GetDouble("alv_euroina"),
                        Loppusumma = dr.GetDouble("loppusumma"),
                        AsiakasID = dr.GetInt32("asiakasID"),
                        Varaus = GetReservation(dr.GetInt32("varausID")),
                        Asiakas = GetCustomer(dr.GetInt32("asiakasID")),
                    };


                    laskut.Add(invoice);
                }
            }

            return laskut;
        }

        public void AddNewCustomer(Customer newCustomer)   //lisätään uusi asiakas tietokantaan
        {

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("INSERT INTO asiakas (nimi, puhelin, katuosoite, postinumero, postitoimipaikka, sahkoposti) VALUES(@asiakasID, @nimi, @puhelin, @katuosoite, @postinumero, @postitoimipaikka, @sahkoposti)", conn);

                cmd.Parameters.AddWithValue("@nimi", newCustomer.Nimi);
                cmd.Parameters.AddWithValue("@puhelin", newCustomer.Puhelin);
                cmd.Parameters.AddWithValue("@katuosoite", newCustomer.Katuosoite);
                cmd.Parameters.AddWithValue("@postinumero", newCustomer.Postinumero);
                cmd.Parameters.AddWithValue("@postitoimipaikka", newCustomer.Postitoimipaikka);
                cmd.Parameters.AddWithValue("@sahkoposti", newCustomer.Sahkoposti);

                cmd.ExecuteNonQuery();

            }
        }
        public void UpdateCustomer(Customer customer) //asiakkaan tietojen päivitys tietokantaan
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                
                MySqlCommand cmd = new MySqlCommand("UPDATE asiakas SET asiakasID=@asiakasID, nimi=@nimi, puhelin=@puhelin, katuosoite=@katuosoite, postinumero=@postinumero, postitoimipaikka=@postitoimipaikka, sahkoposti=@sahkoposti  WHERE asiakasID=@asiakasId", conn);
                cmd.Parameters.AddWithValue("@asiakasID", customer.AsiakasID);
                cmd.Parameters.AddWithValue("@nimi", customer.Nimi);
                cmd.Parameters.AddWithValue("@puhelin", customer.Puhelin);
                cmd.Parameters.AddWithValue("@katuosoite", customer.Katuosoite);
                cmd.Parameters.AddWithValue("@postinumero", customer.Postinumero);
                cmd.Parameters.AddWithValue("@postitoimipaikka", customer.Postitoimipaikka);
                cmd.Parameters.AddWithValue("@sahkoposti", customer.Sahkoposti);

                cmd.ExecuteNonQuery();
            }
        }
        public void DeleteCustomer(Customer customer) //poistetaan asiakas tietokannasta
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("DELETE FROM asiakas WHERE asiakasID=@asiakasID", conn);
                cmd.Parameters.AddWithValue("@asiakasID", customer.AsiakasID);
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Palvelun poisto
        /// </summary>
        /// <param name="service"></param>
        public void RemoveService(Service service)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("DELETE FROM palvelu WHERE palveluID=@PalveluID", conn);
                cmd.Parameters.AddWithValue("@PalveluID", service.PalveluID);

                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Palveluiden tallentaminen
        /// </summary>
        /// <param name="services"></param>
        public void SaveServices(ObservableCollection<Service> services)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                foreach (var service in services)
                {
                    if (service.PalveluID == 0)
                    {
                        MySqlCommand cmd = new MySqlCommand("INSERT INTO palvelu (tuote, palvelun_hinta, alv_prosentti, maara, toimipisteID) VALUES (@Tuote, @PalvelunHinta, @AlvProsentti, @Maara, @ToimipisteID)", conn);
                        cmd.Parameters.AddWithValue("@Tuote", service.Tuote);
                        cmd.Parameters.AddWithValue("@PalvelunHinta", service.PalvelunHinta);
                        cmd.Parameters.AddWithValue("@AlvProsentti", service.AlvProsentti);
                        cmd.Parameters.AddWithValue("@Maara", service.Maara);
                        cmd.Parameters.AddWithValue("@ToimipisteID", service.Toimipiste.ToimipisteID);

                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        MySqlCommand cmd = new MySqlCommand("UPDATE palvelu SET tuote=@Tuote, palvelun_hinta=@PalvelunHinta, alv_prosentti=@AlvProsentti, maara=@Maara, toimipisteID=@ToimipisteID WHERE palveluID=@PalveluID", conn);
                        cmd.Parameters.AddWithValue("@Tuote", service.Tuote);
                        cmd.Parameters.AddWithValue("@PalvelunHinta", service.PalvelunHinta);
                        cmd.Parameters.AddWithValue("@AlvProsentti", service.AlvProsentti);
                        cmd.Parameters.AddWithValue("@Maara", service.Maara);
                        cmd.Parameters.AddWithValue("@ToimipisteID", service.Toimipiste.ToimipisteID);
                        cmd.Parameters.AddWithValue("@PalveluID", service.PalveluID);


                        cmd.ExecuteNonQuery();
                    }

                }
            }
        }

        public void SaveInvoice(Invoice invoice) //menetelmä laskutietojen tallentamiseksi tietokanta
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand("INSERT INTO lasku (laskuID, laskutustapa, veroton_summa, alv_euroina, loppusumma, asiakasID, varausID) VALUES (@Laskunumero, @Laskutustapa, @VerotonSumma, @AlvEuroina, @Loppusumma, @AsiakasID, @VarausID)", conn);

                cmd.Parameters.AddWithValue("@Laskunumero", invoice.Laskunumero);
                cmd.Parameters.AddWithValue("@Laskutustapa", invoice.Laskutustapa);
                cmd.Parameters.AddWithValue("@VerotonSumma", invoice.VerotonSumma);
                cmd.Parameters.AddWithValue("@AlvEuroina", invoice.AlvEuroina);
                cmd.Parameters.AddWithValue("@Loppusumma", invoice.Loppusumma);
                cmd.Parameters.AddWithValue("@AsiakasID", invoice.AsiakasID);
                cmd.Parameters.AddWithValue("@VarausID", invoice.VarausID);

                cmd.ExecuteNonQuery();
            }
        }

        //menetelmä tilinumeron automaattista lisäämistä varten 
        public int GetMaxInvoiceNumber()
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT MAX(laskuID) FROM lasku", conn);
                object result = cmd.ExecuteScalar();
                if (result != DBNull.Value)
                    return Convert.ToInt32(result) + 1;
                return 1; //jos merkintöjä ei ole, numerointi aloitetaan numerosta 1
            }
        }

        public void DeleteInvoice(int invoiceNumber)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand("DELETE FROM lasku WHERE laskuID = @Laskunumero", conn);
                cmd.Parameters.AddWithValue("@Laskunumero", invoiceNumber);
                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Hakee paikkakunnan toimipisteet
        /// </summary>
        /// <param name="paikkakunta"></param>
        /// <returns></returns>
        public ObservableCollection<Office> GetAllOfficesByMunicipality(string paikkakunta)
        {
            var toimipisteet = new ObservableCollection<Office>();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("SELECT * FROM toimipiste WHERE paikkakunta=@paikkakunta", conn);
                cmd.Parameters.AddWithValue("@paikkakunta", paikkakunta);


                var dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    toimipisteet.Add(new Office
                    {
                        ToimipisteID = dr.GetInt32("toimipisteID"),
                        Paikkakunta = dr.GetString("paikkakunta"),
                        ToimipisteNimi = dr.GetString("toimipiste_nimi"),
                        Katuosoite = dr.GetString("katuosoite"),
                        Postinumero = dr.GetString("postinumero"),
                        Postitoimipaikka = dr.GetString("postitoimipaikka"),
                        Puhelin = dr.GetString("puhelin"),
                        Yritys = GetCompany(dr.GetInt32("yritysID")),
                    });
                }
            }

            return toimipisteet;
        }

        /// <summary>
        /// Hakee toimipisteen huoneet
        /// </summary>
        /// <param name="toimipiste"></param>
        /// <returns></returns>
        public ObservableCollection<Room> GetAllRoomsByOffice(int toimipiste)
        {
            var huoneet = new ObservableCollection<Room>();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("SELECT * FROM huoneet WHERE toimipisteID=@ToimipisteID", conn);
                cmd.Parameters.AddWithValue("@ToimipisteID", toimipiste);

                var dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    huoneet.Add(new Room
                    {
                        HuoneenNumeroID = dr.GetInt32("huoneen_numeroID"),
                        Nimi = dr.GetString("nimi"),
                        Hinta = dr.GetDouble("hinta"),
                        AlvProsentti = dr.GetDouble("alv_prosentti"),
                        HloMaara = dr.GetInt32("hlo_maara"),
                        Toimipiste = GetOffice(dr.GetInt32("toimipisteID")),
                    });
                }
            }

            return huoneet;
        }

        /// <summary>
        /// Hakee toimipisteen palvelut
        /// </summary>
        /// <param name="toimipiste"></param>
        /// <returns></returns>
        public ObservableCollection<Service> GetAllServicesByOffice(int toimipiste)
        {
            var palvelut = new ObservableCollection<Service>();

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("SELECT * FROM palvelu WHERE toimipisteID=@ToimipisteID", conn);
                cmd.Parameters.AddWithValue("@ToimipisteID", toimipiste);

                var dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    palvelut.Add(new Service
                    {
                        PalveluID = dr.GetInt32("palveluID"),
                        Tuote = dr.GetString("tuote"),
                        PalvelunHinta = dr.GetDouble("palvelun_hinta"),
                        AlvProsentti = dr.GetDouble("alv_prosentti"),
                        Maara = dr.GetInt32("maara"),
                        Toimipiste = GetOffice(dr.GetInt32("toimipisteID")),
                    });
                }
            }

            return palvelut;
        }

        /// <summary>
        /// Lisää varauksen
        /// </summary>
        /// <param name="reservations"></param>
        public void AddReservation(Reservation reservation)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();            

                string query = "INSERT INTO asiakkaan_varaus (varausID, varaus_alkaa, varaus_paattyy, huoneen_numeroID, varauspvm, lisatiedot, asiakasID, tyontekijaID)" +
                    "VALUES(@VarausID, @VarausAlkaa, @VarausPaattyy, @HuoneenNumeroID, @VarausPaiva, @Lisatiedot, @AsiakasID, @TyontekijaID)";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@VarausID", reservation.VarausID);
                cmd.Parameters.AddWithValue("@VarausAlkaa", reservation.VarausAlkaa);
                cmd.Parameters.AddWithValue("@VarausPaattyy", reservation.VarausPaattyy);
                cmd.Parameters.AddWithValue("@HuoneenNumeroID", reservation.Huone.HuoneenNumeroID);
                cmd.Parameters.AddWithValue("@VarausPaiva", reservation.Varauspaiva);
                cmd.Parameters.AddWithValue("@Lisatiedot", reservation.Lisatiedot);
                cmd.Parameters.AddWithValue("@AsiakasID", reservation.Asiakas.AsiakasID);
                cmd.Parameters.AddWithValue("@TyontekijaID", reservation.Tyontekija.TyontekijaID);

                cmd.ExecuteNonQuery();
                reservation.VarausID = (int)cmd.LastInsertedId;
                
            }
        }

        /// <summary>
        /// Lisää varauksen palvelut
        /// </summary>
        /// <param name="reservationServices"></param>
        public void AddReservationServices(ReservationService reservationServices)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                string query = "INSERT INTO varauksen_palvelut (as_palveluvarauksenID, palveluID, varausID, kpl)" +
                    "VALUES(@PalveluvarausID, @PalveluID, @VarausID, @kpl)";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@PalveluvarausID", reservationServices.PalveluvarausID);
                cmd.Parameters.AddWithValue("@PalveluID", reservationServices.Palvelu.PalveluID);
                cmd.Parameters.AddWithValue("@VarausID", reservationServices.VarausID);
                cmd.Parameters.AddWithValue("@kpl", reservationServices.Kpl);

                cmd.ExecuteNonQuery();
                reservationServices.PalveluvarausID = (int)cmd.LastInsertedId;

            }
        }

        /// <summary>
        /// Hakee varauksen huoneen mukaan
        /// </summary>
        /// <param name="rooms"></param>
        /// <returns></returns>
        internal List<Reservation> GetAllReservationsForRoomsAndTime(ObservableCollection<Room> rooms)
        {
            var varaukset = new List<Reservation>();

            foreach (var room in rooms)
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("SELECT * FROM asiakkaan_varaus WHERE huoneen_numeroID=@huoneID;", conn);
                    cmd.Parameters.AddWithValue("@huoneID", room.HuoneenNumeroID);


                    var dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        varaukset.Add(new Reservation
                        {
                            VarausID = dr.GetInt32("varausID"),
                            VarausAlkaa = dr.GetDateTime("varaus_alkaa"),
                            VarausPaattyy = dr.GetDateTime("varaus_paattyy"),
                            Varauspaiva = dr.GetDateTime("varauspvm"),
                            Huone = GetRoom(dr.GetInt32("huoneen_numeroID")),
                            Lisatiedot = dr.GetString("lisatiedot"),
                            Asiakas = GetCustomer(dr.GetInt32("asiakasID")),
                            Tyontekija = GetEmployee(dr.GetInt32("tyontekijaID")),
                            VarauksenPalvelut = GetReservationServices(dr.GetInt32("varausID")),
                        });
                    }
                }
            }


            return varaukset;
        }

        /// <summary>
        /// Luodaan päivät varattaviksi
        /// </summary>
        /// <param name="rooms"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        internal List<ReservationAvailability> CreateBaseAvailability(ObservableCollection<Room> rooms, DateTime startDate, DateTime endDate)
        {
            var result = new List<ReservationAvailability>();

            foreach (var day in EachCalendarDay(startDate, endDate))
            {
                foreach (var room in rooms)
                {
                    result.Add(new ReservationAvailability() { Date = day, Room = room });
                }
            }

            return result;

        }

        /// <summary>
        /// Käy päiviä läpi
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public IEnumerable<DateTime> EachCalendarDay(DateTime startDate, DateTime endDate)
        {
            for (var date = startDate.Date; date.Date <= endDate.Date; date = date.AddDays(1)) yield
            return date;
        }

        /// <summary>
        /// Tarkistaa onko huone varattavissa ajankohtana
        /// </summary>
        /// <param name="baseAvailability"></param>
        /// <param name="reservations"></param>
        /// <returns></returns>
        internal List<ReservationAvailability> CreateAvailability(List<ReservationAvailability> baseAvailability, List<Reservation> reservations)
        {
            foreach (var availability in baseAvailability)
            {
                if (reservations.Any(reservation => reservation.Huone.HuoneenNumeroID == availability.Room.HuoneenNumeroID &&
                                                    reservation.VarausAlkaa <= availability.Date &&
                                                    reservation.VarausPaattyy >= availability.Date))
                {
                    availability.Available = false;
                }
            }

            return baseAvailability;
        }


        //Uuden tai muokatun varauksen tallentaminen:
        public void SaveReservation(Reservation varaus)
        {
            int ID = varaus.VarausID;
            bool uusiVaraus = (ID == 0);

            if (uusiVaraus)
            {
                //using (MySqlConnection conn = new MySqlConnection(connectionString))
                //{
                //    conn.Open();

                //    MySqlCommand cmd = new MySqlCommand("INSERT INTO asiakkaan_varaus(varaus_alkaa, varaus_paattyy, huoneen_numeroID, varauspvm, lisatiedot, asiakasID, tyontekijaID) VALUES(@varaus_alkaa, @varaus_paattyy, @huoneen_numeroID, @varauspvm, @lisatiedot, @asiakasID, @tyontekijaID)", conn);
                //    cmd.Parameters.AddWithValue("@varaus_alkaa", varaus.VarausAlkaa);
                //    cmd.Parameters.AddWithValue("@varaus_paattyy", varaus.VarausPaattyy);
                //    cmd.Parameters.AddWithValue("@huoneen_numeroID", varaus.Huone.HuoneenNumeroID);
                //    cmd.Parameters.AddWithValue("@varauspvm", varaus.Varauspaiva);
                //    cmd.Parameters.AddWithValue("@lisatiedot", varaus.Lisatiedot);
                //    cmd.Parameters.AddWithValue("@asiakasID", varaus.Asiakas.AsiakasID);
                //    cmd.Parameters.AddWithValue("@tyontekijaID", varaus.Tyontekija.TyontekijaID);

                //    cmd.ExecuteNonQuery();

                //    int lastInsertedId = Convert.ToInt32(cmd.ExecuteScalar());

                //    foreach (var varattuPalvelu in varaus.VarauksenPalvelut)
                //    {
                //        cmd = new MySqlCommand("INSERT INTO varauksen_palvelut(palveluID, kpl, varausID) VALUES(@palveluID, @kpl, @varausID)", conn);
                //        cmd.Parameters.AddWithValue("@palveluID", varattuPalvelu.Palvelu.PalveluID);
                //        cmd.Parameters.AddWithValue("@kpl", varattuPalvelu.Kpl);
                //        cmd.Parameters.AddWithValue("@varausID", lastInsertedId);
                        
                //        cmd.ExecuteNonQuery();
                //    }
                //}
            }
            else
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    //Päivitetään varauksen lisätiedot:
                    MySqlCommand cmd = new MySqlCommand("UPDATE asiakkaan_varaus SET lisatiedot=@lisatiedot WHERE varausID=@varausID", conn);
                    cmd.Parameters.AddWithValue("@varausID", varaus.VarausID);
                    cmd.Parameters.AddWithValue("@lisatiedot", varaus.Lisatiedot);
                    cmd.ExecuteNonQuery();

                    //Vanhat palveluvaraukset pois:
                    cmd = new MySqlCommand("DELETE FROM varauksen_palvelut WHERE varausID=@varausID", conn);
                    cmd.Parameters.AddWithValue("@varausID", varaus.VarausID);
                    cmd.ExecuteNonQuery();

                    //Tallennetaan päivitetyt palveluvaraukset:
                    foreach (var varattuPalvelu in varaus.VarauksenPalvelut)
                    {
                        cmd = new MySqlCommand("INSERT INTO varauksen_palvelut(palveluID, kpl, varausID) VALUES(@palveluID, @kpl, @varausID)", conn);
                        cmd.Parameters.AddWithValue("@palveluID", varattuPalvelu.Palvelu.PalveluID);
                        cmd.Parameters.AddWithValue("@kpl", varattuPalvelu.Kpl);
                        cmd.Parameters.AddWithValue("@varausID", varaus.VarausID);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        //Poistetaan varaus: 
        public void DeleteReservation(int reservationID)
        {
            using (var conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new MySqlCommand("DELETE FROM varauksen_palvelut WHERE varausID = @varausID", conn);
                cmd.Parameters.AddWithValue("@varausID", reservationID);
                cmd.ExecuteNonQuery();

                cmd = new MySqlCommand("DELETE FROM asiakkaan_varaus WHERE varausID = @varausID", conn);
                cmd.Parameters.AddWithValue("@varausID", reservationID);
                cmd.ExecuteNonQuery();
            }
        }

        //Asiakastietojen tallennus/päivitys:
        public void SaveCustomer(Customer asiakas)
        {
            bool uusiAsiakas = (asiakas.AsiakasID == 0);

            if (uusiAsiakas)
            {

                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    MySqlCommand cmd = new MySqlCommand("INSERT INTO asiakas(nimi, puhelin, katuosoite, postinumero, postitoimipaikka, sahkoposti) VALUES(@nimi, @puhelin, @katuosoite, @postinumero, @postitoimipaikka, @sahkoposti)", conn);
                    cmd.Parameters.AddWithValue("@nimi", asiakas.Nimi);
                    cmd.Parameters.AddWithValue("@puhelin", asiakas.Puhelin);
                    cmd.Parameters.AddWithValue("@katuosoite", asiakas.Katuosoite);
                    cmd.Parameters.AddWithValue("@postinumero", asiakas.Postinumero);
                    cmd.Parameters.AddWithValue("@postitoimipaikka", asiakas.Postitoimipaikka);
                    cmd.Parameters.AddWithValue("@sahkoposti", asiakas.Sahkoposti);
                    
                    cmd.ExecuteNonQuery();
                }
            }
            else
            {
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    MySqlCommand cmd = new MySqlCommand("UPDATE asiakas SET nimi=@nimi, puhelin=@puhelin, katuosoite=@katuosoite, postinumero=@postinumero, postitoimipaikka=@postitoimipaikka, sahkoposti=@sahkoposti  WHERE asiakasID=@asiakasID", conn);
                    cmd.Parameters.AddWithValue("@asiakasID", asiakas.AsiakasID);
                    cmd.Parameters.AddWithValue("@nimi", asiakas.Nimi);
                    cmd.Parameters.AddWithValue("@puhelin", asiakas.Puhelin);
                    cmd.Parameters.AddWithValue("@katuosoite", asiakas.Katuosoite);
                    cmd.Parameters.AddWithValue("@postinumero", asiakas.Postinumero);
                    cmd.Parameters.AddWithValue("@postitoimipaikka", asiakas.Postitoimipaikka);
                    cmd.Parameters.AddWithValue("@sahkoposti", asiakas.Sahkoposti);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        //Get information for reports
        private MySqlDataAdapter adapter;
        public DataTable GetInvoicesForPrint()
        {
            adapter = new MySqlDataAdapter();
            DataTable dataTable = new DataTable();
            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    conn.Open();
                    //var cmd = new MySqlCommand("SELECT lasku.*, asiakas.*, asiakkaan_varaus.* FROM lasku JOIN asiakas ON lasku.asiakasID = asiakas.asiakasID JOIN asiakkaan_varaus ON lasku.varausID = asiakkaan_varaus.varausID", conn);

                    //Add WHERE for dates
                    var cmd = new MySqlCommand("SELECT f.toimipiste_nimi, a.varausID, a.varaus_alkaa, a.varaus_paattyy, a.huoneen_numeroID, a.varauspvm, a.lisatiedot, a.asiakasID AS asiakkaan_varaus_asiakasID, a.tyontekijaID, b.asiakasID AS asiakas_asiakasID, b.nimi AS asiakas_nimi, b.puhelin AS asiakas_puhelin, b.katuosoite AS asiakas_katuosoite, b.postinumero AS asiakas_postinumero, b.postitoimipaikka AS asiakas_postitoimipaikka, b.sahkoposti AS asiakas_sahkoposti, c.palveluID AS varauksen_palvelut_palveluID, c.kpl, c.varausID AS varauksen_palvelut_varausID, d.palveluID AS palvelu_palveluID, d.tuote AS palvelu_tuote, d.palvelun_hinta, d.alv_prosentti AS palvelu_alv_prosentti, d.maara AS palvelu_maara\r\nFROM asiakkaan_varaus AS a, asiakas AS b, varauksen_palvelut AS c, palvelu AS d, huoneet AS e, toimipiste AS f\r\nWHERE a.asiakasID = b.asiakasID \r\nAND a.varausID = c.varausID \r\nAND c.palveluID = d.palveluID\r\nAND a.huoneen_numeroID = e.huoneen_numeroID\r\nAND e.toimipisteID = f.toimipisteID", conn);
                    adapter.SelectCommand = cmd;
                    adapter.Fill(dataTable);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Tietoja ei voitu hakea.", "Virhe!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return dataTable;

        }
    }
}


