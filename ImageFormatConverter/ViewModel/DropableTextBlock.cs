using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ImageFormatConverter.ViewModel;
public static class DropFolderBehavior
{
    public static readonly DependencyProperty DragDropProperty =
        DependencyProperty.RegisterAttached(
            "DropFolder",
            typeof(ICommand),
            typeof(DropFolderBehavior),
            new PropertyMetadata(null, PropertyMetadataChanged)
            );

    
    public static void SetDropFolder(DependencyObject obj, ICommand value) 
    {
        obj.SetValue(DragDropProperty, value);
    }
    public static ICommand GetDropFolder(DependencyObject obj)
    {
        return (ICommand)obj.GetValue(DragDropProperty); 
    }

    private static void PropertyMetadataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is UIElement uiControl)
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