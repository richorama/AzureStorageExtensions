using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Two10.StorageExtension.Tests
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
