using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Messaging;
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
    /// Interaction logic for Offices.xaml
    /// </summary>
    public partial class Offices : UserControl
        {
        private string connectionString = "Server=127.0.0.1; Port=3306; User ID=opiskelija; Pwd=opiskelija1; Database=vuokratoimistot;";
        // Alustetaan tarvittavat luokat ja listat
        Classes.Office toimipiste;
        Classes.Room room;
        OffficeInfoList OffficeInfoListForChng = new OffficeInfoList();
        OffficeInfoList OffficeInfoListForAdd = new OffficeInfoList();
        RoomInfoList RoomInfoList = new RoomInfoList();
        RoomInfoList RoomInfoListForChange = new RoomInfoList();
        RoomInfoList RoomInfoListForDel = new RoomInfoList();
        OffficeInfoList offficeInfoListDel = new OffficeInfoList();

        public Offices ()
            {
            InitializeComponent();

            }
        // Metodi, joka suoritetaan, kun uusi toimipiste lisätään
        private void AddNewOffice (object sender, RoutedEventArgs e)
            {
            bool check = checker();
            if (check)
                {
                // Suoritetaan tietokantakysely ja lisätään uusi toimipiste
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                    {
                    connection.Open();
                    string query = "INSERT INTO toimipiste (toimipiste_nimi, paikkakunta, katuosoite, postinumero, postitoimipaikka, puhelin, yritysID) VALUES (@Nimi, @Paikkakunta, @Katuosoite, @Postinumero, @Postitoimipaikka, @Puhelin, 1)";

                    MySqlCommand command = new MySqlCommand(query, connection);

                    command.Parameters.AddWithValue("@Nimi", OffficeInfoListForAdd.txtToimipisteNimi.Text);
                    command.Parameters.AddWithValue("@Paikkakunta", OffficeInfoListForAdd.txtPaikkakunta.Text);
                    command.Parameters.AddWithValue("@Katuosoite", OffficeInfoListForAdd.txtKatuosoite.Text);
                    command.Parameters.AddWithValue("@Postinumero", OffficeInfoListForAdd.txtPostinumero.Text);
                    command.Parameters.AddWithValue("@Postitoimipaikka", OffficeInfoListForAdd.txtPostitoimipaikka.Text);
                    command.Parameters.AddWithValue("@Puhelin", OffficeInfoListForAdd.txtPuhelin.Text);


                    try
                        {
                        command.ExecuteNonQuery();
                        MessageBox.Show("Toimipiste lisätty onnistuneesti.");
                        OffficeInfoListForAdd.DataContext = new Classes.Office();
                        }
                    catch (Exception ex)
                        {
                        MessageBox.Show("Virhe: " + ex.Message);
                        }

                    }
                Offices_Loaded();

                }
            else
                {
                MessageBox.Show("täytä puutteet!");
                }

            }

        // Metodi, joka tarkistaa syötteiden oikeellisuuden
        private bool checker ()
            {
            return true;
            }


        private void DeleteBtn (object sender, RoutedEventArgs e)
            {
            MessageBoxResult result = MessageBox.Show("Haluatko varmasti poistaa tämän toimipisteen ja kaikki siihen liittyvät tiedot?", "Vahvista poisto", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.No)
                {
                return; // Peruuta poisto, jos käyttäjä ei halua jatkaa
                }

            using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                connection.Open();
                MySqlTransaction transaction = connection.BeginTransaction();

                try
                    {


                    // Poista toimipisteeseen liittyvät työntekijät
                    using (MySqlCommand deleteEmployeesCommand = new MySqlCommand())
                        {
                        deleteEmployeesCommand.Connection = connection;
                        deleteEmployeesCommand.Transaction = transaction;
                        deleteEmployeesCommand.CommandText = "DELETE FROM toimipisteen_tyontekija WHERE toimipisteID = @BranchID";
                        deleteEmployeesCommand.Parameters.AddWithValue("@BranchID", combListOfDelete.SelectedValue);
                        deleteEmployeesCommand.ExecuteNonQuery();
                        }

                    // Poista toimipisteeseen liittyvät laskut
                    using (MySqlCommand deleteInvoicesCommand = new MySqlCommand())
                        {
                        deleteInvoicesCommand.Connection = connection;
                        deleteInvoicesCommand.Transaction = transaction;
                        deleteInvoicesCommand.CommandText = "DELETE FROM lasku WHERE varausID IN (SELECT varausID FROM asiakkaan_varaus WHERE huoneen_numeroID IN (SELECT huoneen_numeroID FROM huoneet WHERE toimipisteID = @BranchID))";
                        deleteInvoicesCommand.Parameters.AddWithValue("@BranchID", combListOfDelete.SelectedValue);
                        deleteInvoicesCommand.ExecuteNonQuery();
                        }

                    // Poista toimipisteeseen liittyvät varaukset ja niihin liittyvät palvelut
                    using (MySqlCommand deleteBookingsCommand = new MySqlCommand())
                        {
                        deleteBookingsCommand.Connection = connection;
                        deleteBookingsCommand.Transaction = transaction;
                        deleteBookingsCommand.CommandText = "DELETE FROM varauksen_palvelut WHERE varausID IN (SELECT varausID FROM asiakkaan_varaus WHERE huoneen_numeroID IN (SELECT huoneen_numeroID FROM huoneet WHERE toimipisteID = @BranchID))";
                        deleteBookingsCommand.Parameters.AddWithValue("@BranchID", combListOfDelete.SelectedValue);
                        deleteBookingsCommand.ExecuteNonQuery();
                        }

                    using (MySqlCommand deleteReservationsCommand = new MySqlCommand())
                        {
                        deleteReservationsCommand.Connection = connection;
                        deleteReservationsCommand.Transaction = transaction;
                        deleteReservationsCommand.CommandText = "DELETE FROM asiakkaan_varaus WHERE huoneen_numeroID IN (SELECT huoneen_numeroID FROM huoneet WHERE toimipisteID = @BranchID)";
                        deleteReservationsCommand.Parameters.AddWithValue("@BranchID", combListOfDelete.SelectedValue);
                        deleteReservationsCommand.ExecuteNonQuery();
                        }
                    // Poista toimipisteeseen liittyvät huoneet
                    using (MySqlCommand deleteRoomsCommand = new MySqlCommand())
                        {
                        deleteRoomsCommand.Connection = connection;
                        deleteRoomsCommand.Transaction = transaction;
                        deleteRoomsCommand.CommandText = "DELETE FROM huoneet WHERE toimipisteID = @BranchID";
                        deleteRoomsCommand.Parameters.AddWithValue("@BranchID", combListOfDelete.SelectedValue);
                        deleteRoomsCommand.ExecuteNonQuery();
                        }

                    using (MySqlCommand deleteRoomsCommand = new MySqlCommand())
                        {
                        deleteRoomsCommand.Connection = connection;
                        deleteRoomsCommand.Transaction = transaction;
                        deleteRoomsCommand.CommandText = "DELETE FROM palvelu WHERE toimipisteID = @BranchID";
                        deleteRoomsCommand.Parameters.AddWithValue("@BranchID", combListOfDelete.SelectedValue);
                        deleteRoomsCommand.ExecuteNonQuery();
                        }
                    // Lopuksi poista itse toimipiste
                    using (MySqlCommand deleteBranchCommand = new MySqlCommand())
                        {
                        deleteBranchCommand.Connection = connection;
                        deleteBranchCommand.Transaction = transaction;
                        deleteBranchCommand.CommandText = "DELETE FROM toimipiste WHERE toimipisteID = @BranchID";
                        deleteBranchCommand.Parameters.AddWithValue("@BranchID", combListOfDelete.SelectedValue);
                        deleteBranchCommand.ExecuteNonQuery();
                        }

                    transaction.Commit();
                    MessageBox.Show("Toimipiste ja siihen liittyvät tiedot on poistettu onnistuneesti.", "Poisto onnistui", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                catch (Exception ex)
                    {
                    MessageBox.Show("Virhe: " + ex.Message);

                    transaction.Rollback();
                    }
                }
            offficeInfoListDel.DataContext = new Classes.Office();
            Offices_Loaded();
            }
        // Metodi, joka suoritetaan käyttöliittymän latautuessa
        private void onLoaded (object sender, RoutedEventArgs e)
            {
            Offices_Loaded();


            }


        private void Offices_Loaded ()
            {

            using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                connection.Open();
                string query = "select toimipisteID, toimipiste_nimi from toimipiste";

                MySqlCommand command = new MySqlCommand(query, connection);
                var Reader = command.ExecuteReader();
                List<Classes.Office> toimipistet = new List<Classes.Office>() { };

                while (Reader.Read())
                    {
                    toimipistet.Add(new Classes.Office() { ToimipisteID = Reader.GetInt32("toimipisteID"), ToimipisteNimi = Reader.GetString("toimipiste_nimi") });

                    }
                combListOfDelete.ItemsSource = toimipistet;
                combListOfChange.ItemsSource = toimipistet;
                RoomInfoList.ToimipisteNimi.ItemsSource = toimipistet;
                RoomInfoListForChange.ToimipisteNimi.ItemsSource = toimipistet;
                RoomInfoListForDel.ToimipisteNimi.ItemsSource = toimipistet;
                }

            using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                connection.Open();
                string queryForRooms = "select huoneen_numeroID, nimi from huoneet";


                MySqlCommand command = new MySqlCommand(queryForRooms, connection);
                command = new MySqlCommand(queryForRooms, connection);
                var ReaderForRooms = command.ExecuteReader();
                List<Classes.Room> rooms = new List<Classes.Room>();
                while (ReaderForRooms.Read())
                    {
                    rooms.Add(new Classes.Room() { HuoneenNumeroID = ReaderForRooms.GetInt32("huoneen_numeroID"), Nimi = ReaderForRooms.GetString("nimi") });
                    }
                RoomcombListOfDelete.ItemsSource = rooms;
                RoomcombListOfChange.ItemsSource = rooms;

                }

            UserControlAddSec.Content = OffficeInfoListForAdd;
            RoomUserControlAddSec.Content = RoomInfoList;

            }


        // Metodi, joka näyttää valitun toimipisteen tiedot
        private void ShowOfficeInfo (object sender, RoutedEventArgs e)
            {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                connection.Open();
                string query = "select * from toimipiste where toimipisteID = @toimipisteID ";


                MySqlCommand command = new MySqlCommand(query, connection);
                if (changeSection.IsSelected)
                    {
                    command.Parameters.AddWithValue("@toimipisteID", combListOfChange.SelectedValue);
                    }
                else if (deleteSection.IsSelected)
                    {
                    command.Parameters.AddWithValue("@toimipisteID", combListOfDelete.SelectedValue);
                    }

                command.ExecuteNonQuery();
                var Reader = command.ExecuteReader();
                while (Reader.Read())
                    {
                    toimipiste = new Classes.Office() { ToimipisteNimi = Reader.GetString("toimipiste_nimi"), Katuosoite = Reader.GetString("katuosoite"), Paikkakunta = Reader.GetString("paikkakunta"), Postinumero = Reader.GetString("postinumero"), Postitoimipaikka = Reader.GetString("postitoimipaikka"), Puhelin = Reader.GetString("puhelin") };
                    }
                if (changeSection.IsSelected)
                    {
                    OffficeInfoListForChng.DataContext = toimipiste;
                    toimiposteContentControlsec.Content = OffficeInfoListForChng;
                    ChangeBtn.Visibility = Visibility.Visible;
                    }
                else if (deleteSection.IsSelected)
                    {
                    offficeInfoListDel.DataContext = toimipiste;
                    toimiposteContentControl.Content = offficeInfoListDel;
                    deleteBtn.Visibility = Visibility.Visible;
                    }
                }


            }
        

        // Metodi, joka muuttaa valitun toimipisteen tietoja
        private void Chnage (object sender, RoutedEventArgs e)
            {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                connection.Open();
                string query = "UPDATE toimipiste SET toimipiste_nimi = @toimipiste_nimi, katuosoite = @katuosoite, paikkakunta = @paikkakunta, postinumero = @postinumero, postitoimipaikka = @postitoimipaikka, puhelin = @puhelin  WHERE toimipisteID = @toimipisteID;";

                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@toimipiste_nimi", OffficeInfoListForChng.txtToimipisteNimi.Text);
                command.Parameters.AddWithValue("@Paikkakunta", OffficeInfoListForChng.txtPaikkakunta.Text);
                command.Parameters.AddWithValue("@Katuosoite", OffficeInfoListForChng.txtKatuosoite.Text);
                command.Parameters.AddWithValue("@Postinumero", OffficeInfoListForChng.txtPostinumero.Text);
                command.Parameters.AddWithValue("@Postitoimipaikka", OffficeInfoListForChng.txtPostitoimipaikka.Text);
                command.Parameters.AddWithValue("@Puhelin", OffficeInfoListForChng.txtPuhelin.Text);
                command.Parameters.AddWithValue("@toimipisteID", combListOfChange.SelectedValue);

                try{
                    command.ExecuteNonQuery();
                    MessageBox.Show("muokattu!");
                    }
                catch(Exception ex) { 
                    MessageBox.Show("Virhe: " + ex.Message);
                    }
                }
            OffficeInfoListForChng.DataContext = new Classes.Office();
            Offices_Loaded();
            }


        // Metodi, joka lisää uuden huoneen tietokantaan
        private void AddNewRoom (object sender, RoutedEventArgs e)
            {
            bool check = checker();
            if (check)
                {

                using (MySqlConnection connection = new MySqlConnection(connectionString))
                    {
                    connection.Open();
                    string query = "INSERT INTO huoneet (nimi, hinta, alv_prosentti, hlo_maara, toimipisteID) VALUES (@Nimi, @hinta, @alv_prosentti, @hlo_maara, @toimipisteID)";

                    MySqlCommand command = new MySqlCommand(query, connection);

                    command.Parameters.AddWithValue("@Nimi", RoomInfoList.RoomNimi.Text);
                    command.Parameters.AddWithValue("@hinta", RoomInfoList.Hinta.Text);
                    command.Parameters.AddWithValue("@alv_prosentti", RoomInfoList.AlvProsentti.Text);
                    command.Parameters.AddWithValue("@hlo_maara", RoomInfoList.HloMaara.Text);
                    command.Parameters.AddWithValue("@toimipisteID", RoomInfoList.ToimipisteNimi.SelectedValue);


                    try
                        {
                        command.ExecuteNonQuery();
                        MessageBox.Show("Huone lisätty onnistuneesti.");
                        }
                    catch (Exception ex)
                        {
                        MessageBox.Show("Virhe: " + ex.Message);
                        }

                    }
                RoomInfoList.DataContext = new Classes.Room();
                Offices_Loaded();

                }
            else
                {
                MessageBox.Show("täytä puutteet!");
                }
            }

        // Metodi, joka näyttää valitun huoneen tiedot
        private void RoomInfo (object sender, RoutedEventArgs e)
            {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                connection.Open();
                string query = "select * from huoneet where huoneen_numeroID = @huoneen_numeroID ";


                MySqlCommand command = new MySqlCommand(query, connection);
                if (RoomDeleteSec.IsSelected)
                    {
                    command.Parameters.AddWithValue("@huoneen_numeroID", RoomcombListOfDelete.SelectedValue);
                    }
                else if (RoomChangeSec.IsSelected)
                    {
                    command.Parameters.AddWithValue("@huoneen_numeroID", RoomcombListOfChange.SelectedValue);
                    }

                var Reader = command.ExecuteReader();
                while (Reader.Read())
                    {
                    room = new Classes.Room() { Nimi = Reader.GetString("nimi"), Hinta = Reader.GetFloat("hinta"), AlvProsentti = Reader.GetFloat("alv_prosentti"), HloMaara = Reader.GetInt32("hlo_maara"), Toimipiste = new Classes.Office() { ToimipisteID = Reader.GetInt32("toimipisteID") } };
                    }
                if (RoomDeleteSec.IsSelected)
                    {
                    RoomInfoListForDel.DataContext = room;
                    RoomInfoListForDel.ToimipisteNimi.SelectedValue = room.Toimipiste.ToimipisteID;
                    HuoneContentControl.Content = RoomInfoListForDel;
                    RoomDeleteBtn.Visibility = Visibility.Visible;
                    }
                else if (RoomChangeSec.IsSelected)
                    {
                    RoomInfoListForChange.DataContext = room;
                    RoomInfoListForChange.ToimipisteNimi.SelectedValue = room.Toimipiste.ToimipisteID;
                    HuoneContentControlForChange.Content = RoomInfoListForChange;
                    RoomChangeBtn.Visibility = Visibility.Visible;
                    }
                }
            

                }
            

        // Metodi, joka poistaa valitun huoneen tietokannasta
        private void DeleteRoomBtn (object sender, RoutedEventArgs e)
            {

            MessageBoxResult result = MessageBox.Show("Kaikki huoneen varaukset poistetaan pysyväti! Haluatko varmasti poistaa tämän huoneen?", "Vahvista poisto", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
                {
                // Poista huone tietokannasta
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                    {
                    connection.Open();
                    MySqlTransaction transaction = connection.BeginTransaction();

                    try
                        {


                        using (MySqlCommand command = new MySqlCommand())
                            {
                            command.Connection = connection;
                            command.Transaction = transaction;
                            command.CommandText = "DELETE FROM lasku WHERE varausID IN (SELECT varausID FROM asiakkaan_varaus WHERE huoneen_numeroID = @RoomID)";
                            command.Parameters.AddWithValue("@RoomID", RoomcombListOfDelete.SelectedValue);
                            command.ExecuteNonQuery();
                            }
                        // Ensin poistetaan varaukset ja niihin liittyvät palvelut
                        using (MySqlCommand command = new MySqlCommand())
                            {
                            command.Connection = connection;
                            command.Transaction = transaction;
                            command.CommandText = "DELETE FROM varauksen_palvelut WHERE varausID IN (SELECT varausID FROM asiakkaan_varaus WHERE huoneen_numeroID = @RoomID)";
                            command.Parameters.AddWithValue("@RoomID", RoomcombListOfDelete.SelectedValue);
                            command.ExecuteNonQuery();
                            }

                        using (MySqlCommand command = new MySqlCommand())
                            {
                            command.Connection = connection;
                            command.Transaction = transaction;
                            command.CommandText = "DELETE FROM asiakkaan_varaus WHERE huoneen_numeroID = @RoomID";
                            command.Parameters.AddWithValue("@RoomID", RoomcombListOfDelete.SelectedValue);
                            command.ExecuteNonQuery();
                            }

                        // Lopuksi poistetaan huone
                        using (MySqlCommand command = new MySqlCommand())
                            {
                            command.Connection = connection;
                            command.Transaction = transaction;
                            command.CommandText = "DELETE FROM huoneet WHERE huoneen_numeroID = @RoomID";
                            command.Parameters.AddWithValue("@RoomID", RoomcombListOfDelete.SelectedValue);
                            command.ExecuteNonQuery();
                            }

                        transaction.Commit();
                        MessageBox.Show("tila on poistettu onnistuneesti.", "Poisto onnistui", MessageBoxButton.OK, MessageBoxImage.Information);


                        }
                    catch (Exception ex)
                        {
                        MessageBox.Show("Virhe: " + ex.Message);
                        transaction.Rollback();
                        }
                    }

                }
            else
                {
                MessageBox.Show("Poisto peruutettu.", "Toiminto peruutettu", MessageBoxButton.OK, MessageBoxImage.Information);
                }


            RoomInfoListForDel.DataContext= new Classes.Room();
            Offices_Loaded();


            }
        // Metodi, joka muuttaa valitun huoneen tietoja
        private void ChangeRoomBtn (object sender, RoutedEventArgs e)
            {
            using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                connection.Open();
                string query = "UPDATE huoneet SET nimi = @nimi, hinta = @hinta, alv_prosentti = @alv_prosentti, hlo_maara = @hlo_maara, toimipisteID = @toimipisteID where huoneen_numeroID = @huoneen_numeroID ";

                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@nimi", RoomInfoListForChange.RoomNimi.Text);
                command.Parameters.AddWithValue("@hinta", RoomInfoListForChange.Hinta.Text);
                command.Parameters.AddWithValue("@alv_prosentti", RoomInfoListForChange.AlvProsentti.Text);
                command.Parameters.AddWithValue("@hlo_maara", RoomInfoListForChange.HloMaara.Text);
                command.Parameters.AddWithValue("@toimipisteID", RoomInfoListForChange.ToimipisteNimi.SelectedValue);
                command.Parameters.AddWithValue("@huoneen_numeroID", RoomcombListOfChange.SelectedValue);

                try
                    {
                    
                    command.ExecuteNonQuery();

                    MessageBox.Show("muokattu!");
                    }
                catch (Exception ex)
                    {
                    MessageBox.Show("Virhe: " + ex.Message);
                    }

                }
            RoomInfoListForChange.DataContext = new Classes.Room();
            Offices_Loaded();

            }
        }
    }
