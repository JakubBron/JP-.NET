using lab3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab3
{
    public interface IUnitOfWork
    {
        Guid Id { get;  } 
    }
}