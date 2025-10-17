using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab3
{

    public class TransactionContext : ITransactionContext
    {
        public Guid Id { get; }
        public IUnitOfWork UnitOfWork { get; }

        public TransactionContext(IUnitOfWork unitOfWork)
        {
            Id = Guid.NewGuid();
            UnitOfWork = unitOfWork;
        }
    }
}
