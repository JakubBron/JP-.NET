using Prism.Events;
using System.Composition;

namespace DashboardApp;

[Export(typeof(IEventAggregator))]
[Shared]
public class CustomEventAggregator : EventAggregator;
