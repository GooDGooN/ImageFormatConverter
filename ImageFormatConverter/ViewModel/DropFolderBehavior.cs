using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace ImageFormatConverter.ViewModel;
public static class DropFolderBehavior
{
    public static readonly DependencyProperty DragDropProperty =
        DependencyProperty.RegisterAttached(
            "DropFolder",
            typeof(ICommand),
            typeof(DropFolderBehavior),
            new PropertyMetadata(null, PropertyChanged)
            );

    
    public static void SetDropFolder(DependencyObject dpo, ICommand value) 
    {
        dpo.SetValue(DragDropProperty, value);
    }
    public static ICommand GetDropFolder(DependencyObject dpo)
    {
        return (ICommand)dpo.GetValue(DragDropProperty); 
    }

    private static void PropertyChanged(DependencyObject dpo, DependencyPropertyChangedEventArgs e)
    {
        if (dpo is UIElement uiControl)
        {
            if (e.OldValue != null)
            {
                uiControl.Drop -= ExecuteDrop;
            }
            if (e.NewValue != null)
            {
                uiControl.Drop += ExecuteDrop;
            }
        }
    }

    private static void ExecuteDrop(object sender, DragEventArgs e)
    {
        if (sender is DependencyObject dependencyObject)
        {
            var command = (ICommand)dependencyObject.GetValue(DragDropProperty);
            if (command != null)
            {
                command.Execute(e);
            }
        }
    }
}