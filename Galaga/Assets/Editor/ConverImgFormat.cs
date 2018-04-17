#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEditor;
using UnityEngine;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Graphics = System.Drawing.Graphics;

public class ConverImgFormat
{
    [MenuItem("Assset/IOSChangeFormat")]
    public static void IOSConvertAllImageToTrueCollor()
    {
        string[] paths = GetAllPrefabs(".png");
        foreach (var path in paths)
        {
            TextureImporter ti = (TextureImporter)TextureImporter.GetAtPath(path);
            ti.SetPlatformTextureSettings(new TextureImporterPlatformSettings()
            {
                overridden = true,
                format = TextureImporterFormat.RGBA32,
                name = "iPhone",
                compressionQuality = 0,
                textureCompression = TextureImporterCompression.Uncompressed,
                crunchedCompression = false
            });
            ti.SaveAndReimport();
        }
    }

    [MenuItem("Assset/IOSChangeFormatOfListSelected")]
    public static void IOSChangeFormatOfListSelected()
    {
        string[] paths = GetAllPathOfObjectsSelected();
        foreach (var path in paths)
        {
            TextureImporter ti = (TextureImporter)TextureImporter.GetAtPath(path);
            ti.SetPlatformTextureSettings(new TextureImporterPlatformSettings()
            {
                overridden = true,
                format = TextureImporterFormat.RGBA32,
                name = "iPhone",
                compressionQuality = 100,
                crunchedCompression = false
            });
            ti.SaveAndReimport();
        }
    }

    [MenuItem("Assset/AndroidChangeFormat")]
    public static void AndroidConvertAllImageToTrueCollor()
    {
        string[] paths = GetAllPrefabs(".png");
        foreach (var path in paths)
        {
            TextureImporter ti = (TextureImporter)TextureImporter.GetAtPath(path);
            ti.SetPlatformTextureSettings(new TextureImporterPlatformSettings()
            {
                overridden = true,
                format = TextureImporterFormat.DXT5,
                name = "Android",
                compressionQuality = 0,
                crunchedCompression = false
            });
            ti.SaveAndReimport();
        }
    }

    [MenuItem("Assset/AndroidChangeFormatOfListSelected")]
    public static void AndroidChangeFormatOfListSelected()
    {
        string[] paths = GetAllPathOfObjectsSelected();
        foreach (var path in paths)
        {
            TextureImporter ti = (TextureImporter)TextureImporter.GetAtPath(path);
            ti.SetPlatformTextureSettings(new TextureImporterPlatformSettings()
            {
                overridden = true,
                name = "Android",
                format = TextureImporterFormat.DXT5,
                compressionQuality = 0,
                crunchedCompression = false
            });
            ti.SaveAndReimport();
        }
    }


    [MenuItem("Assets/ResizeSelectedTexture")]
    public static void ResizeSelectedTexture()
    {
        var objs = Selection.objects;
        var paths = objs.Select(s => AssetDatabase.GetAssetPath(s)).ToList();
        foreach (var path in paths)
        {
            Resize(path);
        }
        AssetDatabase.Refresh();
    }

    public static void ResizeListSelected()
    {
        var paths = GetAllPathOfObjectsSelected();
        foreach (var path in paths)
            Resize(path);
        AssetDatabase.Refresh();
    }

    public static void Resize(string path)
    {
        path = Application.dataPath + path.Replace("Assets", "");
        var image = Image.FromFile(path);
        var newImage = ScaleImage(image);
        image.Dispose();
        newImage.Save(path, ImageFormat.Png);
    }

    public static Image ScaleImage(Image image)
    {
        var newWidth = (int)Mathf.Round(image.Width / 4f) * 4;
        var newHeight = (int)Mathf.Round(image.Height / 4f) * 4;

        var newImage = new Bitmap(newWidth, newHeight);

        using (var graphics = Graphics.FromImage(newImage))
        {
            graphics.CompositingMode = CompositingMode.SourceCopy; ;
            graphics.CompositingQuality = CompositingQuality.Default;
            graphics.DrawImage(image, 0, 0, newWidth, newHeight);
        }

        return newImage;
    }

    public static string[] GetAllPathOfObjectsSelected()
    {
        return Selection.objects.Select(s => AssetDatabase.GetAssetPath(s)).ToArray();
    }

    public static string[] GetAllPrefabs(string format)
    {
        string[] temp = AssetDatabase.GetAllAssetPaths();
        List<string> result = new List<string>();
        foreach (string s in temp)
            if (s.Contains(format)) result.Add(s);
        return result.ToArray();
    }
}
#endif