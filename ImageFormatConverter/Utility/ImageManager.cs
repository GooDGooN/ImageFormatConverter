using ImageFormatConverter.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;

namespace ImageFormatConverter.Utility;

public enum DialogType
{
    ImportFiles,
    ImportFolder,
    Export,
}

public class ImageManager
{
    private static ImageFormat[] formats = [
        ImageFormat.Png, 
        ImageFormat.Gif, 
        ImageFormat.Jpeg, 
        ImageFormat.Bmp, 
        ImageFormat.Webp, 
        ImageFormat.Icon,
    ];
    public static void GetImageDirectorys(Collection<string> targetCollection, params string[] names)
    {
        if (Directory.Exists(names[0]))
        {
            var files = new DirectoryInfo(names[0]).GetFiles();
            foreach (var info in files)
            {
                if (IsImageFormat(info.FullName))
                {
                    targetCollection.Add(info.FullName);
                }
            }
        }
        else
        {
            foreach (var info in names)
            {
                if (IsImageFormat(info))
                {
                    targetCollection.Add(info);
                }
            }
        }
    }

    public static void ExportImages(Collection<string> files, string directory, TargetImageFormat format, bool createDirectory)
    {
        if (createDirectory)
        {
            Directory.CreateDirectory(Path.Combine(directory, "ConvertedImage"));
        }
        foreach (var file in files)
        {
            var formatStr = format.ToString();
            var fileName = Path.ChangeExtension(file, formatStr);
            var directoryInsertIndex = fileName.LastIndexOf(Path.DirectorySeparatorChar);
            if (createDirectory)
            {
                fileName = fileName.Insert(directoryInsertIndex, Path.DirectorySeparatorChar + "ConvertedImage");
            }
            try
            {
                using (var image = Image.FromFile(file))
                {
                    image.Save(fileName, formats[(int)format]);
                }
            }
            catch
            {
                MessageBox.Show($"ERROR:{fileName}\nThere is no Folder!\ndid you delete folder while converting?");
                return;
            }
        }

        MessageBox.Show($"saved at {directory}");
    }

    private static bool IsImageFormat(string filePath)
    {
        try
        {
            using (var image = Image.FromFile(filePath))
            {
                if (image != null)
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
