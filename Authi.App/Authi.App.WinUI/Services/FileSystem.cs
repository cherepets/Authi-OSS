using Authi.App.Logic.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace Authi.App.WinUI.Services
{
    internal class FileSystem : IFileSystem
    {
        public string AppDataDirectory => ApplicationData.Current.LocalFolder.Path;

        public async Task<Stream?> ReadFromPickerAsync()
        {
            var openPicker = new FileOpenPicker();
            var window = App.Current.MainWindow;
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
            WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.FileTypeFilter.Add("*");
            var file = await openPicker.PickSingleFileAsync();

            if (file == null)
            {
                return null;
            }
            var ras = await file.OpenReadAsync();
            return ras.AsStream();
        }

        public async Task<bool> WriteToPickerAsync(Stream stream, string? suggestedFileName = null)
        {
            var savePicker = new FileSavePicker();
            var window = App.Current.MainWindow;
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
            WinRT.Interop.InitializeWithWindow.Initialize(savePicker, hWnd);
            savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            if (suggestedFileName != null)
            {
                var fileType = $".{suggestedFileName.Split('.').Last()}";
                savePicker.FileTypeChoices.Add(fileType, new List<string>() { fileType });
                savePicker.SuggestedFileName = suggestedFileName;
            }
            var file = await savePicker.PickSaveFileAsync();

            if (file == null)
            {
                return false;
            }
            using var fileStream = await file.OpenStreamForWriteAsync();
            await stream.CopyToAsync(fileStream);
            return true;
        }
    }
}
