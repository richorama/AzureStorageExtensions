using Microsoft.WindowsAzure.Storage;

namespace Two10.AzureJsonStore
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Microsoft.WindowsAzure.Storage.Blob;
    using Newtonsoft.Json;

    public class JsonStore<T>
    {
        private readonly CloudBlobContainer container;

        public JsonStore(CloudBlobContainer container)
        {
            this.container = container;
        }

        public T Get(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentNullException("key");

            try
            {

                var blob = this.container.GetBlockBlobReference(key);
                using (var stream = new MemoryStream())
                {
                    blob.DownloadToStream(stream);

                    if (stream.Length == 0)
                    {
                        return default(T);
                    }
                    stream.Position = 0;
                    using (var reader = new StreamReader(stream))
                    {
                        return JsonConvert.DeserializeObject<T>(reader.ReadToEnd());
                    }
                }
            }
            catch (StorageException ex)
            {
                if (ex.RequestInformation.HttpStatusCode == 404)
                {
                    return default(T);
                }
                throw;
            }
        }

        public void Put(string key, T value)
        {
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentNullException("key");
            if (null == value) throw new ArgumentNullException("value");

            var blob = this.container.GetBlockBlobReference(key);
            using (var stream = new MemoryStream())
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.Write(JsonConvert.SerializeObject(value));
                    writer.Flush();
                    stream.Position = 0;
                    blob.UploadFromStream(stream);
                }
            }
        }

        public void Delete(string key)
        {
            if (string.IsNullOrWhiteSpace(key)) throw new ArgumentNullException("key");

            var blob = container.GetBlockBlobReference(key);
            blob.DeleteIfExists();
        }

        public IEnumerable<string> Query()
        {
            return container.ListBlobs().Select(x => x.Uri.Segments.Last());
        }

        public void CreateIfNotExists()
        {
            container.CreateIfNotExists();
        }

        public void Delete()
        {
            container.Delete();
        }

        public void Create()
        {
            container.Create();
        }

        public void DeleteIfExists()
        {
            container.DeleteIfExists();
        }

    }
}
