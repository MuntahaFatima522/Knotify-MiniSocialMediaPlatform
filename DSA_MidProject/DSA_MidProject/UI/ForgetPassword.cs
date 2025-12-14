using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DSA_MidProject.DL;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace DSA_MidProject
{
    public partial class ForgetPassword : Form
    {
        public ForgetPassword()
        {
            InitializeComponent();
            this.ActiveControl = panel1;

        }
        private void SetPlaceholder(System.Windows.Forms.TextBox txt, string placeholder, bool isPassword = false)
        {
            txt.Tag = placeholder;
            txt.Text = placeholder;
            txt.ForeColor = Color.Gray;

            if (isPassword)
                txt.UseSystemPasswordChar = false;

            txt.Enter += (s, e) =>
            {
                if (txt.Text == placeholder)
                {
                    txt.Text = "";
                    txt.ForeColor = Color.Black;

                    if (isPassword)
                        txt.UseSystemPasswordChar = true;
                }
            };

            txt.Leave += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txt.Text))
                {
                    txt.Text = placeholder;
                    txt.ForeColor = Color.Gray;

                    if (isPassword)
                        txt.UseSystemPasswordChar = false;
                }
            };
        }

        private void ForgetPassword_Load(object sender, EventArgs e)
        {
            SetPlaceholder(textBox1, "Enter your email");
            SetPlaceholder(textBox2, "Enter new password", true);
            SetPlaceholder(textBox3, "Confirm your password", true);

        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Login other = new Login();
            NavigationManager.NavigateTo(other, this);

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (textBox2.Text != textBox2.Tag.ToString())
            {
                textBox2.UseSystemPasswordChar = !checkBox1.Checked;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (textBox3.Text != textBox3.Tag.ToString())
            {
                textBox3.UseSystemPasswordChar = !checkBox2.Checked;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string email = textBox1.Text.Trim();
            string newPassword = textBox2.Text;
            string confirmPassword = textBox3.Text;


            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                MessageBox.Show("All fields are required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (newPassword != confirmPassword)
            {
                MessageBox.Show("Passwords do not match.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!UserCRUD.CheckPasswordStrength(newPassword))
            {
                MessageBox.Show("Password must contain at least one digit, one alphabet, and one special character.", "Weak Password", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool success = Program.AppData.userCrud.ResetPassword(email, newPassword);

            if (success)
            {
                MessageBox.Show("Password has been reset successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearFields();
            }
            else
            {
                MessageBox.Show("Invalid email.", "Failure", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ClearFields()
        {
            textBox1.Text = (string)textBox1.Tag;
            textBox1.ForeColor = Color.Gray;

            textBox2.Text = (string)textBox2.Tag;
            textBox2.ForeColor = Color.Gray;
            textBox2.UseSystemPasswordChar = false;

            textBox3.Text = (string)textBox3.Tag;
            textBox3.ForeColor = Color.Gray;

           

            checkBox1.Checked = false;

            this.ActiveControl = panel1;
        }
    }
}
