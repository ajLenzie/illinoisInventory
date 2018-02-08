using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InventorySystem
{
    public partial class Paint : Form
    {
        //  String for the location of the SQL Server
        string dataSource = @"Data Source=AJ-PC\SQLEXPRESS;Initial Catalog=Inventory;Integrated Security=True";
        DataTable dt = new DataTable ();
        bool searchClearButton = false;
        bool searchCleared = true;

        public Paint()
        {
            InitializeComponent();
        }

        private void Paint_Load(object sender, EventArgs e)
        {
            //  Automatically set the ComboBox to Tnemec @ 0.
            comboBox1.SelectedIndex = 0;
            LoadData();
        }

// Add Button
        private void button1_Click(object sender, EventArgs e)
        {
            //  Connect to the SQL Server
            SqlConnection con = new SqlConnection(dataSource);
            //  Logic
            con.Open();

            var sqlQuery = "";
            
            if (PaintExists(con, textBox1.Text))
            {
                sqlQuery = @"UPDATE [Inventory].[dbo].[Paint]
                           SET [PaintManufacturer] = '" + comboBox1.Text + "', [PaintSeries] = '" + textBox2.Text + "', [ColorCode] = '" + textBox3.Text + "', [PaintColor] = '" + textBox4.Text + "' ,[Quantity] = '" + textBox5.Text + "' WHERE [ProductCode] ='" + textBox1.Text + "'";
            }
            else
            {
                sqlQuery = @"INSERT INTO[Inventory].[dbo].[Paint] ([ProductCode], [PaintManufacturer], [PaintSeries], [ColorCode], [PaintColor], [Quantity])
                            VALUES
                            ('"+ textBox1.Text +"', '" + comboBox1.Text + "', '" + textBox2.Text + "', '" + textBox3.Text + "' , '" + textBox4.Text + "', '" + textBox5.Text + "')";
            }

            //  If Quantity is a string or NaN, throw an error and clear the Quantity field until the correct information is inserted!
            float f;
            if (textBox5.Text == null || !float.TryParse(textBox5.Text, out f))
            {
                MessageBox.Show("Invalid Quantity!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                textBox5.Clear();
                return;
            }

            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if (dataGridView1.Rows[i].Cells[0].Value != null && textBox1.Text == dataGridView1.Rows[i].Cells[0].Value.ToString())
                {
                    int nextAvailable = dataGridView1.Rows.Count;
                    MessageBox.Show("This Product Code is already in use!" +Environment.NewLine+ "Select the Update Button to Update!" + Environment.NewLine + "Next available Product Code number is : "
                        + nextAvailable, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    textBox1.Text = dataGridView1.Rows.Count.ToString ();
                    return;
                }

                for (int j = 0; j < dataGridView1.Columns.Count; j++)
                {
                    if (dataGridView1.Rows[i].Cells[1].Value.ToString() == comboBox1.Text && dataGridView1.Rows[i].Cells[2].Value.ToString() == textBox2.Text && 
                        dataGridView1.Rows[i].Cells[3].Value.ToString() == textBox3.Text && dataGridView1.Rows[i].Cells[4].Value.ToString() == textBox4.Text)
                    {
                        int subtract = i;
                        MessageBox.Show("This product is already entered, please update at Product Number : " + subtract, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
            }

            SqlCommand cmd = new SqlCommand(sqlQuery, con);
            cmd.ExecuteNonQuery();

            con.Close();

            //  Read Data
            LoadData();
        }

        private bool PaintExists(SqlConnection con, string productCode)
        {
            SqlDataAdapter sda = new SqlDataAdapter("Select 1 From [Paint] WHERE [ProductCode]= '"+ productCode +"'", con);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            if (dt.Rows.Count > 0)
                return true;
            else
                return false;
        }

        public void LoadData()
        {
            SqlConnection con = new SqlConnection(dataSource);
            con.Open();

            SqlDataAdapter sda = new SqlDataAdapter("Select * From Paint", con);
            dt = new DataTable();
            sda.Fill(dt);

            if (dataGridView1.DataSource != null)
                dataGridView1.DataSource = null;
            else
                dataGridView1.Rows.Clear();

            if (searchClearButton)
            {
                dataGridView1.Columns.Add("ProductCode", "PC");
                dataGridView1.Columns.Add("PaintManufacturer", "Manufacturer");
                dataGridView1.Columns.Add("PaintSeries", "Paint Series");
                dataGridView1.Columns.Add("ColorCode", "Color Code");
                dataGridView1.Columns.Add("PaintColor", "Paint Color");
                dataGridView1.Columns.Add("Quantity", "Quantity");

                foreach (DataGridViewColumn column in dataGridView1.Columns)
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                searchCleared = true;
            }

            foreach (DataRow item in dt.Rows)
            {
                int n = dataGridView1.Rows.Add();
                dataGridView1.Rows[n].Cells[0].Value = item["ProductCode"].ToString();
                dataGridView1.Rows[n].Cells[1].Value = item["PaintManufacturer"].ToString();
                dataGridView1.Rows[n].Cells[2].Value = item["PaintSeries"].ToString();
                dataGridView1.Rows[n].Cells[3].Value = item["ColorCode"].ToString();
                dataGridView1.Rows[n].Cells[4].Value = item["PaintColor"].ToString();
                dataGridView1.Rows[n].Cells[5].Value = item["Quantity"].ToString();
            }

            searchClearButton = false;
            con.Close();
        }
        
        private void dataGridView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            textBox1.Text = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
            comboBox1.Text = dataGridView1.SelectedRows[0].Cells[1].Value.ToString();
            textBox2.Text = dataGridView1.SelectedRows[0].Cells[2].Value.ToString();
            textBox3.Text = dataGridView1.SelectedRows[0].Cells[3].Value.ToString();
            textBox4.Text = dataGridView1.SelectedRows[0].Cells[4].Value.ToString();
            textBox5.Text = dataGridView1.SelectedRows[0].Cells[5].Value.ToString(); 
        }
        
// Delete Button
        private void button2_Click(object sender, EventArgs e)
        {
            SqlConnection con = new SqlConnection(dataSource);

            var sqlQuery = "";
            if (PaintExists(con, textBox1.Text))
            {
                if (MessageBox.Show("Are you sure you want to DELETE this item?", "WARNING", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
                {
                    con.Open();
                    sqlQuery = @"DELETE FROM [Inventory].[dbo].[Paint] WHERE [ProductCode] = '" + textBox1.Text + "'";

                    SqlCommand cmd = new SqlCommand(sqlQuery, con);
                    cmd.ExecuteNonQuery();

                    con.Close();
                }
                else
                {
                    return;
                }
            }
            else
            {
                MessageBox.Show("Double click a line item to select then try Deleting again!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            //  Read Data
            LoadData();
        }

// Update Button
        private void button3_Click(object sender, EventArgs e)
        {
            //  Connect to the SQL Server
            SqlConnection con = new SqlConnection(dataSource);
            //  Logic
            con.Open();

            var sqlQuery = "";

            //  If any textBox is empty, do not allow the user to Update.  If all boxes are filled in appropriately, allow the user to update.
            float f;
            int emptyI;
            if (textBox1.Text == null || !int.TryParse (textBox1.Text, out emptyI) || String.IsNullOrWhiteSpace (textBox2.Text) ||
                String.IsNullOrWhiteSpace(textBox3.Text) || String.IsNullOrWhiteSpace(textBox4.Text) || textBox5.Text == null || !float.TryParse(textBox5.Text, out f))
            {
                if (MessageBox.Show("Please fill out the appropriate data before Updating!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error) == DialogResult.OK);
                return;
            }

            if (PaintExists(con, textBox1.Text))
            {
                sqlQuery = @"UPDATE [Inventory].[dbo].[Paint]
                        SET [PaintManufacturer] = '" + comboBox1.Text + "', [PaintSeries] = '" + textBox2.Text + "', [ColorCode] = '" + textBox3.Text + "', [PaintColor] = '" + textBox4.Text + "' ,[Quantity] = '" + textBox5.Text + "' WHERE [ProductCode] ='" + textBox1.Text + "'";
            }
            else
            {
                sqlQuery = @"INSERT INTO[Inventory].[dbo].[Paint] ([ProductCode], [PaintManufacturer], [PaintSeries], [ColorCode], [PaintColor], [Quantity])
                        VALUES
                        ('" + textBox1.Text + "', '" + comboBox1.Text + "', '" + textBox2.Text + "', '" + textBox3.Text + "' , '" + textBox4.Text + "', '" + textBox5.Text + "')";
            }

            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if (dataGridView1.Rows[i].Cells[0].Value != null && textBox1.Text == dataGridView1.Rows[i].Cells[0].Value.ToString())
                {
                    if (MessageBox.Show("Are you sure you want to Update?", "WARNING", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.OK)
                    {
                        if (PaintExists(con, textBox1.Text))
                        {
                            sqlQuery = @"UPDATE [Inventory].[dbo].[Paint]
                        SET [PaintManufacturer] = '" + comboBox1.Text + "', [PaintSeries] = '" + textBox2.Text + "', [ColorCode] = '" + textBox3.Text + "', [PaintColor] = '" + textBox4.Text + "' ,[Quantity] = '" + textBox5.Text + "' WHERE [ProductCode] ='" + textBox1.Text + "'";
                        }
                    }
                    else
                    {
                        return;
                    }
                }
            }

            SqlCommand cmd = new SqlCommand(sqlQuery, con);
            cmd.ExecuteNonQuery();

            con.Close();

            //  Read Data
            LoadData();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
         
        }
//  Search Button
        private void button4_Click(object sender, EventArgs e)
        {
            searchCleared = false;

            SqlConnection con = new SqlConnection(dataSource);
            con.Open();

            if (!String.IsNullOrWhiteSpace(textBox2.Text))
            {
                dataGridView1.Columns.Clear();
                SqlDataAdapter sda = new SqlDataAdapter("SELECT * FROM Paint WHERE PaintSeries LIKE '" + textBox2.Text + "%'", con);
                dt = new DataTable();
                dataGridView1.DataSource = dt;
                sda.Fill(dt);
            }
            else if (!String.IsNullOrWhiteSpace(textBox3.Text))
            {
                dataGridView1.Columns.Clear();
                SqlDataAdapter sda = new SqlDataAdapter("SELECT * FROM Paint WHERE ColorCode LIKE '" + textBox3.Text + "%'", con);
                dt = new DataTable();
                dataGridView1.DataSource = dt;
                sda.Fill(dt);
            }
            else if (!String.IsNullOrWhiteSpace(textBox4.Text))
            {
                dataGridView1.Columns.Clear();
                SqlDataAdapter sda = new SqlDataAdapter("SELECT * FROM Paint WHERE PaintColor LIKE '" + textBox4.Text + "%'", con);
                dt = new DataTable();
                dataGridView1.DataSource = dt;
                sda.Fill(dt);
            }

            foreach (DataGridViewColumn column in dataGridView1.Columns)
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            con.Close();
        //    dv.RowFilter = string.Format("PaintSeries LIKE '%{0}%'", textBox2.Text);
    }

//  Clear Search Button
    private void button5_Click(object sender, EventArgs e)
        {
            if (!searchCleared)
            {
                searchClearButton = true;
                textBox1.Text = "";
                textBox2.Text = "";
                textBox3.Text = "";
                textBox4.Text = "";
                LoadData();
            }
            else
                return;
        }


    }
}
