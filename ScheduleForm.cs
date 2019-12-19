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
        private string CURRENT_TABLE_NAME = "Current Items";
        private string COMPLETED_TABLE_NAME = "Completed Items";

        private DBConnection dbCon = DBConnection.Instance();

        public ScheduleForm()
        {
            InitializeComponent();

            //set the minimum date here because the property window doesn't like it
            dateTimePicker1.MinDate = DateTime.Today;
          
            //set up the combo box
            comboBox1.Items.Add(CURRENT_TABLE_NAME);
            comboBox1.Items.Add(COMPLETED_TABLE_NAME);
            comboBox1.SelectedIndex = 0;
        }

        private void createConnection()
        {      
            dbCon = DBConnection.Instance();
            dbCon.DatabaseName = "myDB";
        }

        private void Schedule_Load(object sender, EventArgs e)
        {
            createConnection();
            loadData(dataGridView3, "scheduledTasks");
            loadData(dataGridView1, "dailyTasks");
        }
   
        private void loadData(DataGridView dataGridView, String tableName)
        {
            DataSet dataSet = new DataSet();
            if (dbCon.IsConnected())
            {
                String query = String.Format("SELECT * FROM {0}", tableName);
                var cmd = new MySqlCommand(query, dbCon.Connection);

                MySqlDataAdapter mySqlAdapter = new MySqlDataAdapter(cmd);
                mySqlAdapter.Fill(dataSet);
                dataGridView.AutoGenerateColumns = true; //this is really important apparently
                dataGridView.DataSource = dataSet.Tables[0];
                //dbCon.Close();
            }
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
                loadData(dataGridView3, "scheduledTasks");
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.Text.Equals(CURRENT_TABLE_NAME))
            {
                loadData(dataGridView3, "scheduledTasks");
                button3.Enabled = false;
            }
            else if(comboBox1.Text.Equals(COMPLETED_TABLE_NAME))
            {
                loadData(dataGridView3, "completedTasks");                
                button3.Enabled = true;
            }
        }

        //Marks a task completed
        private void button1_Click(object sender, EventArgs e)
        {
            //Get all changes to the table
            var changes = ((DataTable)dataGridView3.DataSource).GetChanges(DataRowState.Modified);
            if(changes == null)
            {
                return;
            }          
            var changedRows = changes.Rows;
        
            for(int index = 0; index < changedRows.Count; index++)
            {
                DataRow row = changedRows[index];
                String id = row[0].ToString();
                String desc = row[1].ToString();
                String enddate = row[2].ToString();
                DateTime date = DateTime.Parse(enddate);
                enddate = date.ToString("yyyy-MM-dd");

                String complete = row[3].ToString();
                             
                if (Boolean.Parse(complete) != true)
                {
                    //Do nothing if the task isn't marked as complete
                    continue; 
                }
                else
                {
                    //Place the item into the completed tasks SQL table (the same values, but completed is set to true)
                    String query = String.Format("INSERT INTO completedTasks VALUES ({0}, \"{1}\", \'{2}\', true)", id, desc, enddate);
                    var cmd = new MySqlCommand(query, dbCon.Connection);
                    cmd.ExecuteNonQuery();

                    //Delete it from the scheduled tasks table (can be done with just the ID)
                    query = String.Format("DELETE FROM scheduledTasks WHERE id={0}", id);
                    cmd = new MySqlCommand(query, dbCon.Connection);
                    cmd.ExecuteNonQuery();
                }
            }         

            loadData(dataGridView3, "scheduledTasks");
        }

        //Clears the completed tasks table
        private void button3_Click(object sender, EventArgs e)
        {
            if(dbCon.IsConnected())
            {
                String query = "DELETE FROM completedTasks";
                var cmd = new MySqlCommand(query, dbCon.Connection);
                cmd.ExecuteNonQuery();
                loadData(dataGridView3, "completedTasks");
            }
        }

      
        private void label3_Click(object sender, EventArgs e)
        {
            if (dbCon.IsConnected())
            {
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (dbCon.IsConnected())
            {

                String desc = textBox1.Text;
                String startTime = textBox3.Text;
                String endTime = textBox4.Text;


                String query = String.Format("INSERT INTO dailyTasks VALUES (NULL, \"{0}\", \"{1}\", \"{2}\", false)", desc, startTime, endTime);
                var cmd = new MySqlCommand(query, dbCon.Connection);
                cmd.ExecuteNonQuery();
                loadData(dataGridView1, "dailyTasks");
            }
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {

            //Get all changes to the table
            var changes = ((DataTable)dataGridView1.DataSource).GetChanges(DataRowState.Modified);
            if (changes == null)
            {
                return;
            }
            var changedRows = changes.Rows;

            for (int index = 0; index < changedRows.Count; index++)
            {
                DataRow row = changedRows[index];
                String id = row[0].ToString();
                String complete = row[4].ToString();

                if (Boolean.Parse(complete) != true)
                {
                    //Do nothing if the task isn't marked as complete
                    continue;
                }
                else
                {                        
                    //Delete it from the daily tasks table (can be done with just the ID)
                    String query = String.Format("DELETE FROM dailyTasks WHERE id={0}", id);
                    var cmd = new MySqlCommand(query, dbCon.Connection);
                    cmd.ExecuteNonQuery();
                }
            }

            loadData(dataGridView1, "dailyTasks");
        }
    }
}
