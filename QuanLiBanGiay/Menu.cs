using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace QuanLiBanGiay
{
    public partial class Menu : Form
    {
        string connetionString = @"Data Source=DESKTOP-8T8L9ET;Initial Catalog=QLBanGiay;Trusted_Connection=true;";
        SqlConnection cnn;
        SqlDataAdapter da;
        SqlCommand cmd;
        DataTable dt;
        DataSet ds;

        public Menu()
        {
            InitializeComponent();
        }

        private void Menu_Load(object sender, EventArgs e)
        {
            
            try
            {
                Reload();
                cb_TheLoai.SelectedIndex = 0;
                cbPaymentMethod.SelectedIndex = 0;

            }

            catch (Exception es)
            {
                MessageBox.Show(es.Message);
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void btnAddSP_Click(object sender, EventArgs e)
        {
            int indexIDTheloai = cb_TheLoai.SelectedIndex;
            if (txbTen.Text == "" && txbSoLuongTonKho.Text == "" && txbGia.Text == "" && txbDesc.Text == "" && cbSize.Text == "")
            {
                MessageBox.Show("Vui lòng nhập đầy đủ", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            else
            {
                string sql = "INSERT INTO PRODUCT (PRODUCTNAME,Description,SIZE,PRICE,StockQuantity,CategoryID) VALUES (N'" + txbTen.Text + "',N'" + txbDesc.Text + "','" + cbSize.Text + "','" + txbGia.Text + "','" +txbSoLuongTonKho.Text+ "','"+ indexIDTheloai + "')";

                Query(sql, "Thêm thành công giày");
             }
           
        }

        private void Reload()
        {
            //bảng trang chủ
            string sqlTableHome = "SELECT PRODUCTID,PRODUCTNAME,Description,SIZE,PRICE,StockQuantity,CategoryID FROM PRODUCT where isdelete = 0";
            FillDataTable(sqlTableHome, dgvData2);


            // hiển thị data cho combobox
            try
            {
                cnn = new SqlConnection(connetionString);
                cnn.Open();

                string sql = "SELECT CATEGORYID,CATEGORYNAME FROM CATEGORY";
               
                SqlCommand sqlCmd = new SqlCommand(sql, cnn);
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();

                

                while (sqlReader.Read())
                {
                    cb_TheLoai.Items.Remove(sqlReader["CATEGORYNAME"].ToString());
                    cbSuaTheLoai.Items.Remove(sqlReader["CATEGORYNAME"].ToString());
                    cb_TheLoai.Items.Add(sqlReader["CATEGORYNAME"].ToString());
                    cbSuaTheLoai.Items.Add(sqlReader["CATEGORYNAME"].ToString());

                    cbLocTheLoai.Items.Remove(sqlReader["CATEGORYNAME"].ToString());
                    cbLocTheLoai.Items.Add(sqlReader["CATEGORYNAME"].ToString());

                }
                cnn.Close();
            }
            catch (Exception es)
            {
                MessageBox.Show(es.Message);
            }

            // hiển thị data tên khách hàng cho combobox 
            try
            {
                cnn = new SqlConnection(connetionString);
                cnn.Open();

                string sql = "SELECT fullname FROM customer";

                SqlCommand sqlCmd = new SqlCommand(sql, cnn);
                SqlDataReader sqlReader = sqlCmd.ExecuteReader();



                while (sqlReader.Read())
                {
                    cbKH.Items.Remove(sqlReader["fullname"].ToString());
                    cbKH.Items.Add(sqlReader["fullname"].ToString());

                }
                cnn.Close();
            }
            catch (Exception es)
            {
                MessageBox.Show(es.Message);
            }


            //bảng xuất giày
            string sqlTableXuatGiay = "SELECT Product.PRODUCTID,PRODUCTNAME,o.Quantity,PRICE FROM PRODUCT JOIN OrderDetail o ON Product.PRODUCTID = o.ProductID where isdelete = 0";
            FillDataTable(sqlTableXuatGiay, dgvXuatGiay);

            //bảng khôi phục
            string sqlTableKhoiPhuc = "SELECT PRODUCTID,PRODUCTNAME,Description,SIZE,PRICE FROM PRODUCT where isdelete = 1";
            FillDataTable(sqlTableKhoiPhuc, dgvKhoiPhuc);

            //bảng loại giày
            string sqlTableCategory = "SELECT CATEGORYID,CategoryName from category";
            FillDataTable(sqlTableCategory, dgvLoaiGiay);

            //bảng khách hàng
            string sqlTableCustomer = "SELECT CustomerID,Fullname,Email,PhoneNumber,Address from customer";
            FillDataTable(sqlTableCustomer, dgvKH);


            //bảng chi tiết
            string sqlTableDetail = @"
                                 SELECT 
                                     cust.CustomerID,
                                     cust.Fullname,
                                     cust.Email,
                                     cust.PhoneNumber,
                                     cust.Address,
                                     p.ProductName,
                                     p.Description,
                                     p.Size,
                                     p.Price,
                                     c.CategoryName,
                                     o.OrderDate,
                                     o.TotalAmount,
                                     od.Quantity,
                                     pay.PaymentMethod,
                                     pay.PaymentDate
                                 FROM Customer cust
                                 JOIN Orderr o ON cust.CustomerID = o.CustomerID
                                 JOIN OrderDetail od ON o.OrderID = od.OrderID
                                 JOIN Product p ON od.ProductID = p.ProductID
                                 JOIN Category c ON p.CategoryID = c.CategoryID
                                 JOIN Payment pay ON o.OrderID = pay.OrderID";

            FillDataTable(sqlTableDetail,dgvSort);
        }
        private void FillDataTable(string query, DataGridView table)
        {
            try
            {
                cnn = new SqlConnection(connetionString);
                cnn.Open();

                da = new SqlDataAdapter(query, cnn);
                dt = new DataTable();
                da.Fill(dt);

                table.DataSource = dt;
                cnn.Close();
            }
            catch (Exception es)
            {
                MessageBox.Show(es.Message);
            }
        }


        private void Query(string query,string notify)
        {
            try
            {
                cnn = new SqlConnection(connetionString);
                cnn.Open();


                cmd = new SqlCommand(query, cnn);

                cmd.ExecuteNonQuery();
                
                if (notify !="") MessageBox.Show(notify);
                Reload();
                cnn.Close();
            }
            catch (Exception es)
            {
               
                if (es.Message.Contains("UQ__Customer__85FB4E3807220615"))
                {
                    MessageBox.Show("Số điện thoại đã có trong bảng, vui lòng nhập lại");
                }
                else
                {
                    MessageBox.Show(es.Message);
                }
            }
        }

        private void txbGia_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(e.KeyChar >= '0' && e.KeyChar <= '9' || e.KeyChar == (char)8))
                e.Handled = true;
        }

        private void txbSoLuong_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(e.KeyChar >= '0' && e.KeyChar <= '9' || e.KeyChar == (char)8))
                e.Handled = true;
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            Reload();
        }

        private void dgvData2_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {

                DialogResult res = MessageBox.Show("Bạn có chắc muốn xóa?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (res == DialogResult.Yes)
                {
                    Query("UPDATE Product SET isDelete = 1 WHERE ProductID ='" + lbID.Text + "'", "");
                    
                }
            }
            catch (Exception es)
            {
                MessageBox.Show(es.Message);

            }

        }

        private void btnThemLoaiGiay_Click(object sender, EventArgs e)
        {
            string sqlQuery = "INSERT INTO CATEGORY (CATEGORYNAME) VALUES ('"+ txbLoaiGiay.Text + "')";
            Query(sqlQuery, "Thêm loại giày: "+ txbLoaiGiay.Text + " thành công");
        }


        private void dgvData2_RowEnter(object sender, DataGridViewCellEventArgs e)
        {

            try
            {
                cnn = new SqlConnection(connetionString);
                cnn.Open();

                string sql = "SELECT PRODUCTID,PRODUCTNAME,Description,SIZE,PRICE,StockQuantity,CategoryID FROM PRODUCT where isdelete = 0";
                da = new SqlDataAdapter(sql, cnn);
                dt = new DataTable();
                da.Fill(dt);


                int row = e.RowIndex;
                lbID.Text = dt.Rows[row][0].ToString();
                txbSuaTen.Text = dt.Rows[row][1].ToString();
                txbSuaMoTa.Text = dt.Rows[row][2].ToString();
                cb_SuaSize.Text = dt.Rows[row][3].ToString();
                txbSuaGia.Text = dt.Rows[row][4].ToString();
                txbSuaSL.Text = dt.Rows[row][5].ToString();
                cbSuaTheLoai.SelectedIndex = int.Parse(dt.Rows[row][6].ToString()) - 1;

              
                cnn.Close();

            }
            catch (Exception es)
            {

            }
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            try
            {
                    string sqlQuery = "UPDATE PRODUCT SET " +
                "PRODUCTNAME = N'" + txbSuaTen.Text + "', " +
                "Description = N'" + txbSuaMoTa.Text + "', " +
                "SIZE = '" + cb_SuaSize.Text + "', " +
                "PRICE = '" + int.Parse(txbSuaGia.Text) + "', " +
                "StockQuantity = '" + int.Parse(txbSuaSL.Text) + "', " +
                "CategoryID = '" + cbSuaTheLoai.SelectedIndex + "' " +
                "WHERE PRODUCTID = '" + lbID.Text + "'";

                Query(sqlQuery, "Sửa thành công sản phẩm: " + txbSuaTen.Text);
            }
            catch (Exception es)
            {

                MessageBox.Show(es.Message);
            }


        }

        private void dgvKhoiPhuc_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                cnn = new SqlConnection(connetionString);
                cnn.Open();

                string sql = "SELECT PRODUCTID,PRODUCTNAME,Description,SIZE,PRICE FROM PRODUCT where isdelete = 1";
                da = new SqlDataAdapter(sql, cnn);
                dt = new DataTable();
                da.Fill(dt);


                int row = e.RowIndex;
                lbIDRestore.Text = dt.Rows[row][0].ToString();
                txbTenSanPham.Text = dt.Rows[row][1].ToString();
                btnKhoiPhuc.Text = "Khôi phục sản phẩm: " + dt.Rows[row][1].ToString();
               


                cnn.Close();

            }
            catch (Exception es)
            {

                MessageBox.Show(es.Message);
            }
        }

        private void btnKhoiPhuc_Click(object sender, EventArgs e)
        {
            try
            {

                DialogResult res = MessageBox.Show("Bạn có chắc muốn khôi phục", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (res == DialogResult.Yes)
                {
                    Query("UPDATE Product SET isDelete = 0 WHERE ProductID ='" + lbIDRestore.Text + "'", "");
                    txbTenSanPham.Text = "";
                }
            }
            catch (Exception es)
            {
                MessageBox.Show(es.Message);

            }
        }

        private void btnSuaLoaiGiay_Click(object sender, EventArgs e)
        {
            try
            {
                string sqlQuery = "UPDATE category SET CategoryName ='" + txbLoaiGiay.Text + "' where categoryID ='" + lbID.Text + "'";
           
                Query(sqlQuery, "Sửa thành công loại giày: " + txbLoaiGiay.Text);
            }
            catch (Exception es)
            {

                MessageBox.Show(es.Message);
            }
        }

        private void txbSDTKH_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(e.KeyChar >= '0' && e.KeyChar <= '9' || e.KeyChar == (char)8))
                e.Handled = true;
        }

        private void groupBox5_Enter(object sender, EventArgs e)
        {

        }

        private void btnXGBill_Click(object sender, EventArgs e)
        {
            if (txbXGSanPham.Text == "" && txbXGSoLuong.Text == "" && txbXGSoTien.Text == "" && cbKH.Text =="")
            {
                MessageBox.Show("Lui lòng nhập thông tin mua hàng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            else
            {
                //tạo khách hàng
                string sqlQuery = "INSERT INTO customer (Fullname,Email,PhoneNumber,Address) VALUES " +
               "(N'" + txbXGKhachHang.Text + "','" + txbXGEmail.Text + "','" + txbXGSDT.Text + "',N'" + txbXGAddress.Text + "')";

                Query(sqlQuery,"");


                //tạo đơn hàng
                string sqlCreateOrder = "INSERT INTO Order (CustomerID, OrderDate, TotalAmount) " +
                    "VALUES (N'" + txbTen.Text + "',N'" + txbDesc.Text + "','" + cbSize.Text + "','" + txbGia.Text + "'" +
                    ",'" + txbSoLuongTonKho.Text + "')";

                Query(sqlCreateOrder, "Thêm thành công giày");


                string sql = "INSERT INTO OrderDetail (OrderID,ProductID,Quantity,UnitPrice) " +
                    "VALUES (N'" + txbTen.Text + "',N'" + txbDesc.Text + "','" + cbSize.Text + "','" + txbGia.Text + "'" +
                    ",'" + txbSoLuongTonKho.Text + "')";

                Query(sql, "Thêm thành công giày");

                //giảm số lượng hàng tồn khi đặt hàng thành công
                UpdateStockQuantity(int.Parse(lbXGID.Text), int.Parse(txbXGSoLuong.Text)); 
            }
        }

        public void UpdateStockQuantity(int productId, int quantity)
        {

            using (cnn = new SqlConnection(connetionString))
            {
                cnn.Open();

                // Update StockQuantity in Product table
                string updateQuery = @"
            UPDATE Product
            SET StockQuantity = StockQuantity - @Quantity
            WHERE ProductID = @ProductID";

                using (SqlCommand cmd = new SqlCommand(updateQuery, cnn))
                {
                    cmd.Parameters.AddWithValue("@ProductID", productId);
                    cmd.Parameters.AddWithValue("@Quantity", quantity);

                    cmd.ExecuteNonQuery();
                }

                cnn.Close();
            }
        }

        private void txbXGSanPham_TextChanged(object sender, EventArgs e)
        {
            (dgvXuatGiay.DataSource as DataTable).DefaultView.RowFilter = string.Format("PRODUCTNAME LIKE '%{0}%'", txbXGTimKiem.Text);
        }

        private void dgvXuatGiay_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                cnn = new SqlConnection(connetionString);
                cnn.Open();
                string sql = "SELECT Product.PRODUCTID,PRODUCTNAME,o.Quantity,PRICE FROM PRODUCT JOIN OrderDetail o ON Product.PRODUCTID = o.ProductID where isdelete = 0";

                da = new SqlDataAdapter(sql, cnn);
                dt = new DataTable();
                da.Fill(dt);

                int row = e.RowIndex;
                lbXGID.Text = dt.Rows[row][0].ToString();
                txbXGSanPham.Text = dt.Rows[row][1].ToString();
                txbXGSoTien.Text = dt.Rows[row][3].ToString();




                cnn.Close();

            }
            catch (Exception es)
            {

            }
        }

        private void dgvKH_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                cnn = new SqlConnection(connetionString);
                cnn.Open();

                string sql = "SELECT CustomerID,Fullname,Email,PhoneNumber,Address from customer";
                da = new SqlDataAdapter(sql, cnn);
                dt = new DataTable();
                da.Fill(dt);


                int row = e.RowIndex;
                lbKHID.Text = dt.Rows[row][0].ToString();
                txbKHName.Text = dt.Rows[row][1].ToString();
                txbKHEmail.Text = dt.Rows[row][2].ToString();
                txbKHSDT.Text = dt.Rows[row][3].ToString();
                txbKHAddress.Text = dt.Rows[row][4].ToString();
               


                cnn.Close();

            }
            catch (Exception es)
            {

            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txbKHName.Text = "";
            txbKHEmail.Text = "";
            txbKHAddress.Text = "";
            txbKHSDT.Text = "";

        }

        private void txbKHSDT_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(e.KeyChar >= '0' && e.KeyChar <= '9' || e.KeyChar == (char)8))
                e.Handled = true;
        }

        private void btnKHSua_Click(object sender, EventArgs e)
        {
            string sqlQuery = "UPDATE customer SET " +
              "Fullname = N'" + txbKHName.Text + "', " +
              "Email = N'" + txbKHEmail.Text + "', " +
              "PhoneNumber = '" + txbKHSDT.Text + "', " +
              "Address = N'" + txbKHAddress.Text + "' " +
              "WHERE CustomerID ='" + lbKHID.Text + "'";

            Query(sqlQuery, "Sửa thành công khách hàng: " + txbKHName.Text);
        }

        private void cbLocTheLoai_SelectedIndexChanged(object sender, EventArgs e)
        {
            (dgvXuatGiay.DataSource as DataTable).DefaultView.RowFilter = string.Format("CategoryName LIKE '%{0}%'", cbLocTheLoai.Text);
        }

        private void dgvLoaiGiay_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            /*string sqlTableCategory = "SELECT CATEGORYID,CategoryName from category";
            FillDataTable(sqlTableCategory, dgvLoaiGiay);
            int row = e.RowIndex;
            lbIDLoaiGiay.Text = dt.Rows[row][0].ToString();
            txbLoaiGiay.Text = dt.Rows[row][1].ToString();*/
        }

        private void txbXGKhachHang_TextChanged(object sender, EventArgs e)
        {
            btnTaoKH.Text = "Tạo KH: "+ txbXGKhachHang.Text;
        }
    }
}
