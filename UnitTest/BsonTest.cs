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

            var dict = new Dictionary<string, string>();
            dict["first"] = "value";

            d["Dict"] = new BsonObject(dict);

            d["Phones"] = new BsonArray();
            d["Phones"].AsArray.Add(123);

            d["Phones"].AsArray.Add(new BsonObject(new { Type = "Mobile", Number = "+55 51 123" }));

            var dt = d.To<Customer>();
            var json = d.ToJson();
            var bytes = d.ToBson();

            var d2 = new BsonDocument(bytes);

            Assert.AreEqual("value", d2["Dict"].As<Dictionary<string, string>>()["first"]);


            var name = (string)d.GetFieldValue("Name");
            var name2 = (string)d.GetFieldValue("Name2");

            Assert.AreEqual(name2, null);

            Assert.AreEqual(d["Name"].AsString, dt.Name);
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
