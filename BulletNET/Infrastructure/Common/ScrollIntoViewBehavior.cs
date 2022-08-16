using Microsoft.Xaml.Behaviors;
using System.Windows.Controls;

namespace BulletNET.Infrastructure.Common;

internal class ScrollIntoViewBehavior : Behavior<DataGrid>
{
    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.SelectionChanged += AssociatedObject_SelectionChanged;
    }

    private void AssociatedObject_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is not DataGrid grid || grid.SelectedItem == null) return;

        grid.Dispatcher.BeginInvoke(() =>
        {
            grid.UpdateLayout();
            grid.ScrollIntoView(grid.SelectedItem, null);
        });
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        AssociatedObject.SelectionChanged -= AssociatedObject_SelectionChanged;
    }
}