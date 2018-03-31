using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApplication7
{
    public partial class Form4 : Form
    {
        public Form4()
        {
            InitializeComponent();
        }

        private void mailBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.mailBindingSource.EndEdit();
            this.tableAdapterManager.UpdateAll(this.testDataSet);

        }

        private void Form4_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'testDataSet.Mail' table. You can move, or remove it, as needed.
            this.mailTableAdapter.Fill(this.testDataSet.Mail);

        }
    }
}
