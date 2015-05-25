using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;
using CsvHelper;
using Microsoft.VisualBasic;
using System.Xml;

namespace WindowsFormsApplication2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public void open_csv()
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "CSV Files|*.csv";
            openFileDialog1.Title = "Select a CSV";

            // Show the Dialog.
            // If the user clicked OK in the dialog and
            // a .CUR file was selected, open it.
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // Assign the cursor in the Stream to the Form's Cursor property.
                //this.Cursor = new Cursor(openFileDialog1);
                var cr = new StreamReader(openFileDialog1.FileName);
                var csv = new CsvReader(cr);
                string hF = "";
                while (csv.Read())
                {
                    hF = csv.FieldHeaders[0];
                    break;
                }
                dataGridView1.RowCount = 1;
                string[] slis = hF.Split(new Char[]{';',','});
                foreach (string news in slis){
                    DataGridViewRow row = (DataGridViewRow)dataGridView1.Rows[0].Clone();
                    row.Cells[0].Value = news;
                    dataGridView1.Rows.Add(row);
                }
                textBox1.Text = openFileDialog1.FileName;
                
            }
        }

        public void load_table()
        {
            foreach (string name in PG_class.load_table_name(textBox1.Text))
                        listBox1.Items.Add(name);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string typeSource = comboBox1.SelectedItem.ToString().Trim();
            switch (typeSource)
            {
                case "SQL":
                    load_table();
                    break;
                case "CSV":
                    open_csv();
                    break;
                    
                default:
                    break;
                    
            }
         
            

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            
        }

        private void listBox1_Click(object sender, EventArgs e)
        {
            //MessageBox.Show(listBox1.SelectedItem.ToString());
            int i = 0;
            dataGridView1.RowCount = 1;
            dataGridView1.ColumnCount = 3;
            foreach (string name in PG_class.load_colum_name(textBox1.Text, listBox1.SelectedItem.ToString())){
                DataGridViewRow row = (DataGridViewRow)dataGridView1.Rows[0].Clone();
                row.Cells[0].Value = name;
                dataGridView1.Rows.Add(row);
            }
            
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            if (comboBox1.SelectedItem == "SQL") {
                textBox1.Text = "Server=localhost;Port=5432;User=postgres;Password=root;Database=postgis_21_sample;";
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "XML Files|*.xml";
            saveFileDialog1.Title = "Save a XML";
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                XmlTextWriter textWritter = new XmlTextWriter(saveFileDialog1.FileName, Encoding.UTF8);
                textWritter.WriteStartDocument();
                textWritter.WriteStartElement("head");
                textWritter.WriteEndElement();
                textWritter.Close();


                for (int i = 0; i < dataGridView1.RowCount; i++)
                {

                }
            }
            
        }
    }
}

static class PG_class
{
    static public List<string> sql_query(String connectionString, String query)
    {
        NpgsqlConnection npgSqlConnection = new NpgsqlConnection(connectionString);
        npgSqlConnection.Open();
        NpgsqlCommand npgSqlCommand = new NpgsqlCommand(query, npgSqlConnection);
        NpgsqlDataReader npgSqlDataReader = npgSqlCommand.ExecuteReader();
        List<string> name_table = new List<string>();
        if (npgSqlDataReader.HasRows)
        {
            while (npgSqlDataReader.Read())
            {
                name_table.Add(npgSqlDataReader[0].ToString());
            }
        }
        npgSqlConnection.Close();
        return name_table;        
    }


    static public List<string> load_table_name(String connectionString)
    {
        return sql_query(connectionString, "select tablename as table from pg_tables where schemaname = 'agz_'");
    }
    static public List<string> load_colum_name(String connectionString, String table_name)
    {
        return sql_query(connectionString, string.Format("SELECT column_name FROM information_schema.columns  WHERE table_schema='agz_' AND table_name='{0}'", table_name));
    }
}
