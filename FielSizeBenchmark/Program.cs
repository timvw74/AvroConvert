using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using AutoFixture;
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



            var data100 = BuildData(1);
            var data1k = BuildData(10);
            var data10k = BuildData(100);
            var data100k = BuildData(1000);
            var data1m = BuildData(10000);


            DoIT(data100);
            DoIT(data1k);
            DoIT(data10k);
            DoIT(data100k);
            DoIT(data1m);



            Console.ReadLine();

        }

        private static void DoIT(IEnumerable<User> data100)
        {
            Console.WriteLine("---------------------------------------------------------");

            var json100 = JsonConvert.SerializeObject(data100);
            var serializedBytes100 = Encoding.UTF8.GetBytes(json100);

            Console.WriteLine("Json " + serializedBytes100.Length);

            Console.WriteLine("Json Gzip " + GzipJson(serializedBytes100).Length);

            Console.WriteLine("Avro " + AvroConvert.Serialize(data100).Length);

            Console.WriteLine("Avro Brotli " + AvroConvert.Serialize(data100, CodecType.Brotli).Length);
        }


        internal static User[] BuildData(int N)
        {
            var fixture = new AutoFixture.Fixture();

            return fixture
                .Build<User>()
                .With(c => c.Offerings, fixture.CreateMany<Offering>(N).ToList())
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
    }
}
