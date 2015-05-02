using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;

namespace UploadDownloadAsyncWithProgress.Model
{
    class FileDetail : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string caller)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(caller));
            }
        }

        public int FileId { get; set; }
        public long FileSize {get;set;}
        public string FileUrl { get; set; }

        private string fileName;
        private float progressPercentage;
        private string progressDetails;
        private bool isFileSelected;
        private bool isUploaded;
        private bool isUploadingOrDownloading;
        private BitmapImage fileImage;
      
        
        public BitmapImage FileImage
        {
            get
            {
                return fileImage;
            }
            set
            {
                fileImage = value;
                NotifyPropertyChanged("FileImage");
            }
        }
        
        public string FileName
        {
            get
            {
                return fileName;
            }
            set
            {
                fileName = value;
                NotifyPropertyChanged("FileName");
            }
        }

        public float ProgressPercentage
        {
            get
            {
                return progressPercentage;
            }
            set
            {
                progressPercentage = value;
                NotifyPropertyChanged("ProgressPercentage");
            }
        }

        public string ProgressDetails
        {
            get
            {
                return progressDetails;
            }
            set
            {
                progressDetails = value;
                NotifyPropertyChanged("ProgressDetails");
            }
        }

        public bool IsFileSelected
        {
            get
            {
                return isFileSelected;
            }
            set
            {
                isFileSelected = value;
                NotifyPropertyChanged("IsFileSelected");
            }
        }

        public bool IsUploaded
        {
            get
            {
                return isUploaded;
            }
            set
            {
                isUploaded = value;
                NotifyPropertyChanged("IsUploaded");
            }
        }

        public bool IsUploadingOrDownloading
        {
            get
            {
                return isUploadingOrDownloading;
            }
            set
            {
                isUploadingOrDownloading = value;
                NotifyPropertyChanged("IsUploadingOrDownloading");
            }
        }

    }
}
