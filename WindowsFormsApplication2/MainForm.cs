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
    public partial class MainForm : Form
    {
        public MainForm()
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
            PG_class.select_table = listBox1.SelectedItem.ToString();
            dataGridView1.RowCount = 1;
            dataGridView1.ColumnCount = 3;
            foreach (string name in PG_class.load_colum_name(textBox1.Text, listBox1.SelectedItem.ToString())){
                DataGridViewRow row = (DataGridViewRow)dataGridView1.Rows[0].Clone();
                row.Cells[0].Value = name;
                row.Cells[1].Value = false;
                row.Cells[2].Value = false;
                dataGridView1.Rows.Add(row);
                (dataGridView2.Rows[0].Cells[0] as DataGridViewComboBoxCell).Items.Add(name);
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
            dataGridView1.EndEdit();

            List<string> all_field = new List<string>();
            List<string> input_field = new List<string>();
            List<string> output_field = new List<string>();
            
            string sql_query = "SELECT ";
            for (int i = 1; i < dataGridView1.RowCount; i++) { 
                sql_query += dataGridView1.Rows[i].Cells[0].Value.ToString() + ", ";
                if ((dataGridView1.Rows[i].Cells[1] as DataGridViewCheckBoxCell).Value.ToString() == "True")                    
                    input_field.Add(dataGridView1.Rows[i].Cells[0].Value.ToString());
                if ((dataGridView1.Rows[i].Cells[2] as DataGridViewCheckBoxCell).Value.ToString() == "True")
                    output_field.Add(dataGridView1.Rows[i].Cells[0].Value.ToString());
                all_field.Add(dataGridView1.Rows[i].Cells[0].Value.ToString());
            }
            sql_query= sql_query.Remove(sql_query.Length - 2, 1);
            sql_query += " FROM " + PG_class.select_table;
            if (dataGridView2.RowCount > 1)
            {
                sql_query += " WHERE ";
                for (int i = 0; i < dataGridView2.RowCount-1; i++)
                {
                    dataGridView2.EndEdit();
                    sql_query += (dataGridView2.Rows[i].Cells[0] as DataGridViewComboBoxCell).Value.ToString() + " " +
                        (dataGridView2.Rows[i].Cells[1] as DataGridViewComboBoxCell).Value.ToString() + " " +
                        (dataGridView2.Rows[i].Cells[2] as DataGridViewTextBoxCell).Value.ToString() + " AND ";
                }
                sql_query = sql_query.Remove(sql_query.Length - 2, 1);
            }
            //MessageBox.Show(sql_query);
            
            SaveXML(ref dataGridView3, sql_query, all_field, input_field, output_field);
                             

        }

        private static void SaveXML(ref DataGridView df, string sql_query,List<string> all_field,List<string> input_field, List<string> output_field)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "XML Files|*.xml";
            saveFileDialog1.Title = "Save a XML";
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                XmlTextWriter textWritter = new XmlTextWriter(saveFileDialog1.FileName, Encoding.UTF8);
                textWritter.WriteStartDocument();
                textWritter.WriteStartElement("source");
                textWritter.WriteStartElement("sql");
                textWritter.WriteString(sql_query);
                textWritter.WriteEndElement();

                foreach (string ipn in input_field)
                {
                    textWritter.WriteStartElement("input");
                    textWritter.WriteString(ipn);
                    textWritter.WriteEndElement();
                }

                foreach (string ipn in output_field)
                {
                    textWritter.WriteStartElement("output");
                    textWritter.WriteString(ipn);
                    textWritter.WriteEndElement();
                }
                MessageBox.Show(df.Rows[1].Cells[0].Value.ToString());
                for (int i = 1; i < df.RowCount-1; i++)
                {
                    textWritter.WriteStartElement(df.Rows[i].Cells[0].Value.ToString());
                    textWritter.WriteStartAttribute("Value");
                    textWritter.WriteString(df.Rows[i].Cells[1].Value.ToString());
                    textWritter.WriteStartAttribute("Value1");
                    textWritter.WriteString(df.Rows[i].Cells[2].Value.ToString());
                    textWritter.WriteEndElement();
                }
                
                textWritter.WriteEndElement();
                
               
                textWritter.Close();
                
            }
        }



        private void button3_Click(object sender, EventArgs e)
        {
            
        }

        private void dataGridView2_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            foreach (string nams in PG_class.tbl_name){
                (dataGridView2.Rows[dataGridView2.RowCount-1].Cells[0] as DataGridViewComboBoxCell).Items.Add(nams);
            }
        }

        private void comboBox4_SelectedValueChanged(object sender, EventArgs e)
        {
            
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            dataGridView3.Rows.Clear();
            if (comboBox4.SelectedItem == "regression")
            {
                string[] param = {"", "criterion", "splitter", "max_features", "max_depth","min_samples_split",
                             "min_samples_leaf", "min_weight_fraction_leaf","max_leaf_nodes"};
                string[][] param_option = new string[param.Length][];
                param_option[0] = new string[0] { };
                param_option[1] = new string[2] { "gini", "entropy" };
                param_option[2] = new string[2] { "best", "random" };
                param_option[3] = new string[5] { "int", "auto", "sqrt", "log2", "None" };
                param_option[4] = new string[1] { "int" };
                param_option[5] = new string[1] { "int" };
                param_option[6] = new string[1] { "int" };
                param_option[7] = new string[1] { "int" };
                param_option[8] = new string[1] { "int" };
                int i = 0;
                foreach (string one_p in param)
                {
                    DataGridViewRow row = (DataGridViewRow)dataGridView3.Rows[0].Clone();
                    row.Cells[0].Value = one_p;
                    foreach (string one in param_option[i])
                        (row.Cells[1] as DataGridViewComboBoxCell).Items.Add(one);
                    //(row.Cells[2] as DataGridViewTextBoxCell)
                    row.Cells[1].Value = "";
                    row.Cells[2].Value = "";
                    dataGridView3.Rows.Add(row);
                    i++;
                }
            }
        }

 
    }

}

static class PG_class
{
    static public string select_table;
    static public List<string> tbl_name;
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
        tbl_name = sql_query(connectionString, string.Format("SELECT column_name FROM information_schema.columns  WHERE table_schema='agz_' AND table_name='{0}'", table_name));
        return tbl_name;
    }
}


