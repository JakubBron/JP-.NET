using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autofac;

namespace lab3
{
    public class Worker
    {
        // wstrzyknięcie przez konstuktor
        public readonly ICalculator _calculator;

        public Worker(ICalculator calculator)
        {
            _calculator = calculator ?? throw new ArgumentNullException(nameof(calculator));
        }

        public string Work(string a, string b)
        {
            var result = _calculator.Eval(a, b);
            Console.WriteLine(result);
            return result;
        }
    }
}   
