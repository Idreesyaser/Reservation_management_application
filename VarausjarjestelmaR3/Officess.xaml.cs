using MySqlConnector;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace VarausjarjestelmaR3
    {
    /// <summary>
    /// Interaction logic for Officess.xaml
    /// </summary>
    public partial class Officess : Window
        {
        private string connectionString = "Server=127.0.0.1; Port=3306; User ID=opiskelija; Pwd=opiskelija1; Database=vuokratoimistot;";
        public List<toimipiste> toimipistet = new List<toimipiste>() { };


        public Officess ()
            {
            InitializeComponent();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                connection.Open();
                string query = "select toimipiste_nimi, toimipisteID from toimipiste";

                MySqlCommand command = new MySqlCommand(query, connection);
                var Reader = command.ExecuteReader();
                while (Reader.Read())
                    {
                    toimipistet.Add(new toimipiste() { id = Reader.GetInt32("toimipisteID"), name = Reader.GetString("toimipiste_nimi") });
                    combList.Items.Add(Reader.GetString("toimipiste_nimi"));
                    }
                this.DataContext = toimipistet;

                }
            }

        private void Check (object sender, RoutedEventArgs e)
            {

            }

        private void clear (object sender, DependencyPropertyChangedEventArgs e)
            {
            TextBox item = (TextBox)sender;
            item.Text = " ";
            }

        private void txtToimipisteNimi_MouseEnter (object sender, MouseEventArgs e)
            {
            TextBox item = (TextBox)sender;
            item.Text = " ";
            }

        private void makeConne (object sender, RoutedEventArgs e)
            {
            bool check = checker();
            if (check) {

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                    {
                    connection.Open();
                    string query = "INSERT INTO toimipiste (toimipiste_nimi, paikkakunta, katuosoite, postinumero, postitoimipaikka, puhelin, yritysID) VALUES (@Nimi, @Paikkakunta, @Katuosoite, @Postinumero, @Postitoimipaikka, @Puhelin, 1)";

                    MySqlCommand command = new MySqlCommand(query, connection);
                        
                        command.Parameters.AddWithValue("@Nimi", txtToimipisteNimi.Text);
                        command.Parameters.AddWithValue("@Paikkakunta", txtPaikkakunta.Text);
                        command.Parameters.AddWithValue("@Katuosoite", txtKatuosoite.Text);
                        command.Parameters.AddWithValue("@Postinumero", txtPostinumero.Text);
                        command.Parameters.AddWithValue("@Postitoimipaikka", txtPostitoimipaikka.Text);
                        command.Parameters.AddWithValue("@Puhelin", txtPuhelin.Text);


                    try
                        {
                            command.ExecuteNonQuery();
                            MessageBox.Show("Toimipiste lisätty onnistuneesti.");
                            }
                        catch (Exception ex)
                            {
                            MessageBox.Show("Virhe: " + ex.Message);
                                          }

                            }
                   
                }
            else
                {
                MessageBox.Show("täytä puutteet!");
                }

            }
        private bool checker ()
            {
          return true;
            }


        }
    public class toimipiste 
        {
        public int id { get; set; }
        public string name {  get; set; }
        public string Paikkakunta { get; set; }
        public string Katuosoite { get; set; }
        public string Postinumero { get; set; }
        public string Postitoimipaikka { get; set; }
        public string Puhelin { get; set; }



        }
    }