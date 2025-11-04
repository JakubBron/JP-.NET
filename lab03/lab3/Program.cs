using Autofac;
using Autofac.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System;
using Autofac.Features.Metadata;
using lab3;


class Program
{
    static void Main(string[] args)
    {

        var builder = new ContainerBuilder();

        // Konfiguracja w kodzie
        builder.RegisterType<CatCalc>().Named<ICalculator>("cat");
        builder.RegisterType<PlusCalc>().Named<ICalculator>("plus");
        builder.RegisterType<StateCalc>().Named<ICalculator>("statecalc").WithParameter("iter", 0).SingleInstance();

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

        builder.RegisterType<Worker2>().OnActivated(e => e.Instance.SetCalculator(e.Context.ResolveNamed<ICalculator>("plus")));
        builder.RegisterType<Worker2>().Named<Worker2>("state").OnActivated(e => e.Instance.SetCalculator(e.Context.ResolveNamed<ICalculator>("statecalc")));

        builder.RegisterType<Worker3>().OnActivated(e => e.Instance.calculator = e.Context.ResolveNamed<ICalculator>("cat"));
        builder.RegisterType<Worker3>().Named<Worker3>("state").OnActivated(e => e.Instance.calculator = e.Context.ResolveNamed<ICalculator>("statecalc"));


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


        var container = builder.Build();



        // tests MINE
        using (var scope = container.BeginLifetimeScope())
        {
            var w1 = scope.Resolve<Worker>();
            w1.Work("a", "b"); // ab

            var w2 = scope.Resolve<Worker2>();
            w2.Work("10", "15"); // 25

            var w3 = scope.ResolveNamed<Worker3>("state");
            w3.Work("x", "y"); // xy1
            w3.Work("a", "b"); // ab2

            var processor = scope.Resolve<TransactionProcessor>();

            Console.WriteLine("=== TRANSACTION 1 ===");
            processor.ProcessTransaction();

            Console.WriteLine("=== TRANSACTION 2 ===");
            processor.ProcessTransaction();

        }
        Console.WriteLine("\nappsetting.json\n");

        /* Konfiguracja przez appsetting.json */
        
        var config2 = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        var builder2 = new ContainerBuilder();

        
        builder2.RegisterModule(new ConfigurationModule(config2.GetSection("autofac")));
        builder2.RegisterType<CatCalc>().Named<ICalculator>("cat");
        builder2.RegisterType<PlusCalc>().Named<ICalculator>("plus");
        builder2.RegisterType<StateCalc>().Named<ICalculator>("statecalc").WithParameter("iter", 0).SingleInstance();

        builder2.RegisterType<Worker>()
            .WithParameter(
                (pi, ctx) => pi.ParameterType == typeof(ICalculator),
                (pi, ctx) => ctx.ResolveNamed<ICalculator>("cat")
            );

        builder2.RegisterType<Worker>()
            .Named<Worker>("state")
            .WithParameter(
                (pi, ctx) => pi.ParameterType == typeof(ICalculator),
                (pi, ctx) => ctx.ResolveNamed<ICalculator>("statecalc")
            );

        builder2.RegisterType<Worker2>().OnActivated(e => e.Instance.SetCalculator(e.Context.ResolveNamed<ICalculator>("plus")));
        builder2.RegisterType<Worker2>().Named<Worker2>("state").OnActivated(e => e.Instance.SetCalculator(e.Context.ResolveNamed<ICalculator>("statecalc")));

        builder2.RegisterType<Worker3>().OnActivated(e => e.Instance.calculator = e.Context.ResolveNamed<ICalculator>("cat"));
        builder2.RegisterType<Worker3>().Named<Worker3>("state").OnActivated(e => e.Instance.calculator = e.Context.ResolveNamed<ICalculator>("statecalc"));


        // Rejestracja UnitOfWork
        builder2.RegisterType<UnitOfWork>()
            .As<IUnitOfWork>()
            .InstancePerLifetimeScope();

        // Rejestracja TransactionContext
        builder2.RegisterType<TransactionContext>()
            .As<ITransactionContext>()
            .InstancePerMatchingLifetimeScope("transaction");

        // Rejestracja serwisów 
        builder2.RegisterType<StepOneService>().AsSelf();
        builder2.RegisterType<StepTwoService>().AsSelf();
        builder2.RegisterType<TransactionProcessor>().AsSelf();
        builder2.RegisterType<CatCalc>().Named<ICalculator>("cat");
        builder2.RegisterType<PlusCalc>().Named<ICalculator>("plus");
        builder2.RegisterType<StateCalc>().Named<ICalculator>("statecalc").WithParameter("iter", 0).SingleInstance();

        builder2.RegisterType<Worker>()
            .WithParameter(
                (pi, ctx) => pi.ParameterType == typeof(ICalculator),
                (pi, ctx) => ctx.ResolveNamed<ICalculator>("cat")
            );

        builder2.RegisterType<Worker>()
            .Named<Worker>("state")
            .WithParameter(
                (pi, ctx) => pi.ParameterType == typeof(ICalculator),
                (pi, ctx) => ctx.ResolveNamed<ICalculator>("statecalc")
            );

        builder2.RegisterType<Worker2>().OnActivated(e => e.Instance.SetCalculator(e.Context.ResolveNamed<ICalculator>("plus")));
        builder2.RegisterType<Worker2>().Named<Worker2>("state").OnActivated(e => e.Instance.SetCalculator(e.Context.ResolveNamed<ICalculator>("statecalc")));

        builder2.RegisterType<Worker3>().OnActivated(e => e.Instance.calculator = e.Context.ResolveNamed<ICalculator>("cat"));
        builder2.RegisterType<Worker3>().Named<Worker3>("state").OnActivated(e => e.Instance.calculator = e.Context.ResolveNamed<ICalculator>("statecalc"));


        // Rejestracja UnitOfWork
        builder2.RegisterType<UnitOfWork>()
            .As<IUnitOfWork>()
            .InstancePerLifetimeScope();

        // Rejestracja TransactionContext
        builder2.RegisterType<TransactionContext>()
            .As<ITransactionContext>()
            .InstancePerMatchingLifetimeScope("transaction");

        // Rejestracja serwisów 
        builder2.RegisterType<StepOneService>().AsSelf();
        builder2.RegisterType<StepTwoService>().AsSelf();
        builder2.RegisterType<TransactionProcessor>().AsSelf();

        var container2 = builder2.Build();

        // tests MINE
        using (var scope = container2.BeginLifetimeScope())
        {
            var w1 = scope.Resolve<Worker>();
            w1.Work("a", "b"); // ab

            var w2 = scope.Resolve<Worker2>();
            w2.Work("10", "15"); // 25

            var w3 = scope.ResolveNamed<Worker3>("state");
            w3.Work("x", "y"); // xy1
            w3.Work("a", "b"); // ab2

            var processor = scope.Resolve<TransactionProcessor>();

            Console.WriteLine("=== TRANSACTION 1 ===");
            processor.ProcessTransaction();

            Console.WriteLine("=== TRANSACTION 2 ===");
            processor.ProcessTransaction();

        }

    }

}
