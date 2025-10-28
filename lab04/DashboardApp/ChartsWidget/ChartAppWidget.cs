using System.Composition;
using System.Windows.Controls;
using Contracts;

namespace ChartsWidget;

[Export(typeof(IAppWidget))]
public class ChartAppWidget : IAppWidget
{
    private const string AppWidgetName = "Chart Widget";
    private readonly ChartWidget _widget = new();

    [ImportingConstructor]
    public ChartAppWidget(IEventAggregator eventAggregator)
    {
        eventAggregator.GetEvent<DataUpdatedEvent>().Subscribe(_widget.ApplyEvent);
    }

    [ExportMetadata(AppWidgetMetadata.Name, AppWidgetName)]
    public string Name => AppWidgetName;
    public UserControl Control => _widget;
}