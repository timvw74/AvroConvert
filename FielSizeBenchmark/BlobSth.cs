using System;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using SolTechnology.Avro;

namespace GrandeBenchmark
{
    public static class BlobContainerClientExtensions
    {
        public static T ReadItemFromBlob<T>(this BlobContainerClient client, string blobName)
        {
            var blob = client.GetBlobClient(blobName);
            var content = blob.DownloadContent();

            var result = AvroConvert.Deserialize<T>(content.Value.Content.ToArray());
            return result;
        }

        public static void WriteItemToBlob(
            this BlobContainerClient client,
            string blobName,
            object content)
        {
            var blob = client.GetBlobClient(blobName);

            var serializedContent = AvroConvert.Serialize(content);

            blob.DeleteIfExists(DeleteSnapshotsOption.IncludeSnapshots);
            blob.Upload(new BinaryData(serializedContent));

        }
    }
}