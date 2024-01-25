using PersonelVardiyaOtomasyonu.Class.DAL;
using PersonelVardiyaOtomasyonu.Class.Helper;
using PersonelVardiyaOtomasyonu.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace PersonelVardiyaOtomasyonu
{
	public partial class Leaves : Form
	{
		LeavesDal _leaveDal = new LeavesDal();

		public Leaves()
		{
			InitializeComponent();
            List<User> userList = GetAll();
            cb_users.DisplayMember = "NameSurname";
            cb_users.ValueMember = "Id"; // veya istediğiniz başka bir özellik
            cb_users.DataSource = userList;

        }
        public List<User> GetAll()
        {
            ConnectionStrings.ConnectionControl();

            SqlCommand sqlCommand = new SqlCommand("Select * from Users", ConnectionStrings._sqlConnection);
            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

            List<User> users = new List<User>();

            while (sqlDataReader.Read())
            {
                User user = new User
                {
                    Id = Convert.ToInt32(sqlDataReader["Id"]),
                    RoleId = Convert.ToInt32(sqlDataReader["RoleId"]),
                    IdentificationNumber = sqlDataReader["IdentificationNumber"].ToString(),
                    Name = sqlDataReader["Name"].ToString(),
                    Surname = sqlDataReader["Surname"].ToString(),
                    Password = sqlDataReader["Password"].ToString(),
                    Address = sqlDataReader["Address"].ToString(),
                    EMail = sqlDataReader["E-Mail"].ToString(),
                    PhoneNumber = sqlDataReader["PhoneNumber"].ToString(),
                    leave_day_1 = sqlDataReader["Leave_day_1"].ToString(),
                    leave_day_2 = sqlDataReader["Leave_day_2"].ToString(),
                    Status = Convert.ToBoolean(sqlDataReader["Status"]),
                    NameSurname = sqlDataReader["Name"].ToString() + " " + sqlDataReader["Surname"].ToString()
                };

                users.Add(user);
            }

            sqlDataReader.Close();
            ConnectionStrings._sqlConnection.Close();

            return users;
        }

        private void Leaves_Load(object sender, EventArgs e)
		{
			GetList();
		}

		public void GetList()
		{
			dgv_leaves.Rows.Clear();

			var leaves = _leaveDal.GetAll();
			foreach (var leave in leaves)
			{
				dgv_leaves.Rows.Add(
					leave.id,
					leave.name,
					leave.surname,
					leave.sicil_no,
					leave.start_date.ToShortDateString(),
					leave.end_date.ToShortDateString()
				);
			}

			dgv_leaves.ClearSelection();
		}

        private void btn_add_Click(object sender, EventArgs e)
        {
            // ComboBox'ta seçili öğe var mı kontrolü
            if (cb_users.SelectedItem != null && cb_users.SelectedItem is User selectedUser)
            {
                Leave leave = new Leave
                {
                    name = selectedUser.Name,
                    surname = selectedUser.Surname,
                    sicil_no = selectedUser.Id, // Varsa IdentificationNumber kullanabilirsiniz.
                    start_date = Convert.ToDateTime(txt_start_date.Text),
                    end_date = Convert.ToDateTime(txt_end_date.Text)
                };

                _leaveDal.Add(leave);
                GetList();
                Clear();
            }
            else
            {
                MessageBox.Show("Lütfen bir kullanıcı seçin!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btn_delete_Click(object sender, EventArgs e)
		{
			if (dgv_leaves.SelectedRows.Count != 0)
			{
				if (MessageBox.Show("Kullanıcıyı silmek istediğinizden emin misiniz?", "Bilgi", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
				{
					_leaveDal.Delete(Convert.ToInt32(dgv_leaves.CurrentRow.Cells[0].Value));
					GetList();
					Clear();
				}
			}
			else
			{
				MessageBox.Show("Silinecek kullanıcıyı seçiniz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}

        private void dgv_leaves_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                object startDateCellValue = dgv_leaves.Rows[e.RowIndex].Cells[4].Value;
                object endDateCellValue = dgv_leaves.Rows[e.RowIndex].Cells[5].Value;

                if (startDateCellValue != null && endDateCellValue != null)
                {
                    txt_start_date.Text = startDateCellValue.ToString();
                    txt_end_date.Text = endDateCellValue.ToString();
                }
                else
                {
                    // İlgili hücrelerde değer olmadığında yapılacak işlem veya mesajı buraya ekleyebilirsiniz.
                    MessageBox.Show("Seçili hücrelerde değer bulunmamaktadır.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }


        public void Clear()
		{
            txt_start_date.Value = DateTime.Now;
			txt_end_date.Value = DateTime.Now;
		}

        private void tableLayoutPanel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tableLayoutPanel5_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
