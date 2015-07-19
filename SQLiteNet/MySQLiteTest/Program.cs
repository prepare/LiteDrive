using System;
using System.Collections.Generic;
using System.Windows.Forms;
using SQLite;
namespace MySQLiteTest
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form2());
        }
    }

    public class OrderLine
    {
        [AutoIncrement, PrimaryKey]
        public int Id { get; set; }
        [Indexed("IX_OrderProduct", 1)]
        public int OrderId { get; set; }
        [Indexed("IX_OrderProduct", 2)]
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public OrderLineStatus Status { get; set; }
    } 
    public enum OrderLineStatus
    {
        Placed = 1,
        Shipped = 100
    }
}
