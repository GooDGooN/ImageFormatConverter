using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ImageFormatConverter.Model;

public enum TargetImageFormat
{
    png,
    gif,
    jpeg,
    bmp,
    webp,
    icon,
}
public class ConverterModel
{
    public string SaveDirection = "";
    public ObservableCollection<string> ListItems = new();
    public ObservableCollection<string> FormatItems = ["PNG", "GIF", "JPEG", "BMP", "WEBP", "ICON"];
    public ObservableCollection<string> SelectedItem = new();
    public int TargetFormatIndex;
    public bool IsCreateNewFolder = true;
}
