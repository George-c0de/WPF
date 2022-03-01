using MySql.Data.MySqlClient;
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

namespace WindowsFormsApp1
{
    public partial class LoginForm : Form
    {
        Point lastPoint;
        public LoginForm()
        {
            InitializeComponent();
            //this.password.AutoSize=false;
            //this.password.Size = new Size(this.password.Size.Width,35);

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void buttonLogin_Click(object sender, EventArgs e)
        {
            Db db = new Db();
            db.openConnection();
            String loginUser = loginField.Text;
            String passUser = password.Text;
            bool admin;
            DataTable table = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter();

            SqlCommand command = new SqlCommand("SELECT * FROM users WHERE login = @uL AND password = @uP", db.getConnection());
            command.Parameters.Add("@uL",SqlDbType.VarChar).Value = loginUser;
            command.Parameters.Add("@uP", SqlDbType.VarChar).Value = passUser;

            adapter.SelectCommand = command;
            adapter.Fill(table);
            if (table.Rows.Count > 0)
            {
                var Name = table.Rows[0].ItemArray[3].ToString();
                int id = Convert.ToInt32(table.Rows[0].ItemArray[0]);
                this.Hide();
                if (table.Rows[0].ItemArray[4].ToString() == "ADMIN")
                {
                    admin = true;
                }
                else
                {
                    admin = false;
                }
                main mainForm = new main(id,admin);

                db.openConnection();
                mainForm.Show();
                mainForm.yourid.Text = id.ToString();
                mainForm.loginuser.Text = "Здравствуйте, " + Name;
            }
            else
            {

                db.openConnection();
                MessageBox.Show("Error");
            }
            
        }

        private void loginField_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {
            this.Hide();
            register registerForm = new register();
            registerForm.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Left += e.X - lastPoint.X;
                this.Top += e.Y - lastPoint.Y;

            }
        }

        private void button1_MouseDown(object sender, MouseEventArgs e)
        {
            lastPoint = new Point(e.X, e.Y);
        }
    }
}
