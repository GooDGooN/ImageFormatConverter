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
using System.Drawing.Imaging;


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
    public ICommand OnExportButton => new RelayCommand(ExportImages);


    public ObservableCollection<string> GetItemsFromModel => model.ListItems;
    public ObservableCollection<string> GetFormatsFromModel => model.FormatItems;

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
    private void ExportImages()
    {
        if (GetItemsFromModel.Count > 0)
        {
            var dialog = new OpenFolderDialog();
            dialog.Title = "Select export location";
            dialog.ShowDialog();
            if(dialog.FolderName != "")
            {
                var dir = dialog.FolderName;
                if (IsCreateNewFolder)
                {
                    dir += @"\ImageFormatConvert";
                    Directory.CreateDirectory(dir);
                }

                foreach(var file in GetItemsFromModel)
                {
                    var format = ImageFormat.Png;
                    var formatStr = ".png";
                    var targetEnum = (TargetImageFormat)FormatIndex;
                    switch (targetEnum)
                    {
                        case TargetImageFormat.gif:
                            format = ImageFormat.Gif;
                            formatStr = ".gif";
                            break;
                        case TargetImageFormat.jpeg: 
                            format = ImageFormat.Jpeg;
                            formatStr = ".jpeg";
                            break;
                        case TargetImageFormat.bmp: 
                            format = ImageFormat.Bmp;
                            formatStr = ".bmp";
                            break;
                        case TargetImageFormat.webp: 
                            format = ImageFormat.Webp;
                            formatStr = ".webp";
                            break;
                        case TargetImageFormat.icon: 
                            format = ImageFormat.Icon;
                            formatStr = ".icon";
                            break;
                    }
                    var strLeftIndex = file.Length - 1;
                    var strRightIndex = file.Length - 1;
                    while (true)
                    {
                        if (file[strLeftIndex--] == 92)
                        {
                            break;
                        }
                        if (file[strRightIndex] != 46)
                        {
                            strRightIndex--;
                        }
                    }
                    var fileName = dir + file.Substring(strLeftIndex + 1, strRightIndex - strLeftIndex - 1) + formatStr;
                    MessageBox.Show(fileName);
                    Image.FromFile(file).Save(fileName, format);
                }

                MessageBox.Show($"saved at {dir}");
            }
        }
        else
        {
            MessageBox.Show($"There is nothing to convert!");
        }
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
