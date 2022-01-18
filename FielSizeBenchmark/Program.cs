using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using AutoFixture;
using Azure.Storage.Blobs;
using BrotliSharpLib;
using GrandeBenchmark;
using Newtonsoft.Json;
using SolTechnology.Avro;

namespace FielSizeBenchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var x = AvroConvert.GenerateSchema(typeof(User));


            try
            {
                BlobContainerClient blobContainerClient = new BlobContainerClient("UseDevelopmentStorage=true", "sample-container");
                blobContainerClient.CreateIfNotExists();

                var data100 = BuildData(100);

                blobContainerClient.WriteItemToBlob("dupa", data100);

                var result = blobContainerClient.ReadItemFromBlob<List<User>>("dupa");

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            


            // var data1 = BuildData(1);
            // var data10 = BuildData(10);
            // var data100 = BuildData(100);
            // var data1k = BuildData(1000);
            // var data10k = BuildData(10000);
            // var data100k = BuildData(100000);
            // var data1m = BuildData(1000000);
            //
            //
            // DoIT(data1, nameof(data1));
            // DoIT(data10, nameof(data10));
            // DoIT(data100, nameof(data100));
            // DoIT(data1k, nameof(data1k));
            // DoIT(data10k, nameof(data10k));
            // DoIT(data100k, nameof(data100k));
            // DoIT(data1m, nameof(data1m));



            Console.ReadLine();

        }

        private static void DoIT(IEnumerable<User> data100, string name)
        {
            Console.WriteLine($"------------------------{name}---------------------------------");

            var json100 = JsonConvert.SerializeObject(data100);
            var serializedBytes100 = Encoding.UTF8.GetBytes(json100);

            Console.WriteLine("Json " + serializedBytes100.Length);

            Console.WriteLine("Json Gzip " + GzipJson(serializedBytes100).Length);

            Console.WriteLine("Json Brotli " + BrotliJson(serializedBytes100).Length);

            Console.WriteLine("Avro " + AvroConvert.Serialize(data100).Length);

            Console.WriteLine("Avro Gzip " + AvroConvert.Serialize(data100, CodecType.GZip).Length);

            Console.WriteLine("Avro Brotli " + AvroConvert.Serialize(data100, CodecType.Brotli).Length);
        }


        internal static User[] BuildData(int N)
        {
            var fixture = new AutoFixture.Fixture();

            return fixture
                .Build<User>()
                .CreateMany(N)
                .ToArray();
        }

        internal static byte[] GzipJson(byte[] uncompressedData)
        {
            using (var compressedStream = new MemoryStream())
            using (var zipStream = new GZipStream(compressedStream, CompressionMode.Compress))
            {
                zipStream.Write(uncompressedData, 0, uncompressedData.Length);
                zipStream.Close();
                return compressedStream.ToArray();
            }
        }

        internal static byte[] BrotliJson(byte[] uncompressedData)
        {
            return Brotli.CompressBuffer(uncompressedData, 0, uncompressedData.Length, 4);
        }
    }
}
