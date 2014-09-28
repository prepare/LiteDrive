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
    public class DocumentTest
    {
        [TestInitialize]
        public void Init()
        {
        }

        [TestMethod]
        public void Document_Create()
        {
            var d = new BsonDocument();
            d["Id"] = Guid.NewGuid();
            d["Name"] = "John Doe";
            d["Now"] = DateTime.Now;
            d["Num"] = 123;
            d["Money"] = 1.55m;
            //d["My.User.Name"] = "Carlos";
            d["List"] = new BsonArray();
            d["List"][5] = 123;
            //d["List"][1]["Name"] = "John";
            //d["List"][5] = DateTime.Now;

            Assert.AreEqual(1.55m, d["Money"].AsDecimal);
            Assert.AreEqual("Carlos", d["My"]["User"]["Name"]);

            //var bytes = d.ToBson();

            //var d2 = new BsonSerializer().ToDocument(bytes);



            //var name = (string)d.GetFieldValue("Name");
            //var name2 = (string)d.GetFieldValue("Name2");

            //Assert.AreEqual(name2, null);

            //Assert.AreEqual(d["Name"].AsString, name);

        }
    }
}
