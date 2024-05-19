using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace VarausjarjestelmaR3.Classes
{
    public class Room
    {
        public int HuoneenNumeroID {  get; set; }
        public String Nimi {  get; set; }
        public double Hinta { get; set; }
        public double AlvProsentti { get; set; }
        public int HloMaara { get; set; }
        public Office Toimipiste { get; set; }
        public Brush Color { get; set; }
        public bool CanBeReserved { get; set; }

        public override string ToString()
        {
            return Nimi;
        }
    }
}
