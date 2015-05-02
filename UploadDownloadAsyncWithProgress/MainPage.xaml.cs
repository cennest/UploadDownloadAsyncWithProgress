using UploadDownloadAsyncWithProgress.Constant;
using UploadDownloadAsyncWithProgress.Helper;
using UploadDownloadAsyncWithProgress.Model;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace UploadDownloadAsyncWithProgress
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        static ObservableCollection<FileDetail> fileDetailList;
        public MainPage()
        {
            this.InitializeComponent();
            fileDetailList = new ObservableCollection<FileDetail> { new FileDetail() };
            this.FileList.DataContext = fileDetailList;
            this.FileList.ItemsSource = fileDetailList;
        }

        private async void PickAFileButton_Click(object sender, RoutedEventArgs e)
        {
            BitmapImage image = new BitmapImage();
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(AppConstant.ExtensionJpg);
            openPicker.FileTypeFilter.Add(AppConstant.ExtensionJpeg);
            openPicker.FileTypeFilter.Add(AppConstant.ExtensionPng);
            StorageFile file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                StorageApplicationPermissions.FutureAccessList.Add(file);
                image.SetSource(await file.OpenReadAsync());
                Button pickFileButton = (Button)sender;
                Grid parent = (Grid)VisualTreeHelper.GetParent(pickFileButton);
                FileDetail fileDetail = (FileDetail)parent.Tag;
                fileDetail.FileImage = image;
                fileDetail.FileName = file.Name;
                fileDetail.FileUrl = file.Path;
                fileDetail.FileId = fileDetailList.Count;
                var property = await file.GetBasicPropertiesAsync();
                fileDetail.FileSize = (long)property.Size;

                if (!fileDetail.IsFileSelected)
                {
                    fileDetail.IsFileSelected = true;
                    fileDetailList.Add(new FileDetail());
                }
            }
        }

        private async void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            Button pickFileButton = (Button)sender;
            Grid parent = (Grid)VisualTreeHelper.GetParent(pickFileButton);
            FileDetail fileDetail = (FileDetail)parent.Tag;
            fileDetail.IsUploadingOrDownloading = true;
            CloudBlobContainer container = await AzureStorageHelper.GetBlobContainer();
            StorageFile file = await StorageFile.GetFileFromPathAsync(fileDetail.FileUrl);

            Task.Run(() => UploadBlob(container, file, fileDetail.FileName, fileDetail.FileId, fileDetail.FileSize));
        }

        private async void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            Button pickFileButton = (Button)sender;
            Grid parent = (Grid)VisualTreeHelper.GetParent(pickFileButton);
            FileDetail fileDetail = (FileDetail)parent.Tag;
            fileDetail.IsUploadingOrDownloading = true;

            CloudBlobContainer container = await AzureStorageHelper.GetBlobContainer();
            Task.Run(() => DownloadBlob(container, fileDetail.FileName, fileDetail.FileId, fileDetail.FileSize));
        }

        private async void UploadBlob(CloudBlobContainer container, StorageFile file, string fileName, int fileId, long fileSize)
        {
            using (var fileStream = await file.OpenStreamForReadAsync())
            {
                // Retrieve reference to a blob 
                CloudBlockBlob blob = container.GetBlockBlobReference(fileName);
                AzureProgressStream progressStream = new AzureProgressStream(fileStream);
                progressStream.ProgressChanged += pstream_ProgressChanged;

                progressStream.SetLength(fileSize);
                progressStream.FileId = fileId;
                IInputStream inputStream = WindowsRuntimeStreamExtensions.AsInputStream(progressStream);
                await blob.UploadFromStreamAsync(inputStream);
            }
        }

        private async void DownloadBlob(CloudBlobContainer container, string fileName, int fileId, long fileSize)
        {
            StorageFolder pictureFolder = KnownFolders.PicturesLibrary;
            StorageFile file = await pictureFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);

            using (var fileStream = await file.OpenStreamForWriteAsync())
            {
                // Retrieve reference to a blob 
                CloudBlockBlob blob = container.GetBlockBlobReference(fileName);
                Stream stream = new MemoryStream();
                AzureProgressStream progressStream = new AzureProgressStream(stream);
                progressStream.ProgressChanged += pstream_ProgressChanged;
                progressStream.SetLength(fileSize);
                progressStream.FileId = fileId;
                IOutputStream outputStream = WindowsRuntimeStreamExtensions.AsOutputStream(progressStream);
                await blob.DownloadToStreamAsync(outputStream);
                stream.Position = 0;
                stream.CopyTo(fileStream);
                fileStream.Flush();
            }
        }


        private void pstream_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            int progress = (int)((double)e.BytesRead / e.TotalLength * 100);
            var dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
            dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    foreach (FileDetail fileDetail in fileDetailList)
                    {
                        if (fileDetail.FileId == e.FileId)
                        {
                            string bytesRead = ((e.BytesRead / 1024f) / 1024f).ToString(AppConstant.TwoDecimalN2);
                            string totalSize = ((e.TotalLength / 1024f) / 1024f).ToString(AppConstant.TwoDecimalN2);

                            if (e.TransferType == TransferTypeEnum.Upload)
                            {
                                fileDetail.ProgressDetails = String.Format(AppConstant.UploadProgressStatus, bytesRead, totalSize);
                            }
                            else
                            {
                                fileDetail.ProgressDetails = String.Format(AppConstant.DownloadProgressStatus, bytesRead, totalSize);
                            }

                            fileDetail.ProgressPercentage = progress;

                            if (progress == 100)
                            {
                                fileDetail.ProgressPercentage = 0;
                                fileDetail.ProgressDetails = "";
                                fileDetail.IsUploadingOrDownloading = false;

                                if (e.TransferType == TransferTypeEnum.Upload)
                                {
                                    fileDetail.IsUploaded = true;
                                    fileDetail.IsFileSelected = false;
                                    fileDetail.ProgressDetails = AppConstant.UploadCompleteMessage;
                                }
                                else
                                {
                                    fileDetail.ProgressDetails = AppConstant.DownloadCompleteMessage;
                                }
                            }

                            break;
                        }
                    }
                });
        }
    }
}