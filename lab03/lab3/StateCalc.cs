using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab3
{
    public class StateCalc: ICalculator
    {
        public int _iter = 0;

        public StateCalc(int iter)
        {
            this._iter = iter;
        }

        public string Eval(string a, string b)
        {
            string result =  a+b+(_iter).ToString();
            _iter++;
            return result;
        }
    }
}
