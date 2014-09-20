using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LiteDB;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;

namespace UnitTest
{
    [TestClass]
    public class BsonTest
    {
        [TestInitialize]
        public void Init()
        {
        }

        [TestMethod]
        public void Bson_Document()
        {
            var d = new BsonDocument();
            d["Id"] = Guid.NewGuid();
            d["Name"] = "John Doe";
            d["Now"] = DateTime.Now;
            d["Num"] = 123;
            d["Money"] = 1.55m;
            d["List"] = new BsonArray();
            d["List"].AsArray.Add(123);
            d["List"].AsArray.Add("abc");

            var o = new BsonObject();
            o["Type"] = "Jogn";
            o["Go"] = 55.55;

            d["List"].AsArray.Add(o);

            var bytes = d.ToBson();

            var d2 = new BsonDocument(bytes);



            var name = (string)d.GetFieldValue("Name");
            var name2 = (string)d.GetFieldValue("Name2");

            Assert.AreEqual(name2, null);

            Assert.AreEqual(d["Name"].AsString, name);

        }

        [TestMethod]
        public void Bson_GetValueField()
        {
        }

        [TestMethod]
        public void Bson_Serialize()
        {
        }
    }
}
