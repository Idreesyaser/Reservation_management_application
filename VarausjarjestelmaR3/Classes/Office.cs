using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VarausjarjestelmaR3.Classes
{
    public class Office
    {
        public int ToimipisteID {  get; set; }
        public String Paikkakunta { get; set; }
        public String ToimipisteNimi { get; set; }
        public String Katuosoite { get; set; }
        public String Postinumero { get; set; }
        public String Postitoimipaikka { get; set; }
        public String Puhelin {  get; set; }
        public Company Yritys { get; set; }

        public override string ToString()
        {
            return ToimipisteNimi;
        }
    }
}
