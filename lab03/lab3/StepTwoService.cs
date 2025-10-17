using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab3
{
    public class StepTwoService
    {
        public readonly ITransactionContext _context;

        public StepTwoService(ITransactionContext context)
        {
            _context = context;
        }

        public void Execute()
        {
            Console.WriteLine($"[StepTwo] Transaction: {_context.Id}, UnitOfWork: {_context.UnitOfWork.Id}");
        }
    }
}
