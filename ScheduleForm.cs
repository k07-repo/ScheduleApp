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
        }

        private void Schedule_Load(object sender, EventArgs e)
        {
            dbCon = DBConnection.Instance();
            dbCon.DatabaseName = "myDB";
            loadData();
        }

        private void loadData()
        {
            DataSet dataSet = new DataSet();
            if (dbCon.IsConnected())
            {
                String query = "SELECT * FROM scheduledTasks";
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
                String query = String.Format("INSERT INTO scheduledTasks VALUES (NULL, \"{0}\", \'2019-11-30\', false)", textBox2.Text);
                var cmd = new MySqlCommand(query, dbCon.Connection);
                cmd.ExecuteNonQuery();
                loadData();
            }
        }
    }
}
