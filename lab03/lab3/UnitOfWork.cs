using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using lab3;

namespace lab3
{
    public class UnitOfWork : IUnitOfWork
    {
        public Guid Id { get; }
        public UnitOfWork()
        {
            Id = Guid.NewGuid();
        }
    }
}
