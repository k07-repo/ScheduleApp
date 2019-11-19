using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

using MySql;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace ScheduleApp
{
    public partial class ScheduleForm : Form
    {       
        private DBConnection dbCon;
        public ScheduleForm()
        {
            InitializeComponent();

            //set the minimum date here because the property window doesn't like it
            dateTimePicker1.MinDate = DateTime.Today;

            comboBox1.Items.Add("Current Items");
            comboBox1.Items.Add("Completed Items");
        }

        private void Schedule_Load(object sender, EventArgs e)
        {
            dbCon = DBConnection.Instance();
            dbCon.DatabaseName = "myDB";
            loadData("scheduledTasks");
        }

   
        private void loadData(String tableName)
        {
            DataSet dataSet = new DataSet();
            if (dbCon.IsConnected())
            {
                String query = String.Format("SELECT * FROM {0}", tableName);
                var cmd = new MySqlCommand(query, dbCon.Connection);

                MySqlDataAdapter mySqlAdapter = new MySqlDataAdapter(cmd);
                mySqlAdapter.Fill(dataSet);
                dataGridView3.AutoGenerateColumns = true; //this is really important apparently
                dataGridView3.DataSource = dataSet.Tables[0];
                //dbCon.Close();
            }
        } 

        private void dataGridView3_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dbCon.IsConnected())
            {
                DateTime dateTime = dateTimePicker1.Value;
                String dateString = dateTime.ToString("yyyy-MM-dd");

                String query = String.Format("INSERT INTO scheduledTasks VALUES (NULL, \"{0}\", \'{1}\', false)", textBox2.Text, dateString);
                var cmd = new MySqlCommand(query, dbCon.Connection);
                cmd.ExecuteNonQuery();
                loadData("scheduledTasks");
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.Text.Equals("Current Items"))
            {
                loadData("scheduledTasks");
            }
            else if(comboBox1.Text.Equals("Completed Items"))
            {
                loadData("completedTasks");
            }
        }
    }
}
