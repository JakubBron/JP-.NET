using System.Composition;
using System.Windows;
using System.Windows.Controls;
using Contracts;

namespace DashboardApp;

[Export]
public partial class Dashboard : Window
{
    [Import]
    public IEventAggregator EventAggregator { get; set; } = null!;

    private WidgetManager _widgetManager = null!;

    public Dashboard()
    {
        InitializeComponent();
        InitializeEventHandlers();
    }

    [OnImportsSatisfied]
    public void OnImportsSatisfied()
    {
        _widgetManager = new WidgetManager(EventAggregator);
        _widgetManager.WidgetsChanged += OnWidgetsChanged;
        _widgetManager.LoadWidgets();
    }

    private void InitializeEventHandlers()
    {
        SendButton.Click += SendButton_Click;
    }

    private void SendButton_Click(object sender, RoutedEventArgs e)
    {
        var data = InputTextBox.Text;

        if (string.IsNullOrWhiteSpace(data))
        {
            return;
        }

        EventAggregator.GetEvent<DataUpdatedEvent>().Publish(new DataUpdatedEventValue(data));
    }

    public void AddWidget(IAppWidget widget)
    {
        var tabItem = new TabItem { Header = widget.Name, Content = widget.Control };

        WidgetsTabControl.Items.Add(tabItem);
    }

    public void RemoveWidget(int index)
    {
        if (index >= 0 && index < WidgetsTabControl.Items.Count)
        {
            WidgetsTabControl.Items.RemoveAt(index);
        }
    }

    public void ClearWidgets()
    {
        WidgetsTabControl.Items.Clear();
    }

    public void OnWidgetsChanged(object? sender, EventArgs e)
    {
        Dispatcher.Invoke(() =>
        {
            ClearWidgets();

            if (_widgetManager.Widgets == null) return;
            foreach (var widget in _widgetManager.Widgets)
            {
                try
                {
                    widget.Control.DataContext = EventAggregator;
                    AddWidget(widget);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Error while loading widget {widget.Name}: {ex.Message}",
                        "Widget Load Error occured!",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error
                    );
                }
            }
        });
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        _widgetManager.Dispose();
    }
}