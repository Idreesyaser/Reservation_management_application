using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VarausjarjestelmaR3.Classes
{
    public class Customer
    {
        public int AsiakasID { get; set; }
        public String Nimi {  get; set; }
        public String Puhelin { get; set; }
        public String Katuosoite { get; set; }
        public String Postinumero { get; set; }
        public String Postitoimipaikka { get; set; }
        public String Sahkoposti { get; set; }
    }
}
