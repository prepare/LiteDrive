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
            d["Name"] = "John Doe";
            d["Phones"] = new BsonArray();
            d["Phones"].AsArray.Add(123);
            d["Phones"].AsArray.Add(new BsonObject());
            d["Phones"][1]["Type"] = "Mobile";
            d["Phones"][1]["Number"] = "+55 51 9900-5555";

            var dt = d.To<Customer>();
            var json = d.ToJson();
            var bytes = d.ToBson();

            var name = (string)d.GetFieldValue("Name2");

            Assert.AreEqual(d["Name"].AsString, dt.Name);
            Assert.AreEqual(d["Name"].AsString, name);

            var d2 = new BsonDocument(dt);

            Assert.AreEqual(d2["Name"].AsString, dt.Name);
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
