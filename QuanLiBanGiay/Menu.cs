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
            int indexIDTheloai = cb_TheLoai.SelectedIndex + 1;
            if (cb_TheLoai.SelectedIndex < 0)
            {
                MessageBox.Show("Vui lòng chọn thể loại", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Stop);

            }
            else
            {
                if (txbTen.Text == "" && txbSoLuongTonKho.Text == "" && txbGia.Text == "" )
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
                else
                {
                    string sql = "INSERT INTO PRODUCT (PRODUCTNAME,Description,SIZE,PRICE,StockQuantity,CategoryID) VALUES (N'" + txbTen.Text + "',N'" + txbDesc.Text + "','" + cbSize.Text + "','" + txbGia.Text + "','" + txbSoLuongTonKho.Text + "','" + indexIDTheloai + "')";

                    Query(sql, "Thêm thành công giày");
                }
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
            string sqlTableXuatGiay = "SELECT Product.PRODUCTID,PRODUCTNAME,StockQuantity,PRICE FROM PRODUCT where isdelete = 0";
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
                                    od.Quantity,
                                    od.TotalAmount,
                                    pay.PaymentMethod,
                                    pay.PaymentDate
                                FROM Customer cust
                                JOIN OrderDetail od ON cust.CustomerID = od.CustomerID
                                JOIN Product p ON od.ProductID = p.ProductID
                                JOIN Category c ON p.CategoryID = c.CategoryID
                                JOIN Payment pay ON od.OrderDetailID = pay.OrderDetailID";

            FillDataTable(sqlTableDetail,dgvSort);

            //bảng thanh toán
            string sqlPayment = @"SELECT 
                                od.OrderDetailID,
                                c.Fullname,
                                p.ProductName,
                                od.Quantity,
                                od.TotalAmount,
                                c.PhoneNumber,
                                od.OrderDate
                            FROM OrderDetail od
                            JOIN Customer c ON od.CustomerID = c.CustomerID
                            JOIN Product p ON od.ProductID = p.ProductID
                            WHERE od.StatusPayment = 0;";
            FillDataTable(sqlPayment, dgvThanhToan);
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
            string sqlQuery = "INSERT INTO CATEGORY (CATEGORYNAME) VALUES (N'"+ txbLoaiGiay.Text + "')";
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
                cbSuaTheLoai.SelectedIndex = int.Parse(dt.Rows[row][6].ToString());

              
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
            cb_TheLoai.Items.Clear();

            try
            {
                string sqlQuery = "UPDATE category SET CategoryName =N'" + txbLoaiGiay.Text + "' where categoryID ='" + lbID.Text + "'";
           
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
            int customID = cbKH.SelectedIndex + 1 ;

            if (cbKH.SelectedIndex < 0) 
            {
                MessageBox.Show("Vui lòng chọn khách hàng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                cbKH.Focus();
            }
            else
            {
                if (txbXGSanPham.Text == "" && txbXGSoLuong.Text == "" && txbXGSoTien.Text == "" && cbKH.Text =="")
                {
                    MessageBox.Show("Lui lòng nhập thông tin mua hàng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    
                }
                else
                {

                    txbXGSoTien.Text = (int.Parse(lbXGSoTien.Text) * int.Parse(txbXGSoLuong.Text)).ToString();

                    //tạo đơn hàng
                    string sqlCreateOrder = "INSERT INTO OrderDetail (CustomerID, ProductID, Quantity, OrderDate, UnitPrice, TotalAmount) " +
                        "VALUES ('" + customID + "','"+int.Parse(lbXGID.Text)+"','"+int.Parse(txbXGSoLuong.Text)+"','" 
                        + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','"+int.Parse(lbXGSoTien.Text)+"','" + int.Parse(txbXGSoTien.Text) + "')";

                    Query(sqlCreateOrder, "Tạo đơn thành công");

                    //giảm số lượng hàng tồn khi đặt hàng thành công
                    UpdateStockQuantity(int.Parse(lbXGID.Text), int.Parse(txbXGSoLuong.Text));
                    Reload();
                }
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
                string sql = "SELECT PRODUCTID,PRODUCTNAME,StockQuantity,PRICE FROM PRODUCT where isdelete = 0";

                da = new SqlDataAdapter(sql, cnn);
                dt = new DataTable();
                da.Fill(dt);

                int row = e.RowIndex;
                lbXGID.Text = dt.Rows[row][0].ToString();
                txbXGSanPham.Text = dt.Rows[row][1].ToString();
                lbXGSoTien.Text = dt.Rows[row][3].ToString();
                lbXGTonKho.Text = dt.Rows[row][2].ToString();




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
            (dgvSort.DataSource as DataTable).DefaultView.RowFilter = string.Format("SoftCategory LIKE '%{0}%'", cbLocTheLoai.Text);
        }

        private void dgvLoaiGiay_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                cnn = new SqlConnection(connetionString);
                cnn.Open();

                string sql = "SELECT CATEGORYID,CategoryName from category";



                da = new SqlDataAdapter(sql, cnn);
                dt = new DataTable();
                da.Fill(dt);


                int row = e.RowIndex;
                lbIDLoaiGiay.Text = dt.Rows[row][0].ToString();
                txbLoaiGiay.Text = dt.Rows[row][1].ToString();


                cnn.Close();

            }
            catch (Exception es)
            {
                MessageBox.Show(es.ToString());

            }
           
            
        }

        private void txbXGKhachHang_TextChanged(object sender, EventArgs e)
        {
            btnTaoKH.Text = "Tạo KH: "+ txbXGKhachHang.Text;
        }

        private void btnTaoKH_Click(object sender, EventArgs e)
        {
           
            if (txbXGKhachHang.Text == "" && txbXGSDT.Text == "" )
            {
                MessageBox.Show("Vui lòng nhập thông tin khách hàng đầy đủ", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            else
            {
                //tạo khách hàng
                string sqlQuery = "INSERT INTO customer (Fullname,Email,PhoneNumber,Address) VALUES " +
               "(N'" + txbXGKhachHang.Text + "','" + txbXGEmail.Text + "','" + txbXGSDT.Text + "',N'" + txbXGAddress.Text + "')";

                Query(sqlQuery, "Tạo khách hàng " + txbXGKhachHang.Text + " thành công");

            }
        }

        private void txbXGSoLuong_TextChanged(object sender, EventArgs e)
        {
            if (int.Parse(txbXGSoLuong.Text) > int.Parse(lbXGTonKho.Text))
            {
                MessageBox.Show("Số lượng "+ txbXGSanPham.Text+" chỉ có ["+lbXGTonKho.Text+"] sản phẩm\nBạn đặt vượt quá số lượng còn trong kho", "Chú ý", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txbXGSoLuong.Text = "1";
            }
            else
            {

                txbXGSoTien.Text = (int.Parse(lbXGSoTien.Text) * int.Parse(txbXGSoLuong.Text)).ToString();
            }

        }

        private void txbXGSoLuong_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(e.KeyChar >= '0' && e.KeyChar <= '9' || e.KeyChar == (char)8))
                e.Handled = true;
        }

        private void dgvThanhToan_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                cnn = new SqlConnection(connetionString);
                cnn.Open();

                string sql = @"SELECT 
                                od.OrderDetailID,
                                c.Fullname,
                                p.ProductName,
                                od.Quantity,
                                od.TotalAmount,
                                c.PhoneNumber,
                                od.OrderDate
                            FROM OrderDetail od
                            JOIN Customer c ON od.CustomerID = c.CustomerID
                            JOIN Product p ON od.ProductID = p.ProductID
                            WHERE od.StatusPayment = 0;";


                da = new SqlDataAdapter(sql, cnn);
                dt = new DataTable();
                da.Fill(dt);


                int row = e.RowIndex;
                lbTTID.Text = dt.Rows[row][0].ToString();
                txbTTName.Text = dt.Rows[row][1].ToString();
                txbTTSP.Text = dt.Rows[row][2].ToString();


                cnn.Close();

            }
            catch (Exception es)
            {
                MessageBox.Show(es.ToString());

            }

        }

        private void btnThanhToan_Click(object sender, EventArgs e)
        {

            string sqlQuery = "insert into payment (OrderDetailID, PaymentMethod, PaymentDate) VALUES ('"+ lbTTID.Text + "',N'"+
                cbPaymentMethod.Text+"','"+ DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "')";
 

            Query(sqlQuery,"");

            string sql = "UPDATE OrderDetail SET StatusPayment = 1 WHERE OrderDetailID ='" + lbTTID.Text + "'";
            Query(sql, "Thanh toán thành công KH: " + txbTTName.Text);

        }

        private void cbSoftSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            (dgvSort.DataSource as DataTable).DefaultView.RowFilter = string.Format("SoftSize LIKE '%{0}%'", cbLocTheLoai.Text);

        }
    }
}
