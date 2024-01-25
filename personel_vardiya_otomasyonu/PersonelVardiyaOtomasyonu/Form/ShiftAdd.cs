using PersonelVardiyaOtomasyonu.Class.DAL;
using PersonelVardiyaOtomasyonu.Class.Helper;
using PersonelVardiyaOtomasyonu.Entities;
using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Windows.Forms;

namespace PersonelVardiyaOtomasyonu
{
    public partial class ShiftAdd : Form
    {
        public ShiftAdd()
        {
            InitializeComponent();
        }

        ShiftDal _shiftDal = new ShiftDal();
        UserDal _userDal = new UserDal();

        private void ShiftAdd_Load(object sender, EventArgs e)
        {
            GetUserList();
        }

        public void GetUserList()
        {
            cb_users.DisplayMember = "NameSurname";
            cb_users.ValueMember = "Id";
            cb_users.DataSource = _userDal.GetAll();
        }

        private void cb_locations_TextChanged(object sender, EventArgs e)
        {            
            cb_hours.Items.Clear();

            switch (cb_locations.Text)
            {
                case "Kampüs Girisi":
                    cb_hours.Items.Add("00:00 - 08:00");
                    cb_hours.Items.Add("08:00 - 16:00");
                    cb_hours.Items.Add("16:00 - 24:00");
                    break;

                case "Kampüs Içi":
                    cb_hours.Items.Add("08:00 - 16:00");
                    cb_hours.Items.Add("09:00 - 17:00");
                    break;

                default:
                    cb_hours.Items.Add("Saat bulunmamaktadır!");
                    break;
            }
        }

        private void btn_continue_Click(object sender, EventArgs e)
        {
            if (cb_locations.Text != "" && cb_hours.Text != "")
            {
                Shift shift = new Shift
                {
                    RegistrantId = UserInformation.User.Id,
                    EmployeeId = Convert.ToInt32(cb_users.SelectedValue),
                    DateOfRegistration = Convert.ToDateTime(DateTime.Now.ToShortDateString()),
                    Date = Convert.ToDateTime(dtp_date.Value.ToShortDateString()),
                    Location = cb_locations.Text,
                    Hours = cb_hours.Text,
                    IsNew = true,
                };

                if (vardiyaKontrol(shift.EmployeeId, shift.Date)) {
                    MessageBox.Show("İşçi o gün zaten çalışıyor!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (ÖzelİzinGünü(shift.Date, shift.EmployeeId))
                {
                    DialogResult result = MessageBox.Show("Resmi Tatil Günü. Yine de yeni vardiya eklemek istiyor musunuz?", "Uyarı",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (result == DialogResult.No)
                    {
                        return;
                    }
                }

                if (!HaftalıkİzinSorgulama(shift.EmployeeId, shift.Date))
                {
                    DialogResult result2 = MessageBox.Show("Haftalık izin günü. Yine de yeni vardiya eklemek istiyor musunuz?", "Uyarı",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (result2 == DialogResult.No)
                    {
                        return;
                    }
                }

                if (!IzinSorgulama(shift.EmployeeId, shift.Date))
                {
                    DialogResult result3 = MessageBox.Show("Kullanıcı o gün izinli. Yine de yeni vardiya eklemek istiyor musunuz?", "Uyarı",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (result3 == DialogResult.No)
                    {
                        return;
                    }
                }

                _shiftDal.DeactivateAllNewShiftsTheEmployee(shift.EmployeeId);
                _shiftDal.Add(shift);

                Main main = (Main)Application.OpenForms["Main"];
                main?.GetList();

                this.Close();
                              
            }
            else
            {
                MessageBox.Show("Gerekli alanları doldurunuz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }






        private static bool vardiyaKontrol(double Sicil, DateTime Days)
        {
            int Count = 0;

            ConnectionStrings.ConnectionControl();

            using (SqlCommand sqlCommand = new SqlCommand("SELECT COUNT(*) FROM Shifts WHERE Date = @Tarih And EmployeeId = @Sicil", ConnectionStrings._sqlConnection))
            {
                sqlCommand.Parameters.AddWithValue("@Tarih", Days); // Corrected parameter name
                sqlCommand.Parameters.AddWithValue("@Sicil", Sicil);

                using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                {
                    while (sqlDataReader.Read())
                    {
                        // Increment the Count variable
                        Count = sqlDataReader.GetInt32(0);
                    }
                }
            }

            ConnectionStrings._sqlConnection.Close();
            // If Count is 0, return true; otherwise, return false
            return Count != 0;
        }
        private static bool IzinSorgulama(double Sicil, DateTime Days)
        {
            DateTime? baslangic = null;
            DateTime? bitis = null;

            ConnectionStrings.ConnectionControl();

            using (SqlCommand sqlCommand = new SqlCommand("SELECT start_date, end_date FROM Leaves WHERE sicil_no=@Id", ConnectionStrings._sqlConnection))
            {
                sqlCommand.Parameters.AddWithValue("@Id", Sicil);

                using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                {
                    while (sqlDataReader.Read())
                    {
                        // Veritabanından alınan değerleri DateTime türüne dönüştür
                        if (DateTime.TryParse(sqlDataReader["start_date"].ToString(), out DateTime startDate))
                        {
                            baslangic = startDate;
                        }

                        if (DateTime.TryParse(sqlDataReader["end_date"].ToString(), out DateTime endDate))
                        {
                            bitis = endDate;
                        }
                    }
                }
            }

            ConnectionStrings._sqlConnection.Close();

            if (baslangic.HasValue && bitis.HasValue)
            {
                TimeSpan gunFarki = bitis.Value - baslangic.Value;
                int gunSayisi = gunFarki.Days;

                for (int i = 0; i <= gunSayisi; i++)
                {
                    DateTime kontrolTarihi = baslangic.Value.AddDays(i);

                    if (kontrolTarihi.Date == Days.Date)
                    {
                        // Eğer Days, izin tarihleri arasında ise izin verme (false döndürme)
                        return false;
                    }
                }
            }

            // İzin sorgulama işlemi başarılıysa true döndür
            return true;
        }
        private static bool ÖzelİzinGünü(DateTime Days, int Sicil)
        {
            int holidayCount = 0;

            ConnectionStrings.ConnectionControl();

            using (SqlCommand sqlCommand = new SqlCommand("SELECT COUNT(*) FROM Holidays WHERE Date = @Tarih", ConnectionStrings._sqlConnection))
            {
                sqlCommand.Parameters.AddWithValue("@Tarih", Days); // @Tarih parametresini düzelt

                holidayCount = (int)sqlCommand.ExecuteScalar();
            }
            ConnectionStrings._sqlConnection.Close();

            // Eğer tatil günü varsa (holidayCount > 0), özel izin günüdür.
            return holidayCount != 0;
        }
        private static bool HaftalıkİzinSorgulama(double Sicil, DateTime Days)
        {
            string bugununGunu = Days.ToString("dddd", new CultureInfo("tr-TR"));
            string leave_day_1 = "";
            string leave_day_2 = "";

            ConnectionStrings.ConnectionControl();

            using (SqlCommand sqlCommand = new SqlCommand("SELECT Leave_day_1, Leave_day_2 FROM Users WHERE Id=@Id", ConnectionStrings._sqlConnection))
            {
                sqlCommand.Parameters.AddWithValue("@Id", Sicil);

                using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                {
                    while (sqlDataReader.Read())
                    {
                        leave_day_1 = sqlDataReader["Leave_day_1"].ToString();
                        leave_day_2 = sqlDataReader["Leave_day_2"].ToString();
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
            return !(leave_day_1.Equals(bugununGunu) || leave_day_2.Equals(bugununGunu));
        }























    }


}








