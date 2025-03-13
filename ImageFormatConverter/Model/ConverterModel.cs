using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageFormatConverter.Model;
public class ConverterModel
{
    public List<string> ImportedImageFiles = new();
    public string? SaveDirection;
    public ObservableCollection<string> ListItems = new();

    public ConverterModel()
    {
        ResetField();
    }

    public void ResetField()
    {
        SaveDirection = "";
        ImportedImageFiles.Clear();
    }
}
