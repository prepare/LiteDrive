using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using SQLite;
namespace MySQLiteTest
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ////create database ***
            //string dbfile = "d:\\WImageTest\\test_sqlite1.sqlite";
            //using (var db = new SQLiteConnection(dbfile, true))
            //{
            //    db.CreateTable<OrderLine>();
            //    //insert orderline
            //    for (int i = 0; i < 10; ++i)
            //    {
            //        var orderline = new OrderLine();
            //        orderline.OrderId = i;
            //        orderline.ProductId = i * 10;
            //        orderline.Quantity = i * 100;
            //        db.Insert(orderline);
            //    }
            //}

        }

        private void button2_Click(object sender, EventArgs e)
        {
            ////read database
            //string dbfile = "d:\\WImageTest\\test_sqlite1.sqlite";
            //using (var db = new SQLiteConnection(dbfile, SQLiteOpenFlags.ReadOnly, true))
            //{
            //    var tableQuery = db.Table<OrderLine>();
            //    foreach (var orderline in tableQuery.Select<OrderLine>(s => s))
            //    {

            //    }
            //}
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //string dbfile = "d:\\WImageTest\\test_sqlite1.sqlite";
            //using (var db = new SQLiteConnection(dbfile, true))
            //{
            //    var tableQuery = db.Table<OrderLine>();
            //    tableQuery.Delete(orderline => orderline.Quantity > 300);
            //}
        }
        private void button4_Click(object sender, EventArgs e)
        {
             
        }
    }

}
