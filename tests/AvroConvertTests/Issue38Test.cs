using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SolTechnology.Avro;
using SolTechnology.Avro.Exceptions;
using Xunit;

namespace AvroConvertComponentTests
{
    public class Issue38Tests
    {
        private readonly byte[] _avroBytes = System.IO.File.ReadAllBytes("avro.avro");


        [Fact]
        public void Issue38()
        {

            string schema =
                "{ \"type\":\"record\", \"name\":\"DepartmentAVRO\", \"namespace\":\"models.mvp.Department\", \"fields\":[ {\"name\":\"struct_code\",\"type\":\"int\",\"doc\":\"Идентификатор подразделения\"}, {\"name\":\"firm_id\",\"type\":\"int\",\"doc\":\"Идентификатор организации\"}, { \"name\":\"struct_parent\",\"type\":[\"int\",\"null\"],\"doc\":\"Идентификатор родительского подразделения\"}, { \"name\":\"struct_name\",\"type\":\"string\",\"doc\":\"Наименование подразделения\"}, { \"name\":\"financial_short_name\",\"type\":\"string\",\"doc\":\"Код ЦФО\"}, { \"name\":\"start_date\",\"type\":[\"null\", { \"type\": \"long\", \"logicalType\":\"date\"}],\"doc\":\"Дата начала действия\"}, { \"name\":\"end_date\",\"type\":[\"null\", { \"type\": \"long\", \"logicalType\":\"date\"}],\"doc\":\"Дата окончания действия\"} ]}";

            //Act
            var bytesAfterChange = _avroBytes.Skip(5);

            var result = AvroConvert.DeserializeHeadless<DepartmentAVRO>(bytesAfterChange.ToArray(), schema);


            //Assert

        }
    }
}