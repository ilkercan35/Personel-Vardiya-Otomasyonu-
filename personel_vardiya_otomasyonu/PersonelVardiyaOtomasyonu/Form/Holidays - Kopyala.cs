using PersonelVardiyaOtomasyonu.Class.DAL;
using PersonelVardiyaOtomasyonu.Entities;
using System;
using System.Windows.Forms;

namespace PersonelVardiyaOtomasyonu
{
	public partial class Holidays : Form
	{
		HolidayDal _holidayDal = new HolidayDal();

		public Holidays()
		{
			InitializeComponent();
		}

		private void Holidays_Load(object sender, EventArgs e)
		{
			GetList();
		}

		public void GetList()
		{
			dgv_holidays.Rows.Clear();

			var holidays = _holidayDal.GetAll();
			foreach (var holiday in holidays)
			{
				dgv_holidays.Rows.Add(
					holiday.Id,
					holiday.Name,
					holiday.Date.ToShortDateString()
				);
			}

			dgv_holidays.ClearSelection();
		}

		private void btn_add_Click(object sender, EventArgs e)
		{
			if (!string.IsNullOrWhiteSpace(txt_name.Text) && !string.IsNullOrWhiteSpace(txt_date.Text))
			{
				Holiday holiday = new Holiday
				{
					Name = txt_name.Text,
					Date = Convert.ToDateTime(txt_date.Text)
				};

				_holidayDal.Add(holiday);
				GetList();
				Clear();
			}
			else
			{
				MessageBox.Show("Gerekli alanları doldurunuz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}

		private void btn_update_Click(object sender, EventArgs e)
		{
			if (dgv_holidays.SelectedRows.Count != 0)
			{
				if (!string.IsNullOrWhiteSpace(txt_name.Text) && !string.IsNullOrWhiteSpace(txt_date.Text))
				{
					Holiday holiday = new Holiday
					{
						Id = Convert.ToInt32(dgv_holidays.CurrentRow.Cells[0].Value),
						Name = txt_name.Text,
						Date = Convert.ToDateTime(txt_date.Text)
					};

					_holidayDal.Update(holiday);
					GetList();
					Clear();
				}
				else
				{
					MessageBox.Show("Gerekli alanları doldurunuz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				}
			}
			else
			{
				MessageBox.Show("Güncellenecek kullanıcıyı seçiniz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}

		private void btn_delete_Click(object sender, EventArgs e)
		{
			if (dgv_holidays.SelectedRows.Count != 0)
			{
				if (MessageBox.Show("Kullanıcıyı silmek istediğinizden emin misiniz?", "Bilgi", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
				{
					_holidayDal.Delete(Convert.ToInt32(dgv_holidays.CurrentRow.Cells[0].Value));
					GetList();
					Clear();
				}
			}
			else
			{
				MessageBox.Show("Silinecek kullanıcıyı seçiniz!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}

		private void dgv_holidays_CellClick(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex >= 0)
			{
				txt_name.Text = dgv_holidays.Rows[e.RowIndex].Cells[1].Value.ToString();
				txt_date.Text = dgv_holidays.Rows[e.RowIndex].Cells[2].Value.ToString();
			}
		}

		public void Clear()
		{
			txt_name.Clear();
			txt_date.Value = DateTime.Now;
		}
	}
}
