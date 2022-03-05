using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;

using System.Threading.Tasks;
using System.Data;

namespace WindowsFormsApp1
{
    internal class Db
    {
        //MySqlConnection connection= new MySqlConnection("server=localhost;port=3306;username=root;password=root;database=project");


        //MySqlConnection connection = new MySqlConnection("Data Source=localhost;Initial Catalog=project;Integrated Security=True");
        //MySqlConnection connection = new MySqlConnection("Data Source=localhost;Initial Catalog=project;Integrated Security=True");
        //MySqlConnection connection = new MySqlConnection(@"Data Source=localhost;username = gerae; Initial Catalog=project;Integrated Security=True");
        //MySqlConnection connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["WindowsFormsApp1.Properties.Settings.projectConnectionString"].ConnectionString);
        //string connString = "Data Source=localhost;Initial Catalog = project; Integrated Security = True";
        SqlConnection connection = new SqlConnection("Data Source=localhost;Initial Catalog = project; Integrated Security = True");
        public void openConnection()
        {
            getConnection();
            if (connection.State != System.Data.ConnectionState.Open)
                connection.Open();

        }
        public void closeConnection()
        {
            if (connection.State != System.Data.ConnectionState.Closed)
                connection.Close();
        }
        public SqlConnection getConnection()
        {
            return connection;
        }

    }
}
