using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace QMS.Storage.CosmosDB
{
    public class JsonCosmosSerializer : CosmosSerializer
    {
        public override T FromStream<T>(Stream stream)
        {
            var result = JsonSerializer.DeserializeAsync<T>(stream).Result;
            stream.Close();
            return result;
        }

        public override Stream ToStream<T>(T input)
        {
            Stream output = new MemoryStream();
            JsonSerializer.SerializeAsync(output, input, typeof(T)).Wait();
            return output;
        }
    }
}
