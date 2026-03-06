using Google.Apis.Storage.v1.Data;
using Google.Cloud.Storage.V1;
using System.Security.AccessControl;
using System.Text;

namespace WebApplication2.Repositories
{
    public class BucketsRepository
    {
        public async Task<string> Upload(Stream file, string bucketName, string filename)
        {
            var storage = StorageClient.Create();
            string uniquefilename = Guid.NewGuid() + System.IO.Path.GetExtension(filename);

            await storage.UploadObjectAsync(bucketName, uniquefilename, 
                "application/octet-stream", file);

            string publicUrl = $"https://storage.googleapis.com/{bucketName}/{uniquefilename}";
            return publicUrl;
        }

        public async Task<string> AssignPermission(string bucketName,string organiser, 
             string filename, string role="READER")
        {
            var storage = StorageClient.Create();
            var storageObject = await storage.GetObjectAsync(bucketName, filename, new GetObjectOptions
            {
                Projection = Projection.Full
            });

            storageObject.Acl.Add(new ObjectAccessControl
            {
                Bucket = bucketName,
                Entity = $"user-{organiser}",
                Role = role,
            });
            var updatedObject = await storage.UpdateObjectAsync(storageObject);

            //note: this is the authenticated URL, which is different than the public url
            //note: this will check whether the user accessing the file has the permission to do so
            return $"https://storage.cloud.google.com/{bucketName}/{filename}";


        }
    }
}
