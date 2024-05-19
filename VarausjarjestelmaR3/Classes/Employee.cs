using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VarausjarjestelmaR3.Classes
{
    public class Employee
        {
        public int TyontekijaID { get; set; }
        public String Nimi { get; set; }
        public String Osoite { get; set; }
        public String Puhelin { get; set; }
        public String KayttajaID { get; set; }
        public String Salasana { get; set; }
        public int KayttoOikeus { get; set; }
        public Company Yritys { get; set; }
        public Office office {get; set;}
    }
}
