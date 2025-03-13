using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ImageFormatConverter.Model;
using System.Drawing;
using Microsoft.Win32;


namespace ImageFormatConverter.ViewModel;
public class ConverterViewModel
{
    private ConverterModel model;
    private ICommand dropFolderCommand;
    public ICommand DropFolderCommand
    {
        get => dropFolderCommand;
        set => dropFolderCommand.Execute(value);
    }

    public ICommand OnImportButton => new RelayCommand(FolderImport);
    public ICommand OnConvertButton => new RelayCommand(ImagesConvert);


    public ObservableCollection<string> GetItemsFromModel => model.ListItems;
    public ObservableCollection<string> GetFormatsFromModel => model.FormatItems;

    public int SetFormatIndex
    {
        get => model.TargetFormatIndex;
        set => model.TargetFormatIndex = value;
    }

    public ConverterViewModel()
    {
        dropFolderCommand = new RelayCommand<DragEventArgs>(AddFolderCommand);
        model = new();
    }

    public void AddFolderCommand(DragEventArgs e)
    {
        var droptedItems = e.Data.GetData(DataFormats.FileDrop) as string[];
        model.ResetField();

        if (droptedItems?.Length == 1)
        {
            TryImportImages(droptedItems[0]);
        }
    }

    private void TryImportImages(string dir)
    {
        if (Directory.Exists(dir))
        {
            var directoryInfo = new DirectoryInfo(dir);
            foreach (var info in directoryInfo.GetFiles())
            {
                if (IsImageFormat(info.FullName))
                {
                    model.ImportedImageFiles.Add(info.FullName);
                    model.ListItems.Add(info.FullName);
                }
            }
        }
    }

    private void FolderImport()
    {
        var dialog = new OpenFolderDialog();
        dialog.Title = "Select import target folder";
        dialog.ShowDialog();
        TryImportImages(dialog.FolderName);
    }
    private void ImagesConvert()
    {
        
    }

    private bool IsImageFormat(string filePath)
    {
        try
        {
            var image = Image.FromFile(filePath);
            return true;
        }
        catch
        {
            return false;
        }
    }

}
