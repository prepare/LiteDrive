using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using LiteDB;

namespace Test01
{
    public partial class Form1 : Form
    {
        string dbFilename = "d:\\WImageTest\\testdb01.ldb";

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (System.IO.File.Exists(dbFilename))
            {
                System.IO.File.Delete(dbFilename);
            }

            //----------------------------------------------
            //1. create db and insert sample data
            using (var db = new LiteEngine(dbFilename))
            {
                var c = db.GetCollection("customer");
                db.BeginTrans();
                for (var i = 1; i <= 500; i++)
                {
                    var d = new BsonDocument();
                    d.Id = i;
                    d["Name"] = "San Jose:" + i;
                    c.Insert(d);
                    //c.Insert(i, d);
                }
                //for (var i = 1; i <= 500; i++)
                //{
                //    c.Delete(i);
                //}
                db.Commit();
            }

            //----------------------------------------------
            //2. read data ...
            using (var db = new LiteEngine(dbFilename))
            {
                var customerCollection = db.GetCollection("customer");
                var cc = customerCollection.FindById(1);


            }
        }


    }
}
