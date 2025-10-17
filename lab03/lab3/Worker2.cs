using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab3
{
    public class Worker2
    {
        // wstrzyknięcie przez metodę SetCalculator
        ICalculator _calculator;

        public Worker2() { }

        public void SetCalculator(ICalculator calculator)
        {
            _calculator = calculator;
        }

        public string Work(string a, string b)
        {
            var result = _calculator.Eval(a, b);
            Console.WriteLine(result);
            return result;
        }
    }
}
