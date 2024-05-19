using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace VarausjarjestelmaR3.Classes
{
    public class Reservation
    {
        public int VarausID { get; set; }
        public DateTime VarausAlkaa { get; set; }
        public DateTime VarausPaattyy { get; set; }
        public DateTime Varauspaiva { get; set; }
        public Room Huone {  get; set; }
        public String Lisatiedot { get; set; }
        public Customer Asiakas { get; set; }
        public Employee Tyontekija { get; set; }
        public ObservableCollection<ReservationService> VarauksenPalvelut { get; set; }
    }

    public class ReservationService
    {
        public int PalveluvarausID { get; set; }
        public Service Palvelu {  get; set; }
        public int Kpl {  get; set; }
        public int VarausID { get; set; }
    }

    public class ReservationForEmplyoeeHis
        {
        public int VarausID { get; set; }
        public DateTime VarausAlkaa { get; set; }
        public DateTime VarausPaattyy { get; set; }
        public DateTime Varauspaiva { get; set; }
        public Room Huone { get; set; }
        public String Lisatiedot { get; set; }
        public Customer Asiakas { get; set; }
        public Employee Tyontekija { get; set; }
        public ObservableCollection<ReservationService> VarauksenPalvelut { get; set; }
        }

    public class ReservationAvailability
    {
        public DateTime Date { get; set; }
        public bool Available { get; set; } = true;
        public Room Room { get; set; }
    }


    }
