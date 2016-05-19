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

        class MySampleListSerializer : ObjectSerializer
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

            /// <summary>
            /// load data to 
            /// </summary>
            /// <param name="objectId"></param>
            /// <param name="list"></param>
            public void Load(int objectId, List<int> list)
            {
                this.objectId = objectId;
                //goto begin
                this.binWriter.Seek(0, SeekOrigin.Begin);
                int j = list.Count;
                for (int i = 0; i < j; ++i)
                {
                    binWriter.Write(list[i]);
                }
                buffer = this.ms.ToArray();
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

        private void button2_Click(object sender, EventArgs e)
        {

        }

    }

}
