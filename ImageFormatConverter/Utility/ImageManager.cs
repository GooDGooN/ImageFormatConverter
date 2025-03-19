using ImageFormatConverter.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;

namespace ImageFormatConverter.Utility;

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
    public static int GetImageDirectorys(Collection<string> targetCollection, params string[] names)
    {
        var count = 0;

        if (Directory.Exists(names[0]))
        {
            var files = new DirectoryInfo(names[0]).GetFiles();

            foreach (var info in files)
            {
                if (IsImageFormat(info.FullName))
                {
                    targetCollection.Add(info.FullName);
                    count++;
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
                    count++;
                }
            }
        }

        return count;
    }

    public static void ExportImages(Collection<string> files, string directory, TargetImageFormat format, bool createDirectory)
    {
        if (createDirectory)
        {
            var oldDir = directory;
            directory = Path.Combine(directory, "ConvertedImage");

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(Path.Combine(oldDir, "ConvertedImage"));
            }
        }

        foreach (var file in files)
        {
            var formatStr = format.ToString();
            var fileName = Path.Combine(directory, Path.GetFileName(file));
            fileName = Path.ChangeExtension(fileName, formatStr);

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
