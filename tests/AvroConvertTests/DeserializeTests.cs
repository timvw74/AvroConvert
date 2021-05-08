using System.Collections.Generic;
using SolTechnology.Avro;
using SolTechnology.Avro.Exceptions;
using Xunit;

namespace AvroConvertComponentTests
{
    public class DeserializeTests
    {
        private readonly byte[] _avroBytes = System.IO.File.ReadAllBytes("avro.avro");


        [Fact]
        public void Issue38()
        {

            string schema =
                "{ \"type\":\"record\", \"name\":\"DepartmentAVRO\", \"namespace\":\"models.mvp.Department\", \"fields\":[ {\"name\":\"struct_code\",\"type\":\"int\",\"doc\":\"Идентификатор подразделения\"}, {\"name\":\"firm_id\",\"type\":\"int\",\"doc\":\"Идентификатор организации\"}, { \"name\":\"struct_parent\",\"type\":[\"int\",\"null\"],\"doc\":\"Идентификатор родительского подразделения\"}, { \"name\":\"struct_name\",\"type\":\"string\",\"doc\":\"Наименование подразделения\"}, { \"name\":\"financial_short_name\",\"type\":\"string\",\"doc\":\"Код ЦФО\"}, { \"name\":\"start_date\",\"type\":[\"null\", { \"type\": \"long\", \"logicalType\":\"date\"}],\"doc\":\"Дата начала действия\"}, { \"name\":\"end_date\",\"type\":[\"null\", { \"type\": \"long\", \"logicalType\":\"date\"}],\"doc\":\"Дата окончания действия\"} ]}";

            //Act
            var result = AvroConvert.DeserializeHeadless<DepartmentAVRO>(_avroBytes, schema);
            //Assert
            var x = 1;
        }

        [Fact]
        public void Deserialize_CustomSchema_OnlyValuesFromCustomSchemaAreReturned()
        {
            //Arrange
            var expectedResult = new List<User>();
            expectedResult.Add(new User
            {
                name = "Alyssa",
                favorite_number = 256,
                favorite_color = null
            });

            expectedResult.Add(new User
            {
                name = "Ben",
                favorite_number = 7,
                favorite_color = "red"
            });

            //Act
            var result = AvroConvert.Deserialize<List<User>>(_avroBytes);


            //Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void Deserialize_NonGenericMethod_OnlyValuesFromCustomSchemaAreReturned()
        {
            //Arrange
            var expectedResult = new List<UserNameClass>();
            expectedResult.Add(new UserNameClass
            {
                name = "Alyssa"
            });

            expectedResult.Add(new UserNameClass
            {
                name = "Ben"
            });

            //Act
            var result = AvroConvert.Deserialize(_avroBytes, typeof(List<UserNameClass>));


            //Assert
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void Deserialize_InvalidFile_InvalidAvroObjectExceptionIsThrown()
        {
            //Arrange
            byte[] invalidBytes = new byte[2137];


            //Act
            var result = Record.Exception(() => AvroConvert.Deserialize<int>(invalidBytes));


            //Assert
            Assert.IsType<InvalidAvroObjectException>(result);
        }
    }
}
