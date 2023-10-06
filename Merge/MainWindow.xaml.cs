using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.Storage.Pickers;
using Windows.Storage;
using Microsoft.UI.Xaml.Controls;
using System.Diagnostics;
using PdfSharp.Pdf.IO;
using PdfSharp.Pdf;
using System.IO;
using System.Threading.Tasks;

namespace Merge
{
    public sealed partial class MainWindow : Window
    {
        IReadOnlyList<StorageFile> files;

        public MainWindow()
        {
            this.InitializeComponent();
            this.Title = "PDF Merge";

            // Encodings for creating pdf files
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var args = Environment.GetCommandLineArgs();
            foreach (var arg in args)
            {
                CheckBox checkBox = new()
                {
                    Content = arg,
                    IsChecked = true,
                    Margin = new Thickness(24, 0, 0, 0)
                };

                Files.Children.Add(checkBox);
            }
        }

        public async Task MergePDFsAsync(IReadOnlyList<StorageFile> files)
        {
            PdfDocument outputDocument = new PdfDocument();

            try
            {
                foreach (StorageFile file in files)
                {
                    using (Stream fileStream = await file.OpenStreamForReadAsync())
                    {
                        PdfDocument inputDocument = PdfReader.Open(fileStream, PdfDocumentOpenMode.Import);

                        int count = inputDocument.PageCount;
                        for (int idx = 0; idx < count; idx++)
                        {
                            PdfPage page = inputDocument.Pages[idx];
                            outputDocument.AddPage(page);
                        }
                    }
                }

                FileSavePicker savePicker = new FileSavePicker();

                var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
                WinRT.Interop.InitializeWithWindow.Initialize(savePicker, hWnd);

                savePicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                savePicker.FileTypeChoices.Add("PDF Files", new List<string> { ".pdf" });
                savePicker.DefaultFileExtension = ".pdf";
                savePicker.SuggestedFileName = "output.pdf";

                StorageFile outputFile = await savePicker.PickSaveFileAsync();

                if (outputFile != null)
                {
                    using (Stream outputStream = await outputFile.OpenStreamForWriteAsync())
                    {
                        outputDocument.Save(outputStream, false);
                    }

                    Debug.WriteLine("PDF saved successfully to: " + outputFile.Path);
                }
                else
                {
                    Debug.WriteLine("User canceled the save operation.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error merging PDFs: " + ex.Message);
            }
        }
        public void MergeButton_Click(object sender, RoutedEventArgs e)
        {
            MergePDFsAsync(files);
        }
        private async void PickFilesButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var openPicker = new FileOpenPicker();

                var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);

                WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);

                openPicker.ViewMode = PickerViewMode.List;
                openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
                openPicker.FileTypeFilter.Add(".pdf");

                files = await openPicker.PickMultipleFilesAsync();
                if (files.Count > 0)
                {
                    foreach (StorageFile file in files)
                    {
                        CheckBox checkBox = new()
                        {
                            Content = file.Name,
                            IsChecked = true,
                            Margin = new Thickness(24, 0, 0, 0)
                        };

                        Files.Children.Add(checkBox);
                    }
                }
                else
                {
                    Console.WriteLine("No files selected");
                }
            } catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

        }
    }
}

