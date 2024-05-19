using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VarausjarjestelmaR3.Classes
{
    public class Invoice
    {
        public int Laskunumero { get; set; }
        public String Laskutustapa { get; set; }
        public double VerotonSumma { get; set; }
        public double AlvEuroina { get; set; }
        public double Loppusumma { get; set; }
        public int AsiakasID { get; set; }
        public int VarausID { get; set; }
        public Customer Asiakas { get; set; }
        public Reservation Varaus { get; set; }
    }
}
