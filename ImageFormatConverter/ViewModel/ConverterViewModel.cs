using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using ImageFormatConverter.Model;
using ImageFormatConverter.Utility;


namespace ImageFormatConverter.ViewModel;
public partial class ConverterViewModel
{
    private ConverterModel model;
    private ICommand dropImageCommand;
    public ICommand DropImageCommand
    {
        get => dropImageCommand;
        set => dropImageCommand.Execute(value);
    }

    public ObservableCollection<string> GetItemsFromModel => model.ListItems;
    public ObservableCollection<string> GetFormatsFromModel => model.FormatItems;
    public ObservableCollection<string> GetCurrentSelectedItems => model.SelectedItem;

    public int FormatIndex
    {
        get => model.TargetFormatIndex;
        set => model.TargetFormatIndex = value;
    }

    public bool IsCreateNewFolder
    {
        get => model.IsCreateNewFolder;
        set => model.IsCreateNewFolder = value;
    }

    public ConverterViewModel()
    {
        dropImageCommand = new RelayCommand<DragEventArgs>(DropImage);
        model = new();
    }

    public void DropImage(DragEventArgs e)
    {
        var droptedItems = e.Data.GetData(DataFormats.FileDrop) as string[];
        model.SaveDirection = "";

        if (droptedItems != null)
        {
            ImageManager.GetImageDirectorys(model.ListItems, droptedItems);
        }
    }
}
