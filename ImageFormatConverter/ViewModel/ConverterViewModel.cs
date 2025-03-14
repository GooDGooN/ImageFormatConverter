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
using System.ComponentModel;


namespace ImageFormatConverter.ViewModel;
public class ConverterViewModel
{
    private ConverterModel model;
    private ICommand dropImageCommand;
    public ICommand DropImageCommand
    {
        get => dropImageCommand;
        set => dropImageCommand.Execute(value);
    }

    public ICommand OnImportFolderButton => new RelayCommand(FolderImport);
    public ICommand OnImportFileButton => new RelayCommand(FileImport);
    public ICommand OnExportButton => new RelayCommand(ExportImages);
    public ICommand OnRemoveButton => new RelayCommand(RemoveSelected);


    public ObservableCollection<string> GetItemsFromModel => model.ListItems;
    public ObservableCollection<string> GetFormatsFromModel => model.FormatItems;
    public ObservableCollection<string> GetSelectedItems => model.SelectedItem;

    
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
        model.ResetField();

        if (droptedItems != null)
        {
            TryImportImages(droptedItems);
        }
    }

    private void TryImportImages(params string[] dirs)
    {
        if(dirs.Length > 0)
        {
            if (Directory.Exists(dirs[0]))
            {
                var directoryInfo = new DirectoryInfo(dirs[0]);
                foreach (var info in directoryInfo.GetFiles())
                {
                    if (IsImageFormat(info.FullName))
                    {
                        model.ListItems.Add(info.FullName);
                    }
                }
            }
            else
            {
                foreach (var dir in dirs)
                {
                    if (IsImageFormat(dir))
                    {
                        model.ListItems.Add(dir);
                    }
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

    private void FileImport()
    {
        var dialog = new OpenFileDialog();
        dialog.Title = "Select import target ImageFiles";
        dialog.Multiselect = true;
        dialog.Filter = "Image Files (*.png, *.gif, *.jpeg, *.bmp, *.webp, *.ico | *.png; *.gif; *.jpeg; *.bmp; *.webp; *.ico | All files (*.*) | *.*";
        dialog.ShowDialog();
        TryImportImages(dialog.FileNames);
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
                    try
                    {
                        Image.FromFile(file).Save(fileName, format);
                    }
                    catch
                    {
                        MessageBox.Show("ERROR:There is no file!\ndid you delete it while converting?");
                    }
                }

                MessageBox.Show($"saved at {dir}");
            }
        }
        else
        {
            MessageBox.Show($"There is nothing to convert!");
        }
    }

    private void RemoveSelected() 
    {
        while(GetSelectedItems.Count > 0)
        {
            model.ListItems.Remove(GetSelectedItems.First());
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
