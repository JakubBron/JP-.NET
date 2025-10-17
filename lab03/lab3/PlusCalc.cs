using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab3
{
    public class PlusCalc: ICalculator
    {
        public PlusCalc() { }

        public string Eval(string a, string b)
        {
            int num1 = int.Parse(a);
            int num2 = int.Parse(b);
            return (num1 + num2).ToString();
        }
    }
}
