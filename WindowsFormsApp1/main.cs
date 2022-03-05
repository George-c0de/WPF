using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Net.Mail;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class main : Form
    {
        int id_salons,id_user, id_servicegroup, id_persone,id_service;
        int oldRowIndex = -1;
        double discount_your,price;
        public main(int id,bool admin)
        {
            InitializeComponent();
            DateTime thisDay = DateTime.Today;
            DateTime localDate = DateTime.Now;
            Db db = new Db();
            db.openConnection();
            id_user = id;
            DataTable table = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter();

            SqlCommand command = new SqlCommand("SELECT * FROM users WHERE id = @uI", db.getConnection());
            command.Parameters.Add("@uI", SqlDbType.Int).Value = id;
            adapter.SelectCommand = command;
            adapter.Fill(table);
            yourname.Text = table.Rows[0].ItemArray[3].ToString();
            loginname.Text = yourname.Text;

            DataTable table_info = new DataTable();
            SqlDataAdapter adapter_info = new SqlDataAdapter();
            SqlCommand command_info = new SqlCommand("SELECT * FROM info_user WHERE id_user = @uI", db.getConnection());
            command_info.Parameters.Add("@uI", SqlDbType.Int).Value = id;
            adapter_info.SelectCommand = command_info;
            adapter_info.Fill(table_info);

            email.Text = table_info.Rows[0].ItemArray[1].ToString();
            allvisits.Text = table_info.Rows[0].ItemArray[5].ToString();
            dateregister.Text = table_info.Rows[0].ItemArray[4].ToString();
            averagechech.Text = table_info.Rows[0].ItemArray[3].ToString();
            lastvisits.Text = localDate.ToString();

            SqlCommand command_last = new SqlCommand("UPDATE info_user SET lastvisits = @uL WHERE info_user.id_user = @uI;", db.getConnection());
            command_last.Parameters.Add("@uI", SqlDbType.Int).Value = id;
            command_last.Parameters.Add("@uL", SqlDbType.Date).Value = localDate;
            command_last.ExecuteNonQuery();


            discount_your = Convert.ToDouble(table.Rows[0].ItemArray[6]);
            if (!admin)
            {
                tabControl1.TabPages.Remove(adminpanel);
            }
            yourmidlename.Text = table.Rows[0].ItemArray[5].ToString();
            yourdecount.Text = table.Rows[0].ItemArray[6].ToString();


            SqlCommand command2 = new SqlCommand("SELECT * FROM servgroups", db.getConnection());
            DataTable table2 = new DataTable();
            adapter.SelectCommand = command2;
            adapter.Fill(table2);
            foreach (DataRow item in table2.Rows)
            {
                var a = servgroup.Text;
                servgroup.Items.Add(item.ItemArray[1].ToString());
            }
            foreach (DataRow item in table2.Rows)
            {
                var a = servgroup.Text;
                groupchange.Items.Add(item.ItemArray[1].ToString());
            }
            foreach (DataRow item in table2.Rows)
            {
                var a = servgroup.Text;
                groupdelete.Items.Add(item.ItemArray[1].ToString());
            }
            SqlDataAdapter adaptersalon = new SqlDataAdapter();
            DataTable tablesalon = new DataTable();
            SqlCommand commandsalon = new SqlCommand("SELECT * FROM salons", db.getConnection());
            adaptersalon.SelectCommand = commandsalon;
            adaptersalon.Fill(tablesalon);
            foreach (DataRow item in tablesalon.Rows)
            {
                salons.Items.Add(item.ItemArray[1].ToString());
            }

            DataTable table3 = new DataTable();
            SqlDataAdapter adapter3 = new SqlDataAdapter();
            SqlCommand command3 = new SqlCommand("SELECT * FROM visits WHERE id_user = @uI", db.getConnection());
            command3.Parameters.Add("@uI", SqlDbType.Int).Value = id_user;
            adapter3.SelectCommand = command3;
            adapter3.Fill(table3);

            datanear_update(id_user);

            number.Maximum = datanear.Rows.Count;
            db.closeConnection();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Db db = new Db();
            db.openConnection();
            DateTime localDate = DateTime.Now;
            SqlCommand command_last = new SqlCommand("UPDATE info_user SET lastvisits = @uL WHERE info_user.id_user = @uI;", db.getConnection());
            command_last.Parameters.Add("@uI", SqlDbType.Int).Value = id_user;
            command_last.Parameters.Add("@uL", SqlDbType.Date).Value = localDate;
            command_last.ExecuteNonQuery();
            db.closeConnection();
            Application.Exit();
        }
        private void main_Load(object sender, EventArgs e)
        {

        }

        private void services_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (salons.Text != "" && services.Text != "")
            {
                Db db = new Db();
                DataTable table = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter();

                SqlCommand command = new SqlCommand("SELECT * FROM master,positions,servgroups WHERE id_salon = @uS AND id_positions = positions.id AND servgroups.id = positions.id_group", db.getConnection());
                command.Parameters.Add("@uS", SqlDbType.Int).Value = id_salons;
                adapter.SelectCommand = command;
                adapter.Fill(table);
                master.Items.Clear();
                master.Text = "";
                foreach (DataRow item in table.Rows)
                {
                    master.Items.Add(item.ItemArray[1].ToString());
                }
            }
            else
                return;
        }

        private void creater_Click(object sender, EventArgs e)
        {
            datanear_update(id_user);
            DateTime thisDay = DateTime.Today;
            Db db = new Db();
            if (master.Text=="")
            {
                MessageBox.Show("Выберите мастера");
                return;
            }
            if (salons.Text == "")
            {
                MessageBox.Show("Выберите салон");
                return;
            }
            if (servgroup.Text == "")
            {
                MessageBox.Show("Выберите группу услуг");
                return;
            }
            if (services.Text == "")
            {
                MessageBox.Show("Выберите услугу");
                return;
            }
            DataTable table3 = new DataTable();
            SqlDataAdapter adapter3 = new SqlDataAdapter();
            SqlCommand command3 = new SqlCommand("SELECT * FROM visits", db.getConnection());
            adapter3.SelectCommand = command3;
            adapter3.Fill(table3);
            if (date.Value< thisDay)
            {
                MessageBox.Show("Некорректная дата");
                return;
            }

            DataTable table4 = new DataTable();
            SqlDataAdapter adapter4 = new SqlDataAdapter();

            SqlCommand command4 = new SqlCommand("SELECT * FROM salons WHERE id = @uS ", db.getConnection());
            command4.Parameters.Add("@uS", SqlDbType.Int).Value = id_salons;
            adapter4.SelectCommand = command4;
            adapter4.Fill(table4);

            DateTime timeopen = Convert.ToDateTime(table4.Rows[0].ItemArray[2].ToString());
            DateTime timeclose = Convert.ToDateTime(table4.Rows[0].ItemArray[3].ToString());
            if (!((time.Value.Hour > timeopen.Hour) && (time.Value.Hour < timeclose.Hour)))
            {
                time.Value = thisDay;
                time.Value.AddDays(1);
                MessageBox.Show("Некорректная дата");
                return;
            }
            
            label10.Text = "";
            DataTable table2 = new DataTable();
            SqlDataAdapter adapter2 = new SqlDataAdapter();
            SqlCommand command2 = new SqlCommand("SELECT * FROM master WHERE name =@uN ", db.getConnection());
            command2.Parameters.Add("@uN", SqlDbType.VarChar).Value = master.Text;
            adapter2.SelectCommand = command2;
            adapter2.Fill(table2);
            id_persone = Convert.ToInt32(table2.Rows[0].ItemArray[0]);

            DataTable table = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter();
            SqlCommand command = new SqlCommand("INSERT INTO visits (id_user, id_service, id_personal, Дата, Время, Услуга_оказана, id_salon,price) VALUES (@id_user, @id_service, @id_personal, @date, @time, '0', @id_salon,@price)", db.getConnection());
            command.Parameters.Add("@id_user", SqlDbType.Int).Value = id_user;
            command.Parameters.Add("@id_service", SqlDbType.Int).Value =id_service ;
            command.Parameters.Add("@id_personal", SqlDbType.Int).Value = id_persone;
            command.Parameters.Add("@date", SqlDbType.Date).Value = date.Value;
            var time2 = time.Value;
            command.Parameters.Add("@time", SqlDbType.DateTime).Value = time.Value;
            command.Parameters.Add("@id_salon", SqlDbType.Int).Value = id_salons;
            command.Parameters.Add("@price", SqlDbType.Int).Value = price-(price*(discount_your/100));
            adapter.SelectCommand = command;
            adapter.Fill(table);
            foreach (DataRow row in table3.Rows)
            {
                if (Convert.ToDateTime(row.ItemArray[4]) == date.Value.Date)
                {
                    var a = row.ItemArray[4];
                    TimeSpan b = (TimeSpan)row.ItemArray[5];
                    //Timespan
                    if (b.Hours == time.Value.Hour && ((Math.Abs(b.Minutes - time.Value.Minute)) <= 30))
                    {
                        if (Convert.ToInt32(row.ItemArray[3]) == id_persone)
                        {
                            MessageBox.Show("На эту дату уже есть запись");
                            time.Value = thisDay;
                            time.Value.AddDays(1);
                            return;
                        }
                    }
                    int c = b.Hours;
                    if (Math.Abs(c - time.Value.Hour) < 1)
                    {
                        MessageBox.Show("Предыдущая запись еще идет");
                        time.Value = thisDay;
                        time.Value.AddDays(1);
                        return;
                    }
                }
                datanear_update(id_user);
            }
            MessageBox.Show("Вы успешно записались");

        }

        private void time_Leave(object sender, EventArgs e)
        {
            if (salons.Text != "")
            {
                Db db = new Db();
                DataTable table = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter();

                SqlCommand command = new SqlCommand("SELECT * FROM salons WHERE id = @uS ", db.getConnection());
                command.Parameters.Add("@uS", SqlDbType.Int).Value = id_salons;
                adapter.SelectCommand = command;
                adapter.Fill(table);

                DataTable table3 = new DataTable();
                SqlDataAdapter adapter3 = new SqlDataAdapter();
                SqlCommand command3 = new SqlCommand("SELECT * FROM visits", db.getConnection());
                adapter3.SelectCommand = command3;
                adapter3.Fill(table3);


                DateTime timeopen = Convert.ToDateTime(table.Rows[0].ItemArray[2].ToString());
                DateTime timeclose = Convert.ToDateTime(table.Rows[0].ItemArray[3].ToString());
                DateTime thisDay = DateTime.Today;
                if ((time.Value.Hour < timeopen.Hour) && (time.Value.Hour < timeclose.Hour))
                {
                    time.Value = thisDay;
                    time.Value.AddDays(1);
                }
                foreach (DataRow row in table3.Rows)
                {
                    if (Convert.ToDateTime(row.ItemArray[4])==time.Value.Date)
                    {
                        var a = row.ItemArray[4];
                        TimeSpan b = (TimeSpan)row.ItemArray[5];
                        //Timespan
                        if (Convert.ToDateTime(a).Hour == time.Value.Hour && b.Hours == time.Value.Minute)
                        {
                            MessageBox.Show("На эту дату уже есть запись");
                            time.Value= thisDay;
                            time.Value.AddDays(1);
                            return;
                        }
                        int c = b.Hours;
                        if ( Math.Abs(c - time.Value.Hour) < 1 )
                        {
                            MessageBox.Show("Предыдущая запись еще идет");
                            time.Value = thisDay;
                            time.Value.AddDays(1);
                            return;
                        }
                    }
                }
                label10.Text = "";
            }
            else
            {
                label10.Text = "Выберите салон";
                label10.ForeColor = Color.Gray;
            }
        }

        private void time_ValueChanged(object sender, EventArgs e)
        {
            if (salons.Text == "")
            {
                label10.Text = "Выберите салон";
                label10.ForeColor = Color.Gray;
            }
        }

        private void Delete_Click(object sender, EventArgs e)
        {
            Db db = new Db();
            db.openConnection();
            DataTable table3 = new DataTable();
            SqlDataAdapter adapter3 = new SqlDataAdapter();
            SqlCommand command3 = new SqlCommand("SELECT * FROM visits WHERE id_user = @uI", db.getConnection());
            command3.Parameters.Add("@uI", SqlDbType.Int).Value = id_user;
            adapter3.SelectCommand = command3;
            adapter3.Fill(table3);
            if (table3.Rows.Count == 0)
            {
                MessageBox.Show("У вас нет записей");
                return;
            }

            DataTable table = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter();

            SqlCommand command = new SqlCommand("DELETE FROM visits WHERE visits.id = @uS", db.getConnection());
            
            command.Parameters.Add("@uS", SqlDbType.Int).Value = table3.Rows[(int)number.Value-1].ItemArray[0];
            adapter.SelectCommand = command;
            command.ExecuteNonQuery();

            table3.Clear();
            adapter3.Fill(table3);

            DataTable table4 = new DataTable();
            SqlDataAdapter adaptersalon1 = new SqlDataAdapter();
            SqlCommand commandsalon1 = new SqlCommand("SELECT * FROM salons WHERE id = @uSI", db.getConnection());

            SqlDataAdapter adapterservices = new SqlDataAdapter();
            SqlCommand commandservices = new SqlCommand("SELECT * FROM services WHERE id = @uS", db.getConnection());

            SqlDataAdapter adaptermaster = new SqlDataAdapter();
            SqlCommand commandmaster = new SqlCommand("SELECT * FROM master WHERE id = @uI", db.getConnection());
            int i = 1;
            datanear.Rows.Clear();
            foreach (DataRow item in table3.Rows)
            {
                int day, month, year;
                string services, master, salon;
                day = Convert.ToDateTime(item.ItemArray[4]).Day;
                month = Convert.ToDateTime(item.ItemArray[4]).Month;
                year = Convert.ToDateTime(item.ItemArray[4]).Year;
                string date = day + "." + month + "." + year;

                commandsalon1.Parameters.Add("@uSI", SqlDbType.Int).Value = Convert.ToInt32(item.ItemArray[7]);
                adaptersalon1.SelectCommand = commandsalon1;
                adaptersalon1.Fill(table4);
                salon = table4.Rows[0].ItemArray[1].ToString();
                commandsalon1.Parameters.Clear();
                adaptersalon1.DeleteCommand = commandsalon1;
                table4.Clear();

                commandservices.Parameters.Add("@uS", SqlDbType.Int).Value = Convert.ToInt32(item.ItemArray[2]);
                adapterservices.SelectCommand = commandservices;
                adapterservices.Fill(table4);
                services = table4.Rows[0].ItemArray[1].ToString();
                commandservices.Parameters.Clear();
                adapterservices.DeleteCommand = commandservices;
                table4.Clear();

                commandmaster.Parameters.Add("@uI", SqlDbType.Int).Value = Convert.ToInt32(item.ItemArray[3]);
                adaptermaster.SelectCommand = commandmaster;
                adaptermaster.Fill(table4);
                master = table4.Rows[0].ItemArray[1].ToString();
                commandmaster.Parameters.Clear();
                adaptermaster.DeleteCommand = commandmaster;
                table4.Clear();

                datanear.Rows.Add(i, date, item.ItemArray[5].ToString(), services, master, salon);
                i++;
            }
            db.closeConnection();
        }


        private void button2_Click(object sender, EventArgs e)
        {

            dataadmin.Rows.Clear();
            Db db = new Db();
            DataTable table3 = new DataTable();
            SqlDataAdapter adapter3 = new SqlDataAdapter();
            SqlCommand command3 = new SqlCommand("SELECT * FROM visits WHERE Услуга_оказана = 0", db.getConnection());
            adapter3.SelectCommand = command3;
            adapter3.Fill(table3);
            numberadmin.Maximum= table3.Rows.Count;

            if (table3.Rows.Count != 0)
            {
                numberadmin.Minimum = 1;
                numberadmin.Value = numberadmin.Minimum;
            }
            DataTable table4 = new DataTable();
            SqlDataAdapter adaptersalon1 = new SqlDataAdapter();
            SqlCommand commandsalon1 = new SqlCommand("SELECT * FROM salons WHERE id = @uSI", db.getConnection());

            SqlDataAdapter adapterservices = new SqlDataAdapter();
            SqlCommand commandservices = new SqlCommand("SELECT * FROM services WHERE id = @uS", db.getConnection());

            SqlDataAdapter adaptermaster = new SqlDataAdapter();
            SqlCommand commandmaster = new SqlCommand("SELECT * FROM master WHERE id = @uI", db.getConnection());

            SqlDataAdapter adapteruser = new SqlDataAdapter();
            SqlCommand commanduser = new SqlCommand("SELECT * FROM users WHERE id = @uU", db.getConnection());

            SqlDataAdapter adapterprice = new SqlDataAdapter();
            SqlCommand commandprice = new SqlCommand("SELECT * FROM visits WHERE id = @uV", db.getConnection());


            int i = 1;
            foreach (DataRow item in table3.Rows)
            {
                int day, month, year;
                string services, master, salon,user,price;
                day = Convert.ToDateTime(item.ItemArray[4]).Day;
                month = Convert.ToDateTime(item.ItemArray[4]).Month;
                year = Convert.ToDateTime(item.ItemArray[4]).Year;
                string date = day + "." + month + "." + year;

                commandsalon1.Parameters.Add("@uSI", SqlDbType.Int).Value = Convert.ToInt32(item.ItemArray[7]);
                adaptersalon1.SelectCommand = commandsalon1;
                adaptersalon1.Fill(table4);
                salon = table4.Rows[0].ItemArray[1].ToString();
                commandsalon1.Parameters.Clear();
                adaptersalon1.DeleteCommand = commandsalon1;
                table4.Clear();

                commandservices.Parameters.Add("@uS", SqlDbType.Int).Value = Convert.ToInt32(item.ItemArray[2]);
                adapterservices.SelectCommand = commandservices;
                adapterservices.Fill(table4);
                services = table4.Rows[0].ItemArray[1].ToString();
                commandservices.Parameters.Clear();
                adapterservices.DeleteCommand = commandservices;
                table4.Clear();

                commandmaster.Parameters.Add("@uI", SqlDbType.Int).Value = Convert.ToInt32(item.ItemArray[3]);
                adaptermaster.SelectCommand = commandmaster;
                adaptermaster.Fill(table4);
                master = table4.Rows[0].ItemArray[1].ToString();
                commandmaster.Parameters.Clear();
                adaptermaster.DeleteCommand = commandmaster;
                table4.Clear();

                commanduser.Parameters.Add("@uU", SqlDbType.Int).Value = Convert.ToInt32(item.ItemArray[1]);
                adapteruser.SelectCommand = commanduser;
                adapteruser.Fill(table4);
                user = table4.Rows[0].ItemArray[1].ToString();
                commanduser.Parameters.Clear();
                adapteruser.DeleteCommand = commanduser;
                table4.Clear();

                commandprice.Parameters.Add("@uV", SqlDbType.Int).Value = Convert.ToInt32(item.ItemArray[0]);
                adapterprice.SelectCommand = commandprice;
                adapterprice.Fill(table4);
                price = table4.Rows[0].ItemArray[5].ToString();
                commandprice.Parameters.Clear();
                adapterprice.DeleteCommand = commandprice;
                table4.Clear();

                dataadmin.Rows.Add(i,user, services, master, date, item.ItemArray[5].ToString(), salon,price);
                i++;
            }

        }

        private void Suc_Click(object sender, EventArgs e)
        {
            int id_user;
            double price_1;
            if (numberadmin.Value == 0)
            {
                MessageBox.Show("Такой записи нет");
                return;
            }
            dataadmin.Rows.Clear();
            Db db = new Db();
            DataTable table3 = new DataTable();
            SqlDataAdapter adapter3 = new SqlDataAdapter();
            SqlCommand command3 = new SqlCommand("SELECT * FROM visits WHERE Услуга_оказана = 0", db.getConnection());
            adapter3.SelectCommand = command3;
            adapter3.Fill(table3);
            if (table3.Rows.Count == 0)
            {
                MessageBox.Show("Нет активных записей");
                return;
            }


            DataTable table = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter();

            SqlCommand command = new SqlCommand("UPDATE visits SET Услуга_оказана = '1' WHERE visits.id = @uS;", db.getConnection());

            command.Parameters.Add("@uS", SqlDbType.Int).Value = table3.Rows[(int)numberadmin.Value - 1].ItemArray[0];
            price_1=Convert.ToInt32(table3.Rows[(int)numberadmin.Value - 1].ItemArray[8]);
            id_user = Convert.ToInt32(table3.Rows[(int)numberadmin.Value - 1].ItemArray[1]);
            adapter.SelectCommand = command;
            adapter.Fill(table);
            update_discount(id_user, price_1);

            table3.Clear();
            adapter3.Fill(table3);

            DataTable table4 = new DataTable();
            SqlDataAdapter adaptersalon1 = new SqlDataAdapter();
            SqlCommand commandsalon1 = new SqlCommand("SELECT * FROM salons WHERE id = @uSI", db.getConnection());

            SqlDataAdapter adapterservices = new SqlDataAdapter();
            SqlCommand commandservices = new SqlCommand("SELECT * FROM services WHERE id = @uS", db.getConnection());

            SqlDataAdapter adaptermaster = new SqlDataAdapter();
            SqlCommand commandmaster = new SqlCommand("SELECT * FROM master WHERE id = @uI", db.getConnection());

            SqlDataAdapter adapteruser = new SqlDataAdapter();
            SqlCommand commanduser = new SqlCommand("SELECT * FROM users WHERE id = @uU", db.getConnection());

            SqlDataAdapter adapterprice = new SqlDataAdapter();
            SqlCommand commandprice = new SqlCommand("SELECT * FROM visits WHERE id = @uV", db.getConnection());

            int i = 1;
            foreach (DataRow item in table3.Rows)
            {
                int day, month, year;
                string services, master, salon, user, price;
                day = Convert.ToDateTime(item.ItemArray[4]).Day;
                month = Convert.ToDateTime(item.ItemArray[4]).Month;
                year = Convert.ToDateTime(item.ItemArray[4]).Year;
                string date = day + "." + month + "." + year;

                commandsalon1.Parameters.Add("@uSI", SqlDbType.Int).Value = Convert.ToInt32(item.ItemArray[7]);
                adaptersalon1.SelectCommand = commandsalon1;
                adaptersalon1.Fill(table4);
                salon = table4.Rows[0].ItemArray[1].ToString();
                commandsalon1.Parameters.Clear();
                adaptersalon1.DeleteCommand = commandsalon1;
                table4.Clear();

                commandservices.Parameters.Add("@uS", SqlDbType.Int).Value = Convert.ToInt32(item.ItemArray[2]);
                adapterservices.SelectCommand = commandservices;
                adapterservices.Fill(table4);
                services = table4.Rows[0].ItemArray[1].ToString();
                commandservices.Parameters.Clear();
                adapterservices.DeleteCommand = commandservices;
                table4.Clear();

                commandmaster.Parameters.Add("@uI", SqlDbType.Int).Value = Convert.ToInt32(item.ItemArray[3]);
                adaptermaster.SelectCommand = commandmaster;
                adaptermaster.Fill(table4);
                master = table4.Rows[0].ItemArray[1].ToString();
                commandmaster.Parameters.Clear();
                adaptermaster.DeleteCommand = commandmaster;
                table4.Clear();

                commanduser.Parameters.Add("@uU", SqlDbType.Int).Value = Convert.ToInt32(item.ItemArray[1]);
                adapteruser.SelectCommand = commanduser;
                adapteruser.Fill(table4);
                user = table4.Rows[0].ItemArray[1].ToString();
                commanduser.Parameters.Clear();
                adapteruser.DeleteCommand = commanduser;
                table4.Clear();

                commandprice.Parameters.Add("@uV", SqlDbType.Int).Value = Convert.ToInt32(item.ItemArray[0]);
                adapterprice.SelectCommand = commandprice;
                adapterprice.Fill(table4);
                price = table4.Rows[0].ItemArray[5].ToString();
                commandprice.Parameters.Clear();
                adapterprice.DeleteCommand = commandprice;
                table4.Clear();

                dataadmin.Rows.Add(i, user, services, master, date, item.ItemArray[5].ToString(), salon, price);
                i++;

                numberadmin.Value = 1;
            }


        }

        private void services_Leave(object sender, EventArgs e)
        {
            if (services.Text != "")
            {
                Db db = new Db();
                DataTable table = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter();

                SqlCommand command = new SqlCommand("SELECT * FROM services WHERE name = @us", db.getConnection());
                command.Parameters.Add("@uS", SqlDbType.VarChar).Value = services.Text;
                adapter.SelectCommand = command;
                adapter.Fill(table);

                price=Convert.ToInt32(table.Rows[0].ItemArray[3]);
            }
        }

        private void servgroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (master.Items.Count != 0)
            {
                master.Items.Clear();
                master.Text = "";
            }
            Db db = new Db();
            SqlDataAdapter adapter2 = new SqlDataAdapter();
            DataTable table2 = new DataTable();
            SqlCommand command2 = new SqlCommand("SELECT * FROM servgroups WHERE Name= @un ", db.getConnection());
            command2.Parameters.Add("@un", SqlDbType.VarChar).Value = servgroup.Text;
            adapter2.SelectCommand = command2;


            adapter2.Fill(table2);
            int id_servicegroup =  Convert.ToInt32(table2.Rows[0].ItemArray[0]);

            services.Items.Clear();
            services.Text = "";
            if (servgroup.Text != "")
            {

                DataTable table = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter();

                SqlCommand command = new SqlCommand("SELECT * FROM services WHERE id_group = @ug ", db.getConnection());
                command.Parameters.Add("@ug", SqlDbType.Int).Value = id_servicegroup;
                adapter.SelectCommand = command;
                adapter.Fill(table);
                foreach (DataRow item in table.Rows)
                {
                    services.Items.Add(item.ItemArray[1].ToString());
                }
            }
        }


        private void emailgo_Click(object sender, EventArgs e)
        {
            if (tema.Text == "" || textemail.Text == "")
            {
                MessageBox.Show("Введите тест и тему письма");
                return;
            }
            Db db = new Db();
            DataTable table = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter();

            SqlCommand command = new SqlCommand("SELECT * FROM info_user WHERE id_user = @ug ", db.getConnection());
            command.Parameters.Add("@ug", SqlDbType.Int).Value = id_user;
            adapter.SelectCommand = command;
            adapter.Fill(table);
            string your_email = "your_email.com";
            string password = "";
            SmtpClient client = new SmtpClient(your_email);
            client.Port = 25;
            client.Credentials = new System.Net.NetworkCredential(your_email, password);
            client.EnableSsl = true;

            foreach (DataRow item in table.Rows)
            {
                string to = item.ItemArray[1].ToString();
                //MailMessage mail = new MailMessage(your_email, to, tema.Text, textemail.VarChar);

                MailMessage message = new MailMessage();
                message.To.Add(new MailAddress(to)); // кому отправлять  
                message.Subject = tema.Text; // тема письма  
                message.Body = textemail.Text;

                try
                {
                    client.Send(message);
                    MessageBox.Show("Сообщение успешно отправлено!", "Отправлено!", MessageBoxButtons.OK);
                }
                catch (Exception o)
                {
                    MessageBox.Show("Ошибка: " + o.Message);
                }
            }

        }

        private void change_Click(object sender, EventArgs e)
        {
            DateTime thisDay = DateTime.Today;
            Db db = new Db();
            db.openConnection();
            DataTable table3 = new DataTable();
            SqlDataAdapter adapter3 = new SqlDataAdapter();
            SqlCommand command3 = new SqlCommand("SELECT * FROM visits", db.getConnection());
            adapter3.SelectCommand = command3;
            adapter3.Fill(table3);
            if (dateup.Value < thisDay)
            {
                MessageBox.Show("Некорректная дата");
                return;
            }
            DataTable table4 = new DataTable();
            SqlDataAdapter adapter4 = new SqlDataAdapter();
            SqlCommand command4 = new SqlCommand("SELECT * FROM salons WHERE id = @uS ", db.getConnection());
            command4.Parameters.Add("@uS", SqlDbType.Int).Value = id_salons;
            adapter4.SelectCommand = command4;
            adapter4.Fill(table4);
            DateTime timeopen = Convert.ToDateTime(table4.Rows[0].ItemArray[2].ToString());
            DateTime timeclose = Convert.ToDateTime(table4.Rows[0].ItemArray[3].ToString());
            if (!((timeup.Value.Hour > timeopen.Hour) && (timeup.Value.Hour < timeclose.Hour)))
            {
                timeup.Value = thisDay;
                timeup.Value.AddDays(1);
                MessageBox.Show("Некорректная дата");
                return;
            }
            foreach (DataRow row in table3.Rows)
            {
                if (Convert.ToDateTime(row.ItemArray[4]) == dateup.Value.Date)
                {
                    var a = row.ItemArray[4];
                    TimeSpan b = (TimeSpan)row.ItemArray[5];
                    //Timespan
                    if (b.Hours == timeup.Value.Hour && ((Math.Abs(b.Minutes - timeup.Value.Minute)) <= 30))
                    {
                        if (Convert.ToInt32(row.ItemArray[3]) == id_persone)
                        {
                            MessageBox.Show("На эту дату уже есть запись");
                            timeup.Value = thisDay;
                            timeup.Value.AddDays(1);
                            return;
                        }
                    }
                    int c = b.Hours;
                    if (Math.Abs(c - timeup.Value.Hour) < 1)
                    {
                        MessageBox.Show("Предыдущая запись еще идет");
                        timeup.Value = thisDay;
                        timeup.Value.AddDays(1);
                        return;
                    }
                }
                datanear_update(id_user);
            }


            DataTable table = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter();
            SqlCommand command = new SqlCommand("UPDATE visits SET Дата=@date,Время=@time, Услуга_оказана=0 WHERE id = @iV", db.getConnection());
            command.Parameters.Add("@iV", SqlDbType.Int).Value = table3.Rows[(int)number.Value - 1].ItemArray[0];
            command.Parameters.Add("@date", SqlDbType.Date).Value = dateup.Value;
            command.Parameters.Add("@time", SqlDbType.DateTime).Value = timeup.Value;


            command.ExecuteNonQuery();
            MessageBox.Show("Запись перенесена");
            datanear_update(id_user);
            db.closeConnection();

        }
        Point lastPoint;
        private void main_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button==MouseButtons.Left)
            {
                this.Left += e.X - lastPoint.X;
                this.Top += e.Y - lastPoint.Y;

            }
        }

        private void main_MouseDown(object sender, MouseEventArgs e)
        {
            lastPoint = new Point(e.X, e.Y); 
        }

        private void servicecreate_Click(object sender, EventArgs e)
        {
            if(servicename.Text=="" || pricechange.Value == 0 || commentservice.Text=="" || groupchange.Text=="")
            {
                MessageBox.Show("Заполните все поля");
                return;
            }
            master.Text = "";
            services.Text = "";
            servgroup.Text = "";
            Db db = new Db();
            db.openConnection();
            DataTable table = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter();
            SqlCommand command2 = new SqlCommand("SELECT * FROM servgroups WHERE name = @ug ", db.getConnection());
            command2.Parameters.Add("@ug", SqlDbType.VarChar).Value = groupchange.Text;
            adapter.SelectCommand = command2;
            
            adapter.Fill(table);
            int id_group = Convert.ToInt16(table.Rows[0].ItemArray[0]);

            DataTable table2 = new DataTable();
            SqlDataAdapter adapter2 = new SqlDataAdapter();
            SqlCommand command3 = new SqlCommand("SELECT * FROM services", db.getConnection());
            adapter2.SelectCommand = command3;
            adapter2.Fill(table2);
            foreach (DataRow item in table2.Rows)
            {
                if(item.ItemArray[1].ToString() == servicename.Text)
                {
                    MessageBox.Show("Нельзя добавить услугу с таким именем");
                    return;
                }
            }

            SqlCommand command = new SqlCommand("INSERT INTO services (name, id_group, price, comment) VALUES (@name, @id_group, @price, @comment)", db.getConnection());
            command.Parameters.Add("@name", SqlDbType.VarChar).Value = servicename.Text;
            command.Parameters.Add("@id_group", SqlDbType.Int).Value = id_group;
            command.Parameters.Add("@price", SqlDbType.Float).Value = Convert.ToDouble(pricechange.Text);
            command.Parameters.Add("@comment", SqlDbType.VarChar).Value =commentservice.Text;

            command.ExecuteNonQuery();
            db.closeConnection();
            MessageBox.Show("Улуга добавлена");

        }

        private void servicedelete_Click(object sender, EventArgs e)
        {
            if (groupdelete.Text=="" || servicenamedelete.Text=="")
            {
                MessageBox.Show("Заполните все поля");
                return;
            }
            Db db = new Db();
            db.openConnection();

            DataTable table = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter();
            SqlCommand command2 = new SqlCommand("SELECT * FROM services WHERE name = @name", db.getConnection());
            command2.Parameters.Add("@name", SqlDbType.VarChar).Value = servicenamedelete.Text;
            adapter.SelectCommand = command2;
            adapter.Fill(table);
            var id_servicedelete = table.Rows[0].ItemArray[0];

            SqlCommand command = new SqlCommand("DELETE FROM services WHERE name = @name", db.getConnection());
            command.Parameters.Add("@name", SqlDbType.VarChar).Value = servicenamedelete.Text;

            DataTable table2 = new DataTable();
            SqlDataAdapter adapter2 = new SqlDataAdapter();
            SqlCommand command3 = new SqlCommand("SELECT * FROM visits WHERE id_service=@id", db.getConnection());
            command3.Parameters.Add("@id", SqlDbType.Int).Value = id_servicedelete;
            adapter2.SelectCommand = command3;
            adapter2.Fill(table2);
            if (table2.Rows.Count != 0)
            {
                MessageBox.Show("Вы не можете удалить эту услугу");
                return;
            }

            command.ExecuteNonQuery();

            db.closeConnection();
            groupdelete.Text = "";
            servicenamedelete.Text = "";
            servicenamedelete.Items.Clear();
            MessageBox.Show("Услуга удалена");       
        }



        private void groupdelete_SelectedIndexChanged(object sender, EventArgs e)
        {
            Db db = new Db();
            if (groupdelete.Text != "")
            {

                DataTable table = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter();

                SqlCommand command = new SqlCommand("SELECT * FROM services WHERE id_group = @ug ", db.getConnection());
                command.Parameters.Add("@ug", SqlDbType.Int).Value = id_servicegroup;
                adapter.SelectCommand = command;
                adapter.Fill(table);
                servicenamedelete.Items.Clear();
                servicenamedelete.Text = "";
                foreach (DataRow item in table.Rows)
                {
                    servicenamedelete.Items.Add(item.ItemArray[1].ToString());
                }
            }
        }

        private void servicenamedelete_SelectedIndexChanged(object sender, EventArgs e)
        {
            Db db = new Db();
            if (servgroup.Text != "")
            {

                DataTable table = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter();

                SqlCommand command = new SqlCommand("SELECT * FROM services WHERE id_group = @ug ", db.getConnection());
                command.Parameters.Add("@ug", SqlDbType.Int).Value = id_servicegroup;
                adapter.SelectCommand = command;
                adapter.Fill(table);
                foreach (DataRow item in table.Rows)
                {
                    services.Items.Add(item.ItemArray[1].ToString());
                }
            }
        }

        private void servicenamedelete_Enter(object sender, EventArgs e)
        {
            Db db = new Db();
            if (groupdelete.Text != "")
            {

                DataTable table = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter();

                SqlCommand command = new SqlCommand("SELECT * FROM services WHERE id_group = @ug ", db.getConnection());
                command.Parameters.Add("@ug", SqlDbType.Int).Value = id_servicegroup;
                adapter.SelectCommand = command;
                adapter.Fill(table);
                servicenamedelete.Items.Clear();
                servicenamedelete.Text = "";
                foreach (DataRow item in table.Rows)
                {
                    servicenamedelete.Items.Add(item.ItemArray[1].ToString());
                }
            }
        }

        private void servgroup_Enter(object sender, EventArgs e)
        {
            services.Items.Clear();
            services.Text = "";
            


        }

        private void master_SelectedIndexChanged(object sender, EventArgs e)
        {
            Db db = new Db();
            DataTable table = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter();

            SqlCommand command = new SqlCommand("SELECT * FROM services WHERE name = @ug ", db.getConnection());
            command.Parameters.Add("@ug", SqlDbType.VarChar).Value = services.Text;
            adapter.SelectCommand = command;
            adapter.Fill(table);

            id_service = Convert.ToInt32(table.Rows[0].ItemArray[0]);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (master.Items.Count != 0)
            {
                master.Items.Clear();
                master.Text = "";
                if (services.Items.Count != 0)
                    services.Items.Clear();
                services.Text = "";
            }
            
            Db db = new Db();
            DataTable table = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter();

            SqlCommand command = new SqlCommand("SELECT * FROM salons WHERE name = @uN", db.getConnection());
            command.Parameters.Add("@uN", SqlDbType.VarChar).Value = salons.Text;
            adapter.SelectCommand = command;
            adapter.Fill(table);
            id_salons = Convert.ToInt32(table.Rows[0].ItemArray[0]);
        }

        private void update_discount(int id,double price_1)
        {
            Db db = new Db();
            db.openConnection();
            DataTable table = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter();
            SqlCommand command = new SqlCommand("SELECT * FROM info_user WHERE id_user = @ug ", db.getConnection());
            command.Parameters.Add("@ug", SqlDbType.Int).Value = id;
            adapter.SelectCommand = command;
            adapter.Fill(table);
            int cow = Convert.ToInt32(table.Rows[0].ItemArray[5])+1;
            double chech = Convert.ToDouble(table.Rows[0].ItemArray[3]);
            double discount=0;
            if ((cow-1)==0)
            {
                chech = price_1;
            }
            double average_new = (chech * cow + price_1) / (cow + 1);
            switch (cow)
            {
                case 1:
                    discount = 1;
                    if (chech > 3000)
                    {
                        discount *= 1.5;
                    }
                    break;
                case 2:
                    discount = 2;
                    if (chech > 3000)
                    {
                        discount *= 1.5;
                    }
                    break;
                case 3:
                    discount = 3;
                    if (chech > 3000)
                    {
                        discount *= 1.5;
                    }
                    break;
                case 4:
                    discount = 4;
                    if (chech > 3000)
                    {
                        discount *= 1.5;
                    }
                    break;
                case 5:
                    discount = 5;
                    if (chech > 3000)
                    {
                        discount *= 1.5;
                    }
                    break;
                case 6:
                    discount = 6;
                    if (chech > 3000)
                    {
                        discount *= 1.5;
                    }
                    break;
                case 7:
                    discount = 7;
                    if (chech > 3000)
                    {
                        discount *= 1.5;
                    }
                    break;
                default:
                    break;
            }

            DataTable table_up = new DataTable();
            SqlCommand command_up = new SqlCommand("UPDATE info_user SET chech=@uC,allvisits=@uA,discount= @uD WHERE id_user = @uI", db.getConnection());
            command_up.Parameters.Add("@uI", SqlDbType.Int).Value = id;
            command_up.Parameters.Add("@uC", SqlDbType.Int).Value = average_new;
            command_up.Parameters.Add("@uA", SqlDbType.Int).Value = cow;
            command_up.Parameters.Add("@uD", SqlDbType.Int).Value = discount;
            command_up.ExecuteNonQuery();
            db.closeConnection();
        }
        private void datanear_update(int id_user)
        {
            datanear.Rows.Clear();
            Db db = new Db();
            DataTable table3 = new DataTable();
            SqlDataAdapter adapter3 = new SqlDataAdapter();
            SqlCommand command3 = new SqlCommand("SELECT * FROM visits WHERE id_user = @uI AND Услуга_оказана = 0", db.getConnection());
            command3.Parameters.Add("@uI", SqlDbType.Int).Value = id_user;
            adapter3.SelectCommand = command3;
            adapter3.Fill(table3);
            DataTable table4 = new DataTable();
            SqlDataAdapter adaptersalon1 = new SqlDataAdapter();
            SqlCommand commandsalon1 = new SqlCommand("SELECT * FROM salons WHERE id = @uSI", db.getConnection());

            SqlDataAdapter adapterservices = new SqlDataAdapter();
            SqlCommand commandservices = new SqlCommand("SELECT * FROM services WHERE id = @uS", db.getConnection());

            SqlDataAdapter adaptermaster = new SqlDataAdapter();
            SqlCommand commandmaster = new SqlCommand("SELECT * FROM master WHERE id = @uI", db.getConnection());
            
            int i = 1;
            foreach (DataRow item in table3.Rows)
            {
                int day, month, year;
                string services, master, salon;
                day = Convert.ToDateTime(item.ItemArray[4]).Day;
                month = Convert.ToDateTime(item.ItemArray[4]).Month;
                year = Convert.ToDateTime(item.ItemArray[4]).Year;
                string date = day + "." + month + "." + year;

                commandsalon1.Parameters.Add("@uSI", SqlDbType.Int).Value = Convert.ToInt32(item.ItemArray[7]);
                adaptersalon1.SelectCommand = commandsalon1;
                adaptersalon1.Fill(table4);
                salon = table4.Rows[0].ItemArray[1].ToString();
                commandsalon1.Parameters.Clear();
                adaptersalon1.DeleteCommand = commandsalon1;
                table4.Clear();

                commandservices.Parameters.Add("@uS", SqlDbType.Int).Value = Convert.ToInt32(item.ItemArray[2]);
                adapterservices.SelectCommand = commandservices;
                adapterservices.Fill(table4);
                services = table4.Rows[0].ItemArray[1].ToString();
                commandservices.Parameters.Clear();
                adapterservices.DeleteCommand = commandservices;
                table4.Clear();

                commandmaster.Parameters.Add("@uI", SqlDbType.Int).Value = Convert.ToInt32(item.ItemArray[3]);
                adaptermaster.SelectCommand = commandmaster;
                adaptermaster.Fill(table4);
                master = table4.Rows[0].ItemArray[1].ToString();
                commandmaster.Parameters.Clear();
                adaptermaster.DeleteCommand = commandmaster;
                table4.Clear();

                datanear.Rows.Add(i, date, item.ItemArray[5].ToString(), services, master, salon);
                i++;
            }
        }
    }
}
