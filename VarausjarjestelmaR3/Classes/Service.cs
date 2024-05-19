using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VarausjarjestelmaR3.Classes
{
    public class Service
    {
        public int PalveluID { get; set; }
        public String Tuote {  get; set; }
        public double PalvelunHinta { get; set; }
        public double AlvProsentti {  get; set; }
        public int Maara {  get; set; }
        public Office Toimipiste { get; set; }
        public int ValittuMaara { get; set; } = 1;

        public override string ToString()
        {
            return Tuote;
        }
    }

}
