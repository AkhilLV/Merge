using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.Storage.Pickers;
using Windows.Storage;

namespace Merge
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            this.Title = "PDF Merge";
        }

        private async void PickFilesButton_Click(object sender, RoutedEventArgs e)
        {
            // Clear previous returned file name, if it exists, between iterations of this scenario
            PickFilesOutputTextBlock.Text = "";

            var openPicker = new FileOpenPicker();

            // Retrieve the window handle (HWND) of the current WinUI 3 window.
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);

            // Initialize the file picker with the window handle (HWND).
            WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);

            // Set options for your file picker
            openPicker.ViewMode = PickerViewMode.List;
            openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            openPicker.FileTypeFilter.Add("*");

            // Open the picker for the user to pick a file
            IReadOnlyList<StorageFile> files = await openPicker.PickMultipleFilesAsync();
            if (files.Count > 0)
            {
                StringBuilder output = new StringBuilder("Picked files:\n");
                foreach (StorageFile file in files)
                {
                    output.Append(file.Name + "\n");
                }
                PickFilesOutputTextBlock.Text = output.ToString();
            }
            else
            {
                PickFilesOutputTextBlock.Text = "Operation cancelled.";
            }
        }
    }
}
