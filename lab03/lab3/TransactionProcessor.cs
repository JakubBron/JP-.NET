using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab3
{
    public class TransactionProcessor
    {
        public readonly ILifetimeScope _scope;

        public TransactionProcessor(ILifetimeScope scope)
        {
            _scope = scope;
        }

        public void ProcessTransaction()
        {
            // Utwórz nowy scope "transaction"
            using (var txScope = _scope.BeginLifetimeScope("transaction"))
            {
                var step1 = txScope.Resolve<StepOneService>();
                var step2 = txScope.Resolve<StepTwoService>();

                step1.Execute();
                step2.Execute();
            }
        }
    }

}
