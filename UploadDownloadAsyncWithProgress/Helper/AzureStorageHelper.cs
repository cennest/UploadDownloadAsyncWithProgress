using UploadDownloadAsyncWithProgress.Constant;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UploadDownloadAsyncWithProgress.Helper
{
    public static class AzureStorageHelper
    {
        public static async Task<CloudBlobContainer> GetBlobContainer()
        {
            // Retrieve storage account from connection string
            StorageCredentials storageCredentials = new StorageCredentials(AppConstant.AccountName,AppConstant.AccountKey);
            CloudStorageAccount storageAccount = new CloudStorageAccount(storageCredentials, useHttps: true);
            // Create the blob client 
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            // Retrieve a reference to a container 
            CloudBlobContainer container = blobClient.GetContainerReference(AppConstant.ContainerName);
            // Create the container if it doesn't already exist
            await container.CreateIfNotExistsAsync();
            // Set the permission 
            await container.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
            return container;
        }
    }
}
