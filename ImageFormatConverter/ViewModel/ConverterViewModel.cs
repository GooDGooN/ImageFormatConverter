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
    public ICommand OnRemoveButton => new RelayCommand(RemoveSelectedInList);

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
        dialog.Filter = "Image Files (*.png, *.gif, *.jpeg, *.bmp, *.webp, *.ico|*.png;*.gif;*.jpeg;*.bmp;*.webp;*.ico|All files (*.*)|*.*";
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

            var dir = dialog.FolderName;
            if (!Directory.Exists(dir))
            {
                if(dir == "")
                {
                    return;
                }
                throw new Exception("Wrong directory!");
            }

            if (IsCreateNewFolder)
            {
                Directory.CreateDirectory(Path.Combine(dir, @"\ImageFormatConvert"));
            }

            var formats = new ImageFormat[] { ImageFormat.Png, ImageFormat.Gif, ImageFormat.Jpeg, ImageFormat.Bmp, ImageFormat.Webp, ImageFormat.Icon };
            var formatStr = ((TargetImageFormat)FormatIndex).ToString();
            foreach (var file in GetItemsFromModel)
            {
                var fileName = Path.Combine(dir, Path.ChangeExtension(file, formatStr));
                try
                {
                    using (var image = Image.FromFile(file))
                    {
                        image.Save(file, formats[FormatIndex]);
                    }
                }
                catch
                {
                    MessageBox.Show("ERROR:There is no Folder!\ndid you delete it while converting?");
                }
            }

            MessageBox.Show($"saved at {dir}");
        }
        else
        {
            MessageBox.Show($"There is nothing to convert!");
        }
    }

    private void RemoveSelectedInList() 
    {
        var items = GetItemsFromModel.Where(item => GetCurrentSelectedItems.Contains(item)).ToArray();
        foreach(var item in items)
        {
            GetItemsFromModel.Remove(item);
        }
    }
    private bool IsImageFormat(string filePath)
    {
        try
        {
            using (var image = Image.FromFile(filePath))
            {
                if(image != null)
                {
                    return true;
                }
                return false;
            }
        }
        catch
        {
            return false;
        }
    }

}
