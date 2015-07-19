using System;
using System.Collections.Generic;
using System.ComponentModel;

using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

using LiteDB;
namespace LiteDriveTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string dbFilename = "d:\\WImageTest\\litedisk1.disk";
            if (System.IO.File.Exists(dbFilename))
            {
                System.IO.File.Delete(dbFilename);
            }
            //-----------------------------------------------
            using (LiteEngine engine = new LiteEngine(dbFilename))
            {

                var listCollection = engine.GetCollection("list1");
                engine.BeginTrans();

                MySimpleListSerializer simpleListCarrier = new MySimpleListSerializer();
                for (int i = 0; i < 20; ++i)
                {
                    simpleListCarrier.LoadDocument(i, new List<int> { 1, 2, 3 });
                    listCollection.Insert(simpleListCarrier);
                }

                engine.Commit();
            }
            //-----------------------------------------------------------------
            //read data back from file
            using (LiteEngine engine = new LiteEngine(dbFilename))
            {
                var listCollection = engine.GetCollection("list1");
                var listItem = listCollection.FindById(1);
                //serialrize back ...


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

        class MySimpleListSerializer : ObjectSerializer
        {

            MemoryStream ms;
            BinaryWriter binWriter;
            byte[] buffer;
            int objectId;
            public MySimpleListSerializer()
            {
                this.ms = new MemoryStream();
                this.binWriter = new BinaryWriter(ms);
            }
            //test only
            public void LoadDocument(int objectId, List<int> list)
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
            public override byte[] Content
            {
                get { return buffer; }
            }
            public override object Id
            {
                get { return this.objectId; }
            }
            public override object GetFieldValue(string fieldName)
            {
                return null;
            }
        }

    }

}
