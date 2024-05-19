using System;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using VarausjarjestelmaR3.Classes;

namespace VarausjarjestelmaR3
{
    /// <summary>
    /// Interaction logic for Report.xaml
    /// </summary>
    public partial class Report : UserControl
    {
        Repository repo;
        ObservableCollection<Reservation> kaikkiVaraukset;
        ObservableCollection<Reservation> valitutVaraukset;
        ObservableCollection<Reservation> jarjestysVaraukset;
        Office selected;
        DateTime startDate;
        DateTime endDate;

        public Report()
        {
            {
                InitializeComponent();

                repo = new Repository();

                var municipalities = GetDistinctMunicipalities();
                municipalities.Insert(0, new Office { Paikkakunta = "Kaikki" });
                ComMunicipality.ItemsSource = municipalities;
            }

        }
        private void GetInfo_Click(object sender, RoutedEventArgs e)
        {
            lvVaraukset.ItemsSource = null;

            kaikkiVaraukset = repo.GetAllReservations();
            jarjestysVaraukset = new ObservableCollection<Reservation>(kaikkiVaraukset.OrderBy(v => v.VarausAlkaa));  //Varaukset aikajärjestykseen.

            valitutVaraukset = new ObservableCollection<Reservation>();
            lvVaraukset.ItemsSource = valitutVaraukset;

            // Get the selected start and end dates
            startDate = StartDatePicker.SelectedDate ?? DateTime.MinValue;
            endDate = EndDatePicker.SelectedDate ?? DateTime.MaxValue;

            //Selected office:
            Office selectedOffice = ComOffices.SelectedItem as Office;

            FilterReservations(jarjestysVaraukset, selectedOffice, startDate, endDate);
        }

        public void FilterReservations(ObservableCollection<Reservation> varaukset, Office selectedOffice, DateTime startDate, DateTime endDate)
        {
            foreach (var varaus in varaukset)
            {
                bool isWithinDateRange = varaus.VarausAlkaa >= startDate && varaus.VarausPaattyy <= endDate;
                bool overlapsStartDate = varaus.VarausAlkaa < startDate && varaus.VarausPaattyy <= endDate && varaus.VarausPaattyy >= startDate;
                bool overlapsEndDate = varaus.VarausAlkaa < startDate && varaus.VarausPaattyy > endDate;
                bool overlapsBothDates = varaus.VarausAlkaa >= startDate && varaus.VarausAlkaa <= endDate && varaus.VarausPaattyy > endDate;

                if ((selectedOffice != null && selectedOffice.Paikkakunta == "Kaikki" && selectedOffice.ToimipisteNimi == "Kaikki") && (isWithinDateRange || overlapsStartDate || overlapsEndDate || overlapsBothDates))
                {
                    valitutVaraukset.Add(varaus);
                }
                else if ((selectedOffice != null && selectedOffice.Paikkakunta == varaus.Huone.Toimipiste.Paikkakunta && selectedOffice.ToimipisteNimi == "Kaikki") && (isWithinDateRange || overlapsStartDate || overlapsEndDate || overlapsBothDates))
                {
                    valitutVaraukset.Add(varaus);
                }
                else if ((selectedOffice != null && selectedOffice.Paikkakunta == varaus.Huone.Toimipiste.Paikkakunta && selectedOffice.ToimipisteNimi == varaus.Huone.Toimipiste.ToimipisteNimi) && (isWithinDateRange || overlapsStartDate || overlapsEndDate || overlapsBothDates))
                {
                    valitutVaraukset.Add(varaus);
                }
                else if ((selectedOffice != null && varaus.Huone.Toimipiste.ToimipisteNimi == selectedOffice.ToimipisteNimi) && (isWithinDateRange || overlapsStartDate || overlapsEndDate || overlapsBothDates))
                {
                    valitutVaraukset.Add(varaus);
                }
            }
        }


        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            ListView listView = lvVaraukset as ListView;

            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string fileName = $"Report_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
            string filePath = System.IO.Path.Combine(desktopPath, fileName);
            Office selectedOffice = ComOffices.SelectedItem as Office;

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine($"Tiedot väliltä {startDate.ToString("dd.MM.yyyy")}-{endDate.ToString("dd.MM.yyyy")}: ");
                writer.WriteLine($"Paikkakunta: {selectedOffice.Paikkakunta}");
                writer.WriteLine($"Toimipiste: {selectedOffice.ToimipisteNimi}");
                writer.WriteLine();
                writer.WriteLine();
                writer.WriteLine();

                foreach (var item in listView.Items)
                {
                    if (item is Reservation reservation)
                    {
                        //Muotoillaan päivämäärä:
                        string varausAlkaa = reservation.VarausAlkaa.ToString("dd.MM.yyyy");
                        string varausPaattyy = reservation.VarausPaattyy.ToString("dd.MM.yyyy");

                        //Varauksen tiedot tiedostoon:
                        writer.WriteLine($"Aika: \t\t{varausAlkaa}-{varausPaattyy}");
                        writer.WriteLine($"Toimipiste: \t{reservation.Huone.Toimipiste}");
                        writer.WriteLine($"Huone: \t\t{reservation.Huone.Nimi}");
                        writer.WriteLine($"Asiakas: \t{reservation.Asiakas.Nimi}, asiakasnumero {reservation.Asiakas.AsiakasID}");
                        writer.WriteLine($"Varausnumero: \t{reservation.VarausID}");

                        //Haetaan varauksen palvelut:
                        var varauksenPalvelut = reservation.VarauksenPalvelut;

                        if (varauksenPalvelut.Count > 0)
                        {
                            writer.Write("Palvelut: \t");
                            for (int i = 0; i < varauksenPalvelut.Count; i++)
                            {
                                var palvelu = varauksenPalvelut[i];
                                writer.Write($"{palvelu.Palvelu.Tuote} {palvelu.Kpl} kpl");

                                //Lisätään pilkku ja välilyönti, paitsi jos viimeinen tulostettava palvelu
                                if (i < varauksenPalvelut.Count - 1)
                                {
                                    writer.Write(", ");
                                }
                            }
                            writer.WriteLine();
                        }

                        if (reservation.Lisatiedot != null && reservation.Lisatiedot != "")
                        {
                            writer.WriteLine($"Lisätiedot: \t{reservation.Lisatiedot}");
                        }

                        //Tyhjiä rivejä ja viivaa jokaisen varauksen väliin:
                        writer.WriteLine();
                        writer.WriteLine("--------------------------------------------------");
                        writer.WriteLine();
                    }
                }

                MessageBox.Show($"Raportti tallennettu työpöydälle tekstitiedostoon nimellä: {fileName}", "Tallennus onnistui", MessageBoxButton.OK, MessageBoxImage.Information);  //Confirmation box
            }
        }

        private void MunicipalityCom_Changed(object sender, RoutedEventArgs e)
        {
            var selected = ComMunicipality.SelectedItem as Office;


            // Jos "Kaikki" on valittu, hae kaikki toimipisteet
            if (selected?.Paikkakunta == "Kaikki")
            {
                var offices = repo.GetAllOffices();
                offices.Insert(0, new Office { ToimipisteID = -1, ToimipisteNimi = "Kaikki", Paikkakunta = "Kaikki" });
                ComOffices.ItemsSource = offices;
            }
            else
            {
                ComOffices.ItemsSource = repo.GetAllOfficesByMunicipality(selected.Paikkakunta);

                // Lisää "Kaikki" vaihtoehto ComboBoxiin ComOffices
                var offices = repo.GetAllOfficesByMunicipality(selected.Paikkakunta);
                offices.Insert(0, new Office { ToimipisteID = -1, ToimipisteNimi = "Kaikki", Paikkakunta = selected.Paikkakunta });
                ComOffices.ItemsSource = offices;
            }

        }

        /// Hakee uniikit paikkakuntien nimet
        public ObservableCollection<Office> GetDistinctMunicipalities()
        {
            ObservableCollection<Office> distinctNames = repo.GetAllOffices();

            var distinct = new ObservableCollection<Office>();

            foreach (var distinctName in distinctNames)
            {
                if (distinct.Any(x => x.Paikkakunta == distinctName.Paikkakunta))
                {
                    continue;
                }

                distinct.Add(distinctName);
            }

            return distinct;
        }

    }
}
