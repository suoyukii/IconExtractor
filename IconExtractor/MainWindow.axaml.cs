using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Platform;
using Avalonia.Platform.Storage;
using Image = Avalonia.Controls.Image;
using Bitmap = Avalonia.Media.Imaging.Bitmap;
using PixelFormat = Avalonia.Platform.PixelFormat;

namespace IconExtractor;

public partial class MainWindow : Window
{
    private string[] last_file_list;

    public MainWindow()
    {
        InitializeComponent();
        DragDrop.SetAllowDrop(this, true);
        AddHandler(DragDrop.DropEvent, Drop);
    }

    private void Drop(object sender, DragEventArgs e)
    {
        var file_list = e.Data.GetFiles();
        var storage_items = file_list.ToList();
        var file_list2 = storage_items.Select(file => file.Path.LocalPath).ToList();
        last_file_list = file_list2.ToArray();
        GenerateImage();
    }

    private new void SizeChanged(object sender, SelectionChangedEventArgs e)
    {
        if (last_file_list == null) return;
        GenerateImage();
    }

    private void GenerateImage()
    {
        foreach (var file_url in last_file_list)
        {
            if (!File.Exists(file_url)) break;
            var size = int.Parse((size_select.SelectedItem as ComboBoxItem).Tag.ToString());
            tip.IsVisible = false;
            list.Children.Clear();
            var index = 0;
            while (true)
            {
                var icon = System.Drawing.Icon.ExtractIcon(file_url, index++, size);
                if (icon == null) break;
                var bmp = icon.ToBitmap();
                icon.Dispose();
                var bmpdata = bmp.LockBits
                (
                    new Rectangle(0, 0, bmp.Width, bmp.Height),
                    ImageLockMode.ReadWrite,
                    bmp.PixelFormat
                );
                var image = new Image
                {
                    Width = 96,
                    Height = 96,
                    Margin = new Thickness(8),
                    Source = new Bitmap(
                        PixelFormat.Bgra8888,
                        AlphaFormat.Unpremul,
                        bmpdata.Scan0,
                        new PixelSize(bmpdata.Width, bmpdata.Height),
                        new Vector(96, 96),
                        bmpdata.Stride
                    )
                };
                image.Tapped += async (_, _) =>
                {
                    var save_file_picker = await GetTopLevel(this).StorageProvider.SaveFilePickerAsync(
                        new FilePickerSaveOptions
                        {
                            Title = "Save Icon File",
                            SuggestedFileName = "icon",
                            DefaultExtension = "png",
                            FileTypeChoices =
                            [
                                new FilePickerFileType("PNG") { Patterns = ["*.png"] },
                                new FilePickerFileType("Icon") { Patterns = ["*.ico"] }
                            ]
                        });

                    if (save_file_picker is null) return;
                    ((Bitmap)image.Source).Save(save_file_picker.Path.LocalPath);
                };
                list.Children.Add(image);
            }
        }
    }
}