using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab3
{
    public class CatCalc: ICalculator
    {
        public string Eval(string a, string b)
        {
            return a + b;
        }
    }
}
