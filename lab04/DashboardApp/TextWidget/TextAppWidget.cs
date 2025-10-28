using System.Composition;
using Contracts;
using System.Windows.Controls;

namespace TextWidget;

[Export(typeof(IAppWidget))]
public class TextAppWidget : IAppWidget
{
    private const string AppWidgetName = "Text Widget";
    private readonly TextWidget _widget = new();

    [ImportingConstructor]
    public TextAppWidget(IEventAggregator eventAggregator)
    {
        eventAggregator.GetEvent<DataUpdatedEvent>().Subscribe(_widget.ApplyEvent);
    }

    [ExportMetadata(AppWidgetMetadata.Name, AppWidgetName)]
    public string Name => AppWidgetName;
    public UserControl Control => _widget;
}