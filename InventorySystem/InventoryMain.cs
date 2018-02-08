using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InventorySystem
{
    public partial class InventoryMain : Form
    {
 
        public InventoryMain()
        {
            InitializeComponent();
        }

        private void paintToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Paint paint = new InventorySystem.Paint();
            paint.MdiParent = this;
            paint.Show();
        }

        private void InventoryMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
