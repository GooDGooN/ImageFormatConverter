using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;


namespace ImageFormatConverter.ViewModel;
public class ConverterViewModel
{    
    private ICommand dropFolderCommand;
    public ICommand DropFolderCommand
    {
        get => dropFolderCommand;
        set => dropFolderCommand.Execute(value);
    }

    public ConverterViewModel()
    {
        dropFolderCommand = new RelayCommand<DragEventArgs>(AddFolderCommand);
    }

    public void AddFolderCommand(DragEventArgs e)
    {
        var droptedItems = e.Data.GetData(DataFormats.FileDrop) as string[];
        if (droptedItems?.Length == 1)
        {
            var directory = droptedItems[0];
            if (Directory.Exists(directory))
            {
                var directoryInfo = new DirectoryInfo(directory);
                
                var str = "";
                foreach(var info in directoryInfo.GetFiles())
                {
                    str += $"{info} \n";
                }
                MessageBox.Show($"{str}");
            }
        }
    }

}
