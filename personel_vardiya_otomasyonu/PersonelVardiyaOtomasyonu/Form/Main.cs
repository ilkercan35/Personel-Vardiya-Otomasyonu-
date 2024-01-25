using PersonelVardiyaOtomasyonu.Class.DAL;
using PersonelVardiyaOtomasyonu.Class.Helper;
using PersonelVardiyaOtomasyonu.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Windows.Forms;

namespace PersonelVardiyaOtomasyonu
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }
        private bool isFormLoaded = false;

        ShiftDal _shiftDal = new ShiftDal();
        UserDal _userDal = new UserDal();

        private void Main_Load(object sender, EventArgs e)
        {
            isFormLoaded = true;
            CheckUserRole();
            GetList();
        }

        public void GetList()
        {
            dgv_shifts.Rows.Clear();

            List<Shift> shifts;
            if (UserInformation.Role.SeeAllShifts)
            {
                 shifts = _shiftDal.GetAllWithDate(dtp_startDate.Value, dtp_endDate.Value);
            }
            else
            {
                 shifts = _shiftDal.GetAllWithDateAndEmployee(dtp_startDate.Value, dtp_endDate.Value, UserInformation.User.Id);
            }

            foreach (Shift shift in shifts)
            {
                var registrantUser = _userDal.GetById(shift.RegistrantId);
                var user = _userDal.GetById(shift.EmployeeId);

                string durum = shift.IsNew ? "Aktif Vardiya" : "Pasif Vardiya";

                dgv_shifts.Rows.Add(shift.Id, shift.RegistrantId, registrantUser.NameSurname, shift.EmployeeId, user.NameSurname,
                    shift.DateOfRegistration.ToShortDateString(), shift.Date.ToShortDateString(), shift.Location, shift.Hours, shift.IsNew,
                    durum, lessonName(shift.Date),shiftUsers(shift.Date));
            }

            dgv_shifts.ClearSelection();
        }


        public static string shiftUsers(DateTime Days)
        {
            string bugununGunu = Days.ToString("dddd", new CultureInfo("tr-TR"));
            string name = "";
            string surname = "";

            ConnectionStrings.ConnectionControl();

            using (SqlCommand sqlCommand = new SqlCommand("SELECT Name, Surname FROM Users WHERE leave_day_1=@day or leave_day_2=@day",
                ConnectionStrings._sqlConnection))
            {
                sqlCommand.Parameters.AddWithValue("@day", bugununGunu);

                using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                {
                    while (sqlDataReader.Read())
                    {
                        name = sqlDataReader["Name"].ToString();
                        surname = sqlDataReader["Surname"].ToString();
                    }
                }
            }

            ConnectionStrings._sqlConnection.Close();

            // Veritabanındaki sütunlar ne tür ise ona göre işlem yapın
            // Örneğin, eğer veritabanında nvarchar sütunlarsa, tip dönüşümüne gerek yoktur.
            // Eğer int sütunlarsa, int.TryParse() gibi bir yöntem kullanmalısınız.

            // Örneğin, int sütunlar olarak düşünüyorsanız:
            // int leaveDay1, leaveDay2;
            // int.TryParse(leave_day_1, out leaveDay1);
            // int.TryParse(leave_day_2, out leaveDay2);
            return name + " " + surname;
        }

        public static string lessonName(DateTime tarih) {
            string bugununGunu = tarih.ToString("dddd", new CultureInfo("tr-TR"));
            return bugununGunu;
        }

        private void dtp_endDate_ValueChanged(object sender, EventArgs e)
        {
            GetList();
        }

        public void CheckUserRole()
        {
            int collapse = flp_buttons.Controls.Count;

            lbl_nameSurname.Text = UserInformation.User.Name + " " + UserInformation.User.Surname.ToUpper();
            lbl_roleName.Text = "(" + UserInformation.Role.Name + ")";

            if (!UserInformation.Role.Add)
            {
                btn_add.Visible = false;
                collapse--;
            }

            if (!UserInformation.Role.Update)
            {
                btn_update.Visible = false;
                collapse--;
            }

            if (!UserInformation.Role.Delete)
            {
                btn_delete.Visible = false;
                collapse--;
            }

            if (!UserInformation.Role.Print)
            {
                btn_print.Visible = false;
                collapse--;
            }

            if (!UserInformation.Role.UsersManagement)
            {
                btn_users.Visible = false;
                collapse--;
            }

            if (!UserInformation.Role.RolesManagement)
            {
                btn_roles.Visible = false;
                collapse--;
            }

            if (collapse == 0)
            {
                splitContainer1.Panel1Collapsed = true;
            }
        }

        private void btn_logout_Click(object sender, EventArgs e)
        {
            Login loginCheck = (Login)Application.OpenForms["Login"];

            if (loginCheck != null)
            {
                loginCheck.Show();
            }
            else
            {
                Login login = new Login();
                login.Show();
            }

            this.Close();
        }

        private void Main_SizeChanged(object sender, EventArgs e)
        {
            // Sadece form ilk kez yüklendiğinde SplitterDistance'ı sabitle
            if (isFormLoaded)
            {
                // SplitterDistance sınırları kontrol et
                int minSize = splitContainer1.Panel1MinSize;
                int maxSize = this.Width - splitContainer1.Panel2MinSize;

                // Geçerli SplitterDistance değerini al
                int currentDistance = splitContainer1.SplitterDistance;

                // Değer sınırlar içinde değilse, düzelt
                if (currentDistance < minSize)
                {
                    splitContainer1.SplitterDistance = minSize;
                }
                else if (currentDistance > maxSize)
                {
                    splitContainer1.SplitterDistance = maxSize;
                }
            }
        }


        #region LeftMenu
        private void btn_roles_Click(object sender, EventArgs e)
        {
            Roles roles = new Roles();
            roles.ShowDialog();
        }

        private void btn_add_Click(object sender, EventArgs e)
        {
            ShiftAdd shiftAdd = new ShiftAdd();
            shiftAdd.ShowDialog();
        }

        private void btn_update_Click(object sender, EventArgs e)
        {
            if (dgv_shifts.SelectedRows.Count != 0)
            {
                ShiftUpdate shiftUpdate = new ShiftUpdate(Convert.ToInt32(dgv_shifts.CurrentRow.Cells[0].Value));
                shiftUpdate.ShowDialog();
            }
            else
            {
                MessageBox.Show("Güncellenecek vardiyayı seçiniz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btn_users_Click(object sender, EventArgs e)
        {
            Users users = new Users();
            users.ShowDialog();
        }

        private void btn_delete_Click(object sender, EventArgs e)
        {
            if (dgv_shifts.SelectedRows.Count != 0)
            {
                if (MessageBox.Show("Vardiyayı silmek istediğinizden emin misiniz?", "Bilgi", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    _shiftDal.Delete(Convert.ToInt32(dgv_shifts.CurrentRow.Cells[0].Value));
                    GetList();
                    dgv_shifts.ClearSelection();
                }
            }
            else
            {
                MessageBox.Show("Silinecek vardiyayı seçiniz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        #endregion

        private void btn_print_Click(object sender, EventArgs e)
        {
            DataTable dataTable = new DataTable();

            // Add necessary columns
            dataTable.Columns.Add("Personel");
            dataTable.Columns.Add("Vardiya Tarihi");
            dataTable.Columns.Add("Lokasyon");
            dataTable.Columns.Add("Saatler");
            dataTable.Columns.Add("Gün Adı");
            dataTable.Columns.Add("Izinli Personel");

            for (int i = 0; i < dgv_shifts.Rows.Count; i++)
            {
                DataRow dataRow = dataTable.NewRow();

                // Populate necessary columns
                dataRow["Personel"] = dgv_shifts.Rows[i].Cells[4].Value;
                dataRow["Vardiya Tarihi"] = dgv_shifts.Rows[i].Cells[6].Value;
                dataRow["Lokasyon"] = dgv_shifts.Rows[i].Cells[7].Value;
                dataRow["Saatler"] = dgv_shifts.Rows[i].Cells[8].Value;
                dataRow["Gün Adı"] = dgv_shifts.Rows[i].Cells[11].Value; 
                dataRow["Izinli Personel"] = dgv_shifts.Rows[i].Cells[12].Value;

                dataTable.Rows.Add(dataRow);
            }

            PrintPDFBuilder.Print(dataTable, dtp_startDate.Value, dtp_endDate.Value);
        }


        private void button1_Click(object sender, EventArgs e)
        {
            otomasyon oto = new otomasyon();
            oto.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Vardiyayı silmek istediğinizden emin misiniz?", "Bilgi", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
            {
                ConnectionStrings.ConnectionControl();


                ConnectionStrings.ConnectionControl();

                using (SqlCommand sqlCommand = new SqlCommand("DELETE FROM Shifts", ConnectionStrings._sqlConnection))
                {
                    sqlCommand.ExecuteNonQuery();
                }
                ConnectionStrings._sqlConnection.Close();

                Main main = (Main)Application.OpenForms["Main"];
                main.GetList();
            }
          
        }

		private void button3_Click(object sender, EventArgs e)
		{
			Holidays holidays = new Holidays();
			holidays.ShowDialog();
		}

		private void button4_Click(object sender, EventArgs e)
		{
			Leaves leaves = new Leaves();
			leaves.ShowDialog();
		}
	}
}
