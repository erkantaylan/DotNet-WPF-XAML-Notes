using System;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;

public class SelectableDataGridBehaviour : Behavior<DataGrid>
{
    public event EventHandler DataGridSelectionChanged;

    protected override void OnAttached()
    {
        AssociatedObject.SelectionChanged += AssociatedObjectOnSelectionChanged;
    }

    private void AssociatedObjectOnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        foreach (object item in e.AddedItems)
        {
            if (item is ISelectable selectable)
            {
                selectable.IsSelected = true;
            }
        }

        foreach (object item in e.RemovedItems)
        {
            if (item is ISelectable selectable)
            {
                selectable.IsSelected = false;
            }
        }

        DataGridSelectionChanged?.Invoke(sender, e);
    }


    protected override void OnDetaching()
    {
        AssociatedObject.SelectionChanged -= AssociatedObjectOnSelectionChanged;
    }
}

public interface ISelectable
{
    bool IsSelected { get; set; }
}
