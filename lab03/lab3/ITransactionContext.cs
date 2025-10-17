using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab3
{
    public interface ITransactionContext
    {
        Guid Id { get; }
        IUnitOfWork UnitOfWork { get; }
    }
}
