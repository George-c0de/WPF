using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace регистрация
{
    public partial class регистрация : Form
    {
        public регистрация()
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

        private void buttonRegister_Click(object sender, EventArgs e)
        {
            Db db = new Db();

        }
    }
}
