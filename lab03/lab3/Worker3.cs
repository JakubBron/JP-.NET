using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab3
{
    public class Worker3
    {
        // wstrzyknięcie przez właściwość

        public ICalculator calculator { get; set; } 

        public string Work(string a, string b)
        {
            var result = calculator.Eval(a, b);
            Console.WriteLine(result);
            return result;
        }
    }
}
