//MIT 2015, WinterDev
using System;
using System.Collections.Generic;
using System.ComponentModel;

using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

using LiteDB;
namespace LiteStoreTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //sample1 
            string dbFilename = "..\\..\\output\\litedisk1.disk";
            if (System.IO.File.Exists(dbFilename))
            {
                System.IO.File.Delete(dbFilename);
            }
            //-----------------------------------------------
            //1. create engine 
            using (LiteEngine engine = new LiteEngine(dbFilename))
            {
                var listCollection = engine.GetCollection("list1");
                engine.BeginTrans();
                var sx1 = new MySampleListSerializer();
                for (int i = 0; i < 20; ++i)
                {
                    sx1.Load(i, new List<int> { 1, 2, 3 });
                    listCollection.Insert(sx1);
                }

                engine.Commit();
            }
            //-----------------------------------------------------------------
            //read data back from file
            using (LiteEngine engine = new LiteEngine(dbFilename))
            {
                var listCollection = engine.GetCollection("list1");
                var blob = listCollection.FindById(1);
                //serialrize back ... to 
                var sx1 = new MySampleListSerializer();
                var list = sx1.ConvertFromBlob(blob);

                //no object id 30 here
                var listItem2 = listCollection.FindById(30);
            }
            //-----------------------------------------------------------------
            //test delete data
            using (LiteEngine engine = new LiteEngine(dbFilename))
            {
                var listCollection = engine.GetCollection("list1");
                engine.BeginTrans();
                listCollection.Delete(0);
                listCollection.Delete(1);
                engine.Commit();

                //object with id 0 and 1 must not found

                if (listCollection.FindById(0) != null ||
                    listCollection.FindById(1) != null)
                {
                    throw new Exception();
                }
            }
            //-----------------------------------------------------------------
        }



        class MySampleListSerializer : ObjectSerializer, IDisposable
        {

            MemoryStream ms;
            BinaryWriter binWriter;
            byte[] buffer;
            int objectId;
            public MySampleListSerializer()
            {
                this.ms = new MemoryStream();
                this.binWriter = new BinaryWriter(ms);
            }
            public void Dispose()
            {
                if (binWriter != null)
                {
                    binWriter.Close();
                    binWriter = null;
                }
                if (ms != null)
                {
                    ms.Close();
                    ms.Dispose();
                    ms = null;
                }
            }
            public void Load(int objectId, List<int> list)
            {

                this.objectId = objectId;
                //goto begin again
                this.binWriter.Seek(0, SeekOrigin.Begin);
                int j = list.Count;
                int len = 0;
                for (int i = 0; i < j; ++i)
                {
                    binWriter.Write(list[i]);
                    len = (int)binWriter.BaseStream.Position;
                }

                binWriter.Flush();
                //make it array
                buffer = new byte[len];
                ms.Position = 0;
                ms.Read(buffer, 0, len);
            }
            public override byte[] GetBlob()
            {
                return buffer;
            }
            public override object Id
            {
                get { return this.objectId; }
            }
            public override object GetFieldValue(string fieldName)
            {
                return null;
            }


            public List<int> ConvertFromBlob(byte[] blob)
            {
                int blobLen = blob.Length;
                using (var ms1 = new MemoryStream(blob))
                using (var reader = new BinaryReader(ms1))
                {
                    List<int> result = new List<int>();
                    //just read 
                    while (ms1.Position < blobLen)
                    {
                        result.Add(reader.ReadInt32());
                    }

                    reader.Close();
                    ms1.Close();
                    return result;
                }


            }
        }


        class Customer
        {
            public int Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //sample1 
            string dbFilename = "..\\..\\output\\litedisk2.disk";
            if (System.IO.File.Exists(dbFilename))
            {
                System.IO.File.Delete(dbFilename);
            }
            //-----------------------------------------------
            //1. create engine 

            List<Customer> customerList = new List<Customer>()
            {
                new Customer{Id=1,FirstName="A1",LastName="B1"},
                new Customer{Id=2,FirstName="A2",LastName="B2"},
                new Customer{Id=3,FirstName="A3",LastName="B3"},
                new Customer{Id=4,FirstName="A4",LastName="B4"},
            }; 
            //-----------------------------------------------
            //***manual*** build a serializer
            var sx1 = MySimpleObjectSx<Customer>.Build(
                x => x.Id,
                w =>
                {
                    w.Set("_id", o => o.Id, (o, v) => o.Id = v);
                    w.Set("firstname", o => o.FirstName, (o, v) => o.FirstName = v);
                    w.Set("lastname", o => o.LastName, (o, v) => o.LastName = v);
                }); 
            //-----------------------------------------------


            using (LiteEngine engine = new LiteEngine(dbFilename))
            {
                var listCollection = engine.GetCollection("list1");
                engine.BeginTrans();
                for (int i = 0; i < customerList.Count; ++i)
                {
                    sx1.Load(i, customerList[i]);
                    listCollection.Insert(sx1);
                }
                engine.Commit();
            }
            //-----------------------------------------------------------------
            //read data back from file
            using (LiteEngine engine = new LiteEngine(dbFilename))
            {
                var listCollection = engine.GetCollection("list1");
                var blob = listCollection.FindById(1);
                //serialrize back ... to  
                var list = sx1.ConvertFromBlob<Customer>(blob);

                //no object id 30 here
                var listItem2 = listCollection.FindById(30);
            }
            //-----------------------------------------------------------------
            //test delete data
            using (LiteEngine engine = new LiteEngine(dbFilename))
            {
                var listCollection = engine.GetCollection("list1");
                engine.BeginTrans();
                listCollection.Delete(0);
                listCollection.Delete(1);
                engine.Commit();

                //object with id 0 and 1 must not found

                if (listCollection.FindById(0) != null ||
                    listCollection.FindById(1) != null)
                {
                    throw new Exception();
                }
            }
        }



    }

}
