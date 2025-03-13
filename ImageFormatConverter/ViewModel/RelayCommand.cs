using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ImageFormatConverter.ViewModel;
public class RelayCommand<T> : ICommand
{
    public event EventHandler? CanExecuteChanged;
    private Action<T> targetExecute;

    public RelayCommand(Action<T> targetExcute)
    {
        this.targetExecute = targetExcute;
    }

    public bool CanExecute(object? parameter)
    {
        return true;
    }

    public void Execute(object? parameter)
    {
        if (parameter is T typedParam)
        {
            targetExecute((T)parameter);
        }
    }
}
