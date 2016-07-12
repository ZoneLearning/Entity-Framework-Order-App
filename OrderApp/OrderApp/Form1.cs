using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OrderApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            OrderAppDataContext dc = new OrderAppDataContext();
            dataGridView1.DataSource = dc.Products;
            cmbCustomer.DataSource = dc.Customers;
            cmbCustomer.DisplayMember = "CompanyName";
            cmbCustomer.ValueMember = "CustomerID";

            //lambda hali
            cmbEmployee.DataSource = dc.Employees.Select(x => new
            {
                x.EmployeeID,
                AdSoyad = x.FirstName + " " + x.LastName  //AdSoyad alias oldu.
            });
            cmbEmployee.DisplayMember = "AdSoyad";
            cmbEmployee.ValueMember = "EmployeeID";



            /*yukardaki kodun linq expression hali
            cmbEmployee.DataSource = from personal in dc.Employees
                                     select new
                                     {
                                         personal.EmployeeID,
                                         AdSoyad = personal.FirstName + "" + personal.LastName
                                     };*/


            cmbShipVia.DataSource = dc.Shippers;
            cmbShipVia.DisplayMember = "CompanyName";
            cmbShipVia.ValueMember = "ShipperID";
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            DataGridViewRow row = dataGridView1.CurrentRow;
            if (row == null) return;


            ListViewItem list = new ListViewItem();
            list.Text = row.Cells["ProductName"].Value.ToString();
            list.SubItems.Add(row.Cells["UnitPrice"].Value.ToString());
            list.SubItems.Add(nudQuantity.Value.ToString());
            list.SubItems.Add(nudDiscount.Value.ToString());
            list.Tag = row.Cells["ProductID"].Value;

            listView1.Items.Add(list);

            nudDiscount.Value = 0;
            nudQuantity.Value = 1;

        }

        private void btnConfirmOrder_Click(object sender, EventArgs e)
        {
            OrderAppDataContext context = new OrderAppDataContext();

            if (cmbCustomer.SelectedItem == null || cmbEmployee.SelectedItem == null || cmbShipVia.SelectedItem == null) return;



            Order new_order = new Order();
            new_order.ShippedDate = DateTime.Now;
            new_order.CustomerID = cmbCustomer.SelectedValue.ToString();
            new_order.EmployeeID = (int)cmbEmployee.SelectedValue;
            new_order.ShipVia = (int)cmbShipVia.SelectedValue;

            context.Orders.InsertOnSubmit(new_order);
            context.SubmitChanges();

            foreach (ListViewItem item in listView1.Items)
            {

                Order_Detail detail = new Order_Detail();

                detail.OrderID = new_order.OrderID;
                detail.ProductID = (int)item.Tag;
                detail.UnitPrice = decimal.Parse(item.SubItems[1].Text);
                detail.Quantity = short.Parse(item.SubItems[2].Text);
                detail.Discount = float.Parse(item.SubItems[3].Text) / 100;

                context.Order_Details.InsertOnSubmit(detail);
                context.SubmitChanges();
            }

            listView1.Items.Clear();
            cmbCustomer.SelectedIndex = cmbShipVia.SelectedIndex = -1;
        }
    }
}
