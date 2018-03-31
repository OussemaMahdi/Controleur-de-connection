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
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        private void adressesBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.adressesBindingSource.EndEdit();
            this.tableAdapterManager.UpdateAll(this.testDataSet);

        }

        private void Form3_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'testDataSet.Adresses' table. You can move, or remove it, as needed.
            this.adressesTableAdapter.Fill(this.testDataSet.Adresses);

        }
    }
}
