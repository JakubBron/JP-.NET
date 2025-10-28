using System.Windows.Controls;
using Contracts;

namespace TextWidget;

public partial class TextWidget : UserControl
{
    public TextWidget()
    {
        InitializeComponent();
    }

    public void ApplyEvent(DataUpdatedEventValue @event)
    {
        Dispatcher.Invoke(() =>
        {
            ReceivedTextBlock.Text = @event.Data;
            CharCountText.Text = @event.Data.Length.ToString();
            WordCountText.Text = @event.Data.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length.ToString();
        });
    }
}