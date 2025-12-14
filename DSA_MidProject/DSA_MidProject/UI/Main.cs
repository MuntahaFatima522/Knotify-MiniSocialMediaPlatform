using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Mysqlx.Crud;

namespace DSA_MidProject
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
            timer1.Tick += Timer1_Tick;
            timer1.Start(); 
        }
        private void Timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();

            Program.AppData.userCrud.LoadFromDB();
            Program.AppData.friendCrud.LoadFromDB();
            Program.AppData.postCrud.LoadPostsFromDB();
            Program.AppData.commentCRUD.LoadFromDB();
            Program.AppData.likeCrud.LoadFromDB();

            Login other = new Login();
            NavigationManager.NavigateTo(other, this);


        }
    }
}
