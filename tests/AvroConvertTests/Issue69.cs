using System;
using System.Collections.Generic;
using System.IO;
using SolTechnology.Avro;
using Xunit;

namespace AvroConvertComponentTests
{
    public class Issue69
    {
        private readonly byte[] _avroBytes;
        private readonly string _example2schema;

        public Issue69()
        {
            _avroBytes = File.ReadAllBytes("bq-sample");

            // _example2schema = File.ReadAllText("AvroSchema.json");
        }

        [Fact]
        public void GetSchemaAsString_ValidBytes_SchemaIsReturned()
        {
            //Arrange

            var schema = AvroConvert.GetSchema(_avroBytes);

            var result = AvroConvert.Deserialize<List<Root>>(_avroBytes);

            var schema2 = AvroConvert.GenerateSchema(typeof(MyModel));
            
            //Act
            // var result = AvroConvert.DeserializeHeadless<MyModel>(_avroBytes, schema);
            
            //Assert
            Assert.Equal(_example2schema, "schema2");
        }

    }


    public class MyModel
    {
        public int Capacity { get; set; }
        public int Count { get; set; }
        public Root Root { get; set; }


    }

    public class Root
    {
        public string? tracker_id { get; set; }
        public double? latitude { get; set; }
        public double? longitude { get; set; }
        public string? datetime { get; set; }
        public double? altitude { get; set; }
        public string? accelerometer { get; set; }
        public double? speed { get; set; }
    }
}
