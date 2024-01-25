using PersonelVardiyaOtomasyonu.Class.DAL;
using PersonelVardiyaOtomasyonu.Class.Helper;
using PersonelVardiyaOtomasyonu.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PersonelVardiyaOtomasyonu
{
    public partial class otomasyon : Form
    {
        public otomasyon()
        {
            InitializeComponent();
        }

        private void otomasyon_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            DateTime baslangıc = DateTime.Parse(dateTimePicker2.Text);
            DateTime bitir = DateTime.Parse(dateTimePicker1.Text);

            int gunfarki = (int)(bitir - baslangıc).TotalDays;

            List<string> kampüs1 = new List<string>();
            kampüs1.Add("00:00-08:00");
            kampüs1.Add("08:00-16:00");
            kampüs1.Add("16:00-24:00");

            List<string> kampüs2 = new List<string>();
            kampüs2.Add("08:00-16:00");
            kampüs2.Add("09:00-17:00");

            List<string> kampüs = new List<string>();
            kampüs.Add("Kampüs Girişi");
            kampüs.Add("Kampüs Dışı");

            for (int i = 0; i <= gunfarki + 1; i++)
            {

                DateTime Days = baslangıc.AddDays(i);
                var k1 = 1;
                var k2 = 0;

                var vardiyaA = 0;
                var vardiyaB = 0;
                var vardiyaC = 0;
                var vardiyaD = 0;
                var vardiyaE = 0;

                List<int> users = GetAllUserIds();

                foreach (double Sicil in users)
                {
                    int Verikampüs = 0;
                    String saat = "0";
                    if (IzinSorgulama(Sicil, Days)) 
                    {
                        if (HaftalıkİzinSorgulama(Sicil, Days)) {
                            if (ÖzelİzinGünü(Days))
                            {
                                if (k1 < k2)
                                {
                                    int enKucukVardiye = Math.Min(vardiyaA, Math.Min(vardiyaB, vardiyaC));
                                    if (enKucukVardiye == vardiyaA)
                                    {
                                        vardiyaA++;
                                        saat = kampüs1[0];
                                    }
                                    else if (enKucukVardiye == vardiyaB)
                                    {
                                        vardiyaB++;
                                        saat = kampüs1[1];
                                    }
                                    else if (enKucukVardiye == vardiyaC)
                                    {
                                        vardiyaC++;
                                        saat = kampüs1[2];
                                        k1++;
                                    }
                                    vardiyaEkleme(Sicil, Days, saat, "Kampüs Girişi");
                                    continue;
                                }
                                else
                                {
                                    Verikampüs = 1;
                                    if (vardiyaD < vardiyaB)
                                    {
                                        vardiyaD++;
                                        saat = kampüs2[0];
                                    }
                                    else
                                    {
                                        vardiyaE++;
                                        saat = kampüs2[1];
                                        k2++;
                                    }
                                    vardiyaEkleme(Sicil, Days, saat, "Kampüs Dışı");
                                    continue;
                                }
                            }
                            if (Verikampüs == 0)
                            {
                            if (vardiyaD < vardiyaE)
                            {
                                vardiyaD++;
                                saat = kampüs2[0];
                            }
                            else
                            {
                                vardiyaE++;
                                saat = kampüs2[1];
                                k2++;
                                Verikampüs = 1;
                            }
                            vardiyaEkleme(Sicil, Days, saat, "Kampüs Dışı");
                            continue;
                            }
                        }
                    }
                }
                Main main = (Main)Application.OpenForms["Main"];
                main.GetList();
            }

        }


        public List<int> GetAllUserIds()
        {
            List<int> userIds = new List<int>();

            try
            {
                ConnectionStrings.ConnectionControl();

                using (SqlCommand sqlCommand = new SqlCommand("SELECT Id FROM users", ConnectionStrings._sqlConnection))
                {
                    using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
                    {
                        while (sqlDataReader.Read())
                        {
                            int userId = sqlDataReader.GetInt32(0);
                            userIds.Add(userId);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Hata yönetimi burada yapılabilir.
                // Örneğin, hata durumunda bir log oluşturabilir veya uygun bir şekilde işleyebilirsiniz.
            }
            finally
            {
                // Bağlantıyı kapatma işlemini finally bloğunda yapmak önemlidir.
                ConnectionStrings._sqlConnection.Close();
            }

            return userIds;
        }






        public static bool HaftalıkİzinSorgulama(double Sicil, DateTime Days)
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


        private static bool ÖzelİzinGünü(DateTime Days)
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
            return holidayCount == 0;
        }


        public static bool IzinSorgulama(double Sicil, DateTime Days)
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
                        MessageBox.Show("Sicil " + Sicil + " Days " + Days);
                        return false;
                    }
                }
            }

            // İzin sorgulama işlemi başarılıysa true döndür
            return true;
        }






        public static void vardiyaEkleme(double Sicil, DateTime Days, String saat, String location)
        {

            ConnectionStrings.ConnectionControl();

            SqlCommand sqlCommand = new SqlCommand("Insert into Shifts values(@registrantId,@employeeId,@dateOfRegistration,@date,@location,@hours,@isNew)", ConnectionStrings._sqlConnection);
            sqlCommand.Parameters.AddWithValue("@registrantId", UserInformation.User.Id);
            sqlCommand.Parameters.AddWithValue("@employeeId", Sicil);
            sqlCommand.Parameters.AddWithValue("@dateOfRegistration", Days);
            sqlCommand.Parameters.AddWithValue("@date", Days);
            sqlCommand.Parameters.AddWithValue("@location", location);
            sqlCommand.Parameters.AddWithValue("@hours", saat);
            sqlCommand.Parameters.AddWithValue("@isNew", false); // true veya false gibi bir boolean değer
            sqlCommand.ExecuteNonQuery();

            ConnectionStrings._sqlConnection.Close();

        }




























        private void baslangıc_ValueChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Main MainCheck = (Main)Application.OpenForms["Main"];

            if (MainCheck != null)
            {
                MainCheck.Show();
            }
            else
            {
                Main main = new Main();
                main.Show();
            }

            this.Close();
        }
    }
}
