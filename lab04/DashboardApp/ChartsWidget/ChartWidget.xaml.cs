using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Contracts;
using Contracts;

namespace ChartsWidget;

/// <summary>
/// Interaction logic for ChartWidget.xaml
/// </summary>
public partial class ChartWidget : UserControl
{
    public ChartWidget()
    {
        InitializeComponent();
    }

    public void ApplyEvent(DataUpdatedEventValue @event)
    {
        Dispatcher.Invoke(() =>
        {
            ChartCanvas.Children.Clear();

            var values = GetValues(@event.Data).ToArray();

            if (values.Length == 0)
            {
                ErrorTextBlock.Visibility = Visibility.Visible;
                ErrorTextBlock.Text =
                    "Cannot parse the values, no values given!. Enter only numbers, space separated (e.g. 10 50 30 80).";
                return;
            }

            ErrorTextBlock.Visibility = Visibility.Collapsed;

            DrawChart(values);
        });
    }

    private void DrawChart(double[] values)
    {
        if (values.Length == 0)
            return;

        const double margin = 30;
        const double width = 40;
        const double spacing = 30;

        var maxValue = values.Max();
        //maxValue = maxValue == 0 ? 1 : maxValue;
        if (maxValue == 0) {
            maxValue = 1;
        }
        
        
        var availableHeight = ChartCanvas.ActualHeight > 0 ? ChartCanvas.ActualHeight - 2 * margin : 300 - 2 * margin;
        var totalWidth = values.Length * (width + spacing) + 2 * margin;

        ChartCanvas.Width = totalWidth;
        ChartCanvas.Height = ChartCanvas.ActualHeight > 0 ? ChartCanvas.ActualHeight : 300;

        var colors = new[]
        {
            Brushes.DodgerBlue,
            Brushes.Orange,
            Brushes.MediumSeaGreen,
            Brushes.Crimson,
        };

        for (var i = 0; i < values.Length; i++)
        {
            var height = values[i] / maxValue * availableHeight;
            var x = margin + i * (width + spacing) + spacing;
            var y = ChartCanvas.Height - margin - height;

            var rectangle = new Rectangle
            {
                Width = width,
                Height = height,
                Fill = colors[i % colors.Length]
            };
            Canvas.SetLeft(rectangle, x);
            Canvas.SetTop(rectangle, y);
            ChartCanvas.Children.Add(rectangle);

            var valueText = new TextBlock
            {
                Text = values[i].ToString("F1"),
                FontSize = 12,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.Black,
            };
            Canvas.SetLeft(valueText, x + width / 2 - 15);
            Canvas.SetTop(valueText, y - 20);
            ChartCanvas.Children.Add(valueText);
            var label = new TextBlock
            {
                Text = (i + 1).ToString(),
                FontSize = 12,
                Foreground = Brushes.Black,
            };
            Canvas.SetLeft(label, x + width / 2 - 5);
            Canvas.SetTop(label, ChartCanvas.Height - margin + 5);
            ChartCanvas.Children.Add(label);
        }
    }


    private static IEnumerable<double> GetValues(string text)
    {
        foreach (var token in text.Split(' ', StringSplitOptions.RemoveEmptyEntries))
        {
            if (double.TryParse(token, NumberStyles.Any, CultureInfo.InvariantCulture, out var number)
                || double.TryParse(token, NumberStyles.Any, CultureInfo.CurrentCulture, out number))
            {
                yield return number;
            }
        }
    }
}