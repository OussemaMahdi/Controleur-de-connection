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
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void administrateursBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.administrateursBindingSource.EndEdit();
            this.tableAdapterManager.UpdateAll(this.testDataSet);

        }

        private void Form2_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'testDataSet.Administrateurs' table. You can move, or remove it, as needed.
            this.administrateursTableAdapter.Fill(this.testDataSet.Administrateurs);

        }
    }
}
