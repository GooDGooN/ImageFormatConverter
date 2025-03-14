using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ImageFormatConverter.ViewModel;

public static class ListboxSelectedItemsBehavior
{
    public static readonly DependencyProperty SelectedItemsProperty =
        DependencyProperty.RegisterAttached(
            "SelectItems",
            typeof(IList),
            typeof(ListboxSelectedItemsBehavior),
            new PropertyMetadata(null, PropertyChanged)
            );
    private static bool isChanging = false;

    public static IList GetSelectItems(DependencyObject dpo)
    {
        return (IList)dpo.GetValue(SelectedItemsProperty);
    }

    public static void SetSelectItems(DependencyObject dpo, IList value)
    {
        dpo.SetValue(SelectedItemsProperty, value);
    }

    private static void PropertyChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs e)
    {
        if (dpo is ListBox listbox)
        {
            listbox.SelectionChanged -= SelectItemChanged;
            if (e.NewValue is IList targetList)
            {
                listbox.SelectionChanged += SelectItemChanged;
                targetList.Clear();
                foreach (var item in listbox.SelectedItems)
                {
                    targetList.Add(item);
                }
            }
        }
    }

    private static void SelectItemChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is ListBox listbox)
        {
            if(!isChanging)
            {
                var bindTargetList = GetSelectItems(listbox);
                try
                {
                    isChanging = true;
                    if (bindTargetList != null)
                    {
                        foreach (var item in e.RemovedItems)
                        {
                            bindTargetList.Remove(item);
                        }
                        foreach (var item in e.AddedItems)
                        {
                            bindTargetList.Add(item);
                        }
                    }
                }
                finally
                {
                    isChanging = false;
                }
            }
        }
    }
}
