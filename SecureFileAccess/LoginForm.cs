using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Windows.Forms;

namespace SecureFileAccess
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            RegisterForm regForm = new RegisterForm();
            regForm.ShowDialog();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtUserID.Text.Trim(), out int userID))
            {
                MessageBox.Show("Please enter a valid numeric User ID.");
                return;
            }

            string password = txtPassword.Text;

            if (DBHelper.ValidateUser(userID, password))
            {
                string name = DBHelper.GetUserName(userID);
                string department = DBHelper.GetUserDepartment(userID);

                FileAccessForm accessForm = new FileAccessForm(userID, name, department);
                accessForm.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Invalid ID or Password.");
            }
        }
    }
}
