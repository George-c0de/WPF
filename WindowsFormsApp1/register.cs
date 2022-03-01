using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class register : Form
    {
        Point lastPoint;
        public register()
        {
            InitializeComponent();
            Name.Text = "Введите имя";
            Name.ForeColor = Color.Gray;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Name_Enter(object sender, EventArgs e)
        {
            if (Name.Text == "Введите имя")
            {
                Name.Text = "";
                Name.ForeColor = Color.Black;
            }
        }

        private void Name_Leave(object sender, EventArgs e)
        {
            if (Name.Text == "")
            {
                Name.Text = "Введите имя";
                Name.ForeColor = Color.Gray;
            }
        }

        [Obsolete]
        private void buttonRegister_Click(object sender, EventArgs e)
        {
            if (Name.Text == "Введите имя")
            {
                MessageBox.Show("Введите имя");
                return;
            }
            if (midlename.Text == "")
            {
                MessageBox.Show("Введите фамилию");
                return;
            }
            if (password.Text!=password2.Text)
            {
                MessageBox.Show("Пароли не совпадают");
                return;
            }
            if (password.Text =="")
            {
                MessageBox.Show("Введите пароль");
                return;
            }
            if (loginField.Text == "")
            {
                MessageBox.Show("Введите логин");
                return;
            }
            if (chechUser())
            {
                return;
            }
            if (email.Text == "")
            {
                MessageBox.Show("Введите email");
                return;
            }
            string pattern = "[.\\-_a-z0-9]+@([a-z0-9][\\-a-z0-9]+\\.)+[a-z]{2,6}";
            Match isMatch = Regex.Match(email.Text, pattern, RegexOptions.IgnoreCase);
            if (!(isMatch.Success))
            {
                MessageBox.Show("Введите email");
                return;
            }
            Db db = new Db();

            DateTime thisDay = DateTime.Now;
            SqlCommand command = new SqlCommand("INSERT INTO users (login, password, name, role, middlename, discount) VALUES (@login, @password, @name, 'USER', @middlename, '0');",db.getConnection());
            command.Parameters.Add("@login",SqlDbType.Text).Value=loginField.Text;
            command.Parameters.Add("@password", SqlDbType.Text).Value = password.Text;
            command.Parameters.Add("@name", SqlDbType.Text).Value = Name.Text;
            //command.Parameters.Add("@role", SqlDbType.Text).Value = loginField.Text;
            command.Parameters.Add("@middlename", SqlDbType.Text).Value = midlename.Text;
            //command.Parameters.Add("@disc", SqlDbType.Text).Value = 0;

           
            db.openConnection();

            if (command.ExecuteNonQuery() == 1)
            {
                MessageBox.Show("Account created");
            }
            else
            {
                MessageBox.Show("Account not created");
            }


            DataTable table_user = new DataTable();
            SqlDataAdapter adapter_user = new SqlDataAdapter();
            SqlCommand command_user = new SqlCommand("SELECT * FROM users ORDER BY users.id DESC", db.getConnection());
            adapter_user.SelectCommand = command_user;
            adapter_user.Fill(table_user);



            SqlCommand command_info = new SqlCommand("INSERT INTO info_user (id_user, email, lastvisits, dateregister) VALUES (@iU, @Email, @lastvisits, @dateregister);", db.getConnection());
            command_info.Parameters.Add("@iU", SqlDbType.Int).Value = table_user.Rows[0].ItemArray[0];
            command_info.Parameters.Add("@Email", SqlDbType.Text).Value = email.Text;
            command_info.Parameters.Add("@lastvisits", SqlDbType.DateTime).Value = thisDay;
            command_info.Parameters.Add("@dateregister", SqlDbType.DateTime).Value = thisDay;
            command_info.ExecuteNonQuery();

            this.Hide();
            LoginForm LoginForm = new LoginForm();
            LoginForm.Show();

            db.closeConnection();
        }
        public Boolean chechUser()
        {
            Db db = new Db();
            DataTable table = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter();

            SqlCommand command = new SqlCommand("SELECT * FROM users WHERE login = @uL", db.getConnection());
            command.Parameters.Add("@uL", SqlDbType.Text).Value = loginField.Text;

            adapter.SelectCommand = command;
            adapter.Fill(table);

            if (table.Rows.Count > 0)
            {
                MessageBox.Show("Такой логин уже существует, введите другой");
                return true;
            }
            else
            {
                return false;
            }
        }

        private void password2_Leave(object sender, EventArgs e)
        {
            if (password.Text != password2.Text && password.Text!="")
            {
                passworderror.ForeColor = Color.Red;
                passworderror.Text = "Пароли не совпадают";
                passworderror.ForeColor = Color.Red;
            }
            else
            {
                passworderror.ForeColor = Color.Black;
                passworderror.Text = "";
                passworderror.ForeColor = Color.Black;
            }
        }

        private void password_Leave(object sender, EventArgs e)
        {
            if (password.Text != password2.Text && password2.Text != "")
            {
                passworderror.ForeColor = Color.Red;
                passworderror.Text = "Пароли не совпадают";
                passworderror.ForeColor = Color.Red;
            }
            else
            {
                passworderror.ForeColor = Color.Black;
                passworderror.Text = "";
                passworderror.ForeColor = Color.Black;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void label6_Click(object sender, EventArgs e)
        {
            this.Hide();
            LoginForm loginForm = new LoginForm();
            loginForm.Show();
        }

        private void register_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Left += e.X - lastPoint.X;
                this.Top += e.Y - lastPoint.Y;

            }
        }

        private void register_MouseDown(object sender, MouseEventArgs e)
        {
            lastPoint = new Point(e.X, e.Y);
        }
    }
}
