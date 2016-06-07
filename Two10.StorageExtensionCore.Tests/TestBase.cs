using Microsoft.WindowsAzure.Storage;
using System;

namespace Two10.StorageExtensionCore.Tests
{
    public abstract class TestBase
    {
        protected CloudStorageAccount Account
        {
            get
            {
                string connectionString = "UseDevelopmentStorage=true";
                if (null != Environment.GetEnvironmentVariable("DataConnectionString"))
                {
                    connectionString = Environment.GetEnvironmentVariable("DataConnectionString");
                }

                return CloudStorageAccount.Parse(connectionString);
            }
        }
    }
}
