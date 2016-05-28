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

            List<Customer> customerList = new List<Customer>()
            {
                new Customer{Id=1,FirstName="A1",LastName="B1"},
                new Customer{Id=2,FirstName="A2",LastName="B2"},
                new Customer{Id=3,FirstName="A3",LastName="B3"},
                new Customer{Id=4,FirstName="A4",LastName="B4"},
            };
            //-----------------------------------------------
            //***manual*** build a serializer
            var sx1 = ManualObjSx<Customer>.Build(
                x => x.Id,
                p =>
                {
                    p.RW("_id", (o, v) => o.Id = v, o => o.Id);
                    p.RW("firstname", (o, v) => o.FirstName = v, o => o.FirstName);
                    p.RW("lastname", (o, v) => o.LastName = v, o => o.LastName);
                });
            //--------------------------------------------------

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
                var data1 = sx1.New(blob);

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

        private void button3_Click(object sender, EventArgs e)
        {
            //sample1 
            string dbFilename = "..\\..\\output\\litedisk3.disk";
            if (System.IO.File.Exists(dbFilename))
            {
                System.IO.File.Delete(dbFilename);
            }

            //----------------------------------------------- 
            var customerList = new[]{
                new {id=0,firstname="X1",lastname="X2"},
                new {id=1,firstname="X2",lastname="Y2"}
            };
            var sx1 = ManualObjSx.Build(customerList, //need sample for type inference
            x => x.id,
            p =>
            {
                p.W("_id", o => o.id);
                p.W("firstname", o => o.firstname);
                p.W("lastname", o => o.lastname);
            });
            //-------------------------------------------------- 
            var sx2 = ManualObjSx<Customer>.Build(
                x => x.Id,
                p =>
                {
                    p.R("_id", (o, v) => o.Id = v);
                    p.R("firstname", (o, v) => o.FirstName = v);
                    p.R("lastname", (o, v) => o.LastName = v);
                });
            //--------------------------------------------------
            using (LiteEngine engine = new LiteEngine(dbFilename))
            {
                var listCollection = engine.GetCollection("list1");
                engine.BeginTrans();
                for (int i = 0; i < customerList.Length; ++i)
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

                var data1 = sx2.New(blob);

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

        class SuperCustomer
        {
            public int Id;
            public Customer Friend1 { get; set; }
            public Customer Friend2 { get; set; }
            public List<Customer> Others { get; set; }
        }

        private void button4_Click(object sender, EventArgs e)
        {


            //----------------------------------------------- 
            //complex object
            var super1 = new SuperCustomer();
            super1.Friend1 = new Customer() { Id = 0, FirstName = "F1" };
            super1.Friend2 = new Customer() { Id = 1, FirstName = "F2" };
            super1.Others = new List<Customer>{
                new Customer() { Id = 2, FirstName = "F3" },
                new Customer() { Id = 3, FirstName = "F4" }
            };
            //----------------------------------------------- 

            //sample1 
            string dbFilename = "..\\..\\output\\litedisk4.disk";
            if (System.IO.File.Exists(dbFilename))
            {
                System.IO.File.Delete(dbFilename);
            }

            //----------------------------------------------- 
            //create serializer plan

            //***manual*** build a serializer
            var sx1 = ManualObjSx<Customer>.Build(
                x => x.Id,
                p =>
                {
                    p.RW("_id", (o, v) => o.Id = v, o => o.Id);
                    p.RW("firstname", (o, v) => o.FirstName = v, o => o.FirstName);
                    p.RW("lastname", (o, v) => o.LastName = v, o => o.LastName);
                });
            //---------------------------------------------------------------------
            var sx2 = ManualObjSx<SuperCustomer>.Build(//need sample for type inference
            x => x.Id,
            p =>
            {
                p.RW("_id", (o, v) => o.Id = v, o => o.Id);
                p.RW("friend1", (o, v) => o.Friend1 = sx1.New(v), o => sx1.ToBlob(o.Friend1));
                p.RW("friend2", (o, v) => o.Friend2 = sx1.New(v), o => sx1.ToBlob(o.Friend2));
                p.RW("others", (o, v) => o.Others = sx1.NewList(v), o => sx1.ToBlob(o.Others));
            });
            //---------------------------------------------------------------------

            //write
            using (LiteEngine engine = new LiteEngine(dbFilename))
            {
                var listCollection = engine.GetCollection("list1");
                engine.BeginTrans();
                sx2.Load(0, super1);
                listCollection.Insert(sx2);
                engine.Commit();
            }

            //read
            //read data back from file
            using (LiteEngine engine = new LiteEngine(dbFilename))
            {
                var listCollection = engine.GetCollection("list1");
                var blob = listCollection.FindById(0);
                //serialrize back ... to   
                var data1 = sx2.New(blob); 
                //no object id 30 here
                var listItem2 = listCollection.FindById(30);
            }

        }

    }

}
