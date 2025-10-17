using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab3
{
    public class StepOneService
    {
        public readonly ITransactionContext _context;

        public StepOneService(ITransactionContext context)
        {
            _context = context;
        }

        public void Execute()
        {
            Console.WriteLine($"[StepOne] Transaction: {_context.Id}, UnitOfWork: {_context.UnitOfWork.Id}");
        }
    }

}
