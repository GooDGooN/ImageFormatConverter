using ImageFormatConverter.Model;
using ImageFormatConverter.Utility;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace ImageFormatConverter.ViewModel;

public partial class ConverterViewModel
{
    public ICommand OnImportFolderButton => new RelayCommand(FolderImport);
    public ICommand OnImportFileButton => new RelayCommand(FileImport);
    public ICommand OnExportButton => new RelayCommand(ExportImages);
    public ICommand OnRemoveButton => new RelayCommand(RemoveSelected);

    private void FolderImport()
    {
        var dialog = new OpenFolderDialog();
        dialog.Title = "Select import target folder";

        if (dialog.ShowDialog() ?? false)
        {
            var count = ImageManager.GetImageDirectorys(model.ListItems, dialog.FolderName);
            MessageBox.Show($"{count} Files imported");
        }
    }

    private void FileImport()
    {
        var dialog = new OpenFileDialog();
        dialog.Title = "Select import target Image Files";
        dialog.Multiselect = true;
        dialog.Filter = "Image Files (*.png, *.gif, *.jpeg, *.bmp, *.webp, *.ico|*.png;*.gif;*.jpeg;*.bmp;*.webp;*.ico|All files (*.*)|*.*";

        if (dialog.ShowDialog() ?? false)
        {
            var count = ImageManager.GetImageDirectorys(model.ListItems, dialog.FileNames);
            MessageBox.Show($"{count} Files imported");
        }
    }
    private void ExportImages()
    {
        if (GetItemsFromModel.Count > 0)
        {
            var dialog = new OpenFolderDialog();
            dialog.Title = "Select export location";

            if (!dialog.ShowDialog() ?? false)
            {
                return;
            }

            var dir = dialog.FolderName;

            if (!Directory.Exists(dir))
            {
                MessageBox.Show("Worng Action!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            ImageManager.ExportImages(model.ListItems, dir, (TargetImageFormat)FormatIndex, IsCreateNewFolder);
        }
        else
        {
            MessageBox.Show($"There is nothing to convert!");
        }
    }

    private void RemoveSelected()
    {
        var items = GetItemsFromModel.Where(item => GetCurrentSelectedItems.Contains(item)).ToArray();

        foreach (var item in items)
        {
            GetItemsFromModel.Remove(item);
        }
    }
}
