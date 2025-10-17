using Autofac;
using lab3;

namespace Testy
{
    public class Testy
    {
        private readonly IContainer _container;
        public Testy()
        {
            var builder = new ContainerBuilder();

            /* Konfiguracja w kodzie */
            builder.RegisterType<CatCalc>().Named<ICalculator>("cat");
            builder.RegisterType<PlusCalc>().Named<ICalculator>("plus");
            builder.RegisterType<StateCalc>().Named<ICalculator>("statecalc").WithParameter("iter", 1).SingleInstance();

            builder.RegisterType<Worker>()
                .WithParameter(
                    (pi, ctx) => pi.ParameterType == typeof(ICalculator),
                    (pi, ctx) => ctx.ResolveNamed<ICalculator>("cat")
                );

            builder.RegisterType<Worker>()
                .Named<Worker>("state")
                .WithParameter(
                    (pi, ctx) => pi.ParameterType == typeof(ICalculator),
                    (pi, ctx) => ctx.ResolveNamed<ICalculator>("statecalc")
                );

            builder.RegisterType<Worker2>()
                .OnActivated(e => e.Instance.SetCalculator(e.Context.ResolveNamed<ICalculator>("plus")));
            builder.RegisterType<Worker2>().Named<Worker2>("state").OnActivated(e =>
                e.Instance.SetCalculator(e.Context.ResolveNamed<ICalculator>("statecalc")));

            builder.RegisterType<Worker3>()
                .OnActivated(e => e.Instance.calculator = e.Context.ResolveNamed<ICalculator>("cat"));
            builder.RegisterType<Worker3>().Named<Worker3>("state").OnActivated(e =>
                e.Instance.calculator = e.Context.ResolveNamed<ICalculator>("statecalc"));


            // Rejestracja UnitOfWork
            builder.RegisterType<UnitOfWork>()
                .As<IUnitOfWork>()
                .InstancePerLifetimeScope();

            // Rejestracja TransactionContext
            builder.RegisterType<TransactionContext>()
                .As<ITransactionContext>()
                .InstancePerMatchingLifetimeScope("transaction");

            // Rejestracja serwisów 
            builder.RegisterType<StepOneService>().AsSelf();
            builder.RegisterType<StepTwoService>().AsSelf();
            builder.RegisterType<TransactionProcessor>().AsSelf();


            _container = builder.Build();
        }


        [Fact]
        public void Worker_Uses_CatCalc_ByDefault()
        {
            using (var scope = _container.BeginLifetimeScope())
            {
                var worker = scope.Resolve<Worker>();
                var result = worker.Work("A", "B");
                Assert.Equal("AB", result);
            }
        }

        [Fact]
        public void Worker2_Uses_PlusCalc_ByDefault()
        {
            using var scope = _container.BeginLifetimeScope();
            var worker2 = scope.Resolve<Worker2>();
            var result = worker2.Work("10", "15");
            Assert.Equal("25", result);
        }

        [Fact]
        public void Worker3_Uses_CatCalc_ByDefault()
        {
            using var scope = _container.BeginLifetimeScope();
            var worker3 = scope.Resolve<Worker3>();
            var result = worker3.Work("X", "Y");
            Assert.Equal("XY", result);
        }

        [Fact]
        public void Worker_Named_State_Uses_StateCalc()
        {
            using var scope = _container.BeginLifetimeScope();
            var worker = scope.ResolveNamed<Worker>("state");
            var result1 = worker.Work("a", "b");
            var result2 = worker.Work("c", "d");
            Assert.Equal("ab1", result1);
            Assert.Equal("cd2", result2);
        }

        [Fact]
        public void StateCalc_Increments_Within_Same_ContainerScope()
        {
            using var scope = _container.BeginLifetimeScope();
            var s1 = scope.ResolveNamed<ICalculator>("statecalc");
            var w1 = scope.ResolveNamed<Worker>("state");
            var r1 = w1.Work("x", "y"); // xy1
            var w2 = scope.ResolveNamed<Worker2>("state");
            var r2 = w2.Work("p", "q"); // pq2
            Assert.Equal("xy1", r1);
            Assert.Equal("pq2", r2);
        }


        [Fact]
        public void Worker3_Named_State_Uses_StateCalc()
        {
            using var scope = _container.BeginLifetimeScope();
            var worker3 = scope.ResolveNamed<Worker3>("state");
            var result1 = worker3.Work("k", "l");
            var result2 = worker3.Work("m", "n");
            Assert.Equal("kl1", result1);
            Assert.Equal("mn2", result2);
        }

        [Fact]
        public void StateCalc_ShouldBe_Singleton()
        {
            using var scope = _container.BeginLifetimeScope();
            var state1 = scope.ResolveNamed<ICalculator>("statecalc");
            var state2 = scope.ResolveNamed<ICalculator>("statecalc");
            Assert.Same(state1, state2);
        }

        // === CZĘŚĆ 3 TESTY ===

        [Fact]
        public void UnitOfWork_ShouldBe_Same_WithinScope_ButDifferentAcrossScopes()
        {
            IUnitOfWork uow1a, uow1b, uow2a;
            using (var scope1 = _container.BeginLifetimeScope())
            {
                uow1a = scope1.Resolve<IUnitOfWork>();
                uow1b = scope1.Resolve<IUnitOfWork>();
                Assert.Same(uow1a, uow1b);
            }

            using (var scope2 = _container.BeginLifetimeScope())
            {
                uow2a = scope2.Resolve<IUnitOfWork>();
                Assert.NotSame(uow1a, uow2a);
            }
        }

        [Fact]
        public void TransactionContext_ShouldBe_Same_For_Services_In_TransactionScope()
        {
            using (var scope = _container.BeginLifetimeScope())
            using (var txScope = scope.BeginLifetimeScope("transaction"))
            {
                var stepOne = txScope.Resolve<StepOneService>();
                var stepTwo = txScope.Resolve<StepTwoService>();

                var ctx1 = stepOne._context;
                var ctx2 = stepTwo._context;

                Assert.Same(ctx1, ctx2);
            }
        }

        [Fact]
        public void TransactionContext_ShouldBe_Different_In_Separate_Transactions()
        {
            using (var scope = _container.BeginLifetimeScope())
            {
                ITransactionContext t1, t2;
                using (var tx1 = scope.BeginLifetimeScope("transaction"))
                {
                    t1 = tx1.Resolve<StepOneService>()._context;
                }

                using (var tx2 = scope.BeginLifetimeScope("transaction"))
                {
                    t2 = tx2.Resolve<StepOneService>()._context;
                }

                Assert.NotSame(t1, t2);
            }
        }
    }
}
