using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLiBanGiay
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (txbUser.Text == "admin" && txbPass.Text == "123123")
            {
                Menu ql = new Menu();
                ql.Show();
            } else
            {
                MessageBox.Show("Tài khoản hoặc mật khẩu bạn nhập sai\nVui lòng thử lại","Chú ý");
            }
        }
    }
}
