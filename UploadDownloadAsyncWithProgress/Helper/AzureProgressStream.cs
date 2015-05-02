using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UploadDownloadAsyncWithProgress.Helper
{
    // This class allows progress changed events to be raised from the blob upload/download.
    public class AzureProgressStream : Stream
    {
        #region Private Fields
        private Stream stream;
        private long bytesTransferred;
        private long totalLength;
        #endregion

        #region Public Handler
        public event EventHandler<ProgressChangedEventArgs> ProgressChanged;
        public int FileId { get; set; }
        #endregion

        #region Public Constructor
        public AzureProgressStream(Stream file)
        {
            this.stream = file;
            this.totalLength = file.Length;
            this.bytesTransferred = 0;
        }
        #endregion

        #region Public Properties
        public override bool CanRead
        {
            get
            {
                return this.stream.CanRead;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return this.stream.CanSeek;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return this.stream.CanWrite;
            }
        }

        public override void Flush()
        {
            this.stream.Flush();
        }

        public override long Length
        {
            get
            {
                return this.stream.Length;
            }
        }

        public override long Position
        {
            get
            {
                return this.stream.Position;
            }
            set
            {
                this.stream.Position = value;
            }
        }
        #endregion

        #region Public Methods
        public override int Read(byte[] buffer, int offset, int count)
        {
            int result = this.stream.Read(buffer, offset, count);
            double actualResult = (double)result / 2;
            bytesTransferred +=(long) Math.Ceiling(actualResult);
            if (ProgressChanged != null)
            {
                try
                {
                    OnProgressChanged(new ProgressChangedEventArgs(bytesTransferred, totalLength,FileId,TransferTypeEnum.Upload));
                }
                catch (Exception)
                {
                    ProgressChanged = null;
                }
            }
            return result;
        }

        protected virtual void OnProgressChanged(ProgressChangedEventArgs e)
        {
            if (ProgressChanged != null)
                ProgressChanged(this, e);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return this.stream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            totalLength = value;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            this.stream.Write(buffer, offset, count);
            bytesTransferred += count;
            {
                try
                {
                    OnProgressChanged(new ProgressChangedEventArgs(bytesTransferred, totalLength,FileId,TransferTypeEnum.Download));
                }
                catch (Exception)
                {
                    ProgressChanged = null;
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        #endregion
    }


    public class ProgressChangedEventArgs : EventArgs
    {
        #region Private Fields
        private long bytesRead;
        private long totalLength;
        private int fileId;
        private TransferTypeEnum transferType;
        #endregion

        #region Public Constructor
        public ProgressChangedEventArgs(long bytesRead, long totalLength, int fileId, TransferTypeEnum transferType)
        {
            this.bytesRead = bytesRead;
            this.totalLength = totalLength;
            this.fileId = fileId;
            this.transferType = transferType;
        }
        #endregion

        #region Public properties

        public long BytesRead
        {
            get
            {
                return this.bytesRead;
            }
            set
            {
                this.bytesRead = value;
            }
        }

        public long TotalLength
        {
            get
            {
                return this.totalLength;
            }
            set
            {
                this.totalLength = value;
            }
        }

        public int FileId
        {
            get
            {
                return this.fileId;
            }
            set
            {
                this.fileId = value;
            }
        }

        public TransferTypeEnum TransferType
        {
            get
            {
                return this.transferType;
            }
            set
            {
                this.transferType = value;
            }
        }

        #endregion
    }

    public enum TransferTypeEnum
    {
        Download,
        Upload
    }

}

