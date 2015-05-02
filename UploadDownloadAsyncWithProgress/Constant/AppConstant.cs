using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UploadDownloadAsyncWithProgress.Constant
{
    public static class AppConstant
    {
        public const string AccountName = "uploadanddownload";
        public const string AccountKey = "P2dEV/nSq0/1WV0BpWqyNZe6obmWRDMgqQ27WmcLxlqRX6AghcVAzEr7bPd3vplfSpPhBThDDPU3jAY2CySXLQ==";
        public const string ContainerName = "mycontainer";
       
        public const string DownloadProgressStatus = "Dowloading {0} MBs of {1} MBs";
        public const string UploadProgressStatus = "Uploading {0} MBs of {1} MBs";
        public const string TwoDecimalN2 = "n2";        
        public const string UploadCompleteMessage = "Uploaded successfully.";
        public const string DownloadCompleteMessage = "Downloaded to Pictures Library.";

        public const string ExtensionJpg = ".jpg";
        public const string ExtensionJpeg = ".jpeg";
        public const string ExtensionPng = ".png";

    }
}
