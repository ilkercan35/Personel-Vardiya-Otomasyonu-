using PersonelVardiyaOtomasyonu.Class.Helper;
using PersonelVardiyaOtomasyonu.Entities;

using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace PersonelVardiyaOtomasyonu.Class.DAL
{
	public class HolidayDal
	{
		public List<Holiday> GetAll()
		{
			ConnectionStrings.ConnectionControl();

			SqlCommand sqlCommand = new SqlCommand("SELECT * FROM Holidays", ConnectionStrings._sqlConnection);
			SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

			List<Holiday> holidays = new List<Holiday>();

			while (sqlDataReader.Read())
			{
				Holiday holiday = new Holiday
				{
					Id = Convert.ToInt32(sqlDataReader["Id"]),
					Name = sqlDataReader["Name"].ToString(),
					Date = Convert.ToDateTime(sqlDataReader["Date"])
				};

				holidays.Add(holiday);
			}

			sqlDataReader.Close();
			ConnectionStrings._sqlConnection.Close();

			return holidays;
		}

		public Holiday GetById(int id)
		{
			ConnectionStrings.ConnectionControl();

			SqlCommand sqlCommand = new SqlCommand("SELECT * FROM Holidays WHERE Id=@id", ConnectionStrings._sqlConnection);
			sqlCommand.Parameters.AddWithValue("@id", id);

			SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

			while (sqlDataReader.Read())
			{
				Holiday holiday = new Holiday
				{
					Id = Convert.ToInt32(sqlDataReader["Id"]),
					Name = sqlDataReader["Name"].ToString(),
					Date = Convert.ToDateTime(sqlDataReader["Date"])
				};

				sqlDataReader.Close();
				ConnectionStrings._sqlConnection.Close();

				return holiday;
			}

			sqlDataReader.Close();
			ConnectionStrings._sqlConnection.Close();

			return null;
		}

		public void Add(Holiday holiday)
		{
			ConnectionStrings.ConnectionControl();

			SqlCommand sqlCommand = new SqlCommand("INSERT INTO Holidays (Name, Date) VALUES (@name, @date)", ConnectionStrings._sqlConnection);

			sqlCommand.Parameters.AddWithValue("@name", holiday.Name);
			sqlCommand.Parameters.AddWithValue("@date", holiday.Date);

			sqlCommand.ExecuteNonQuery();

			ConnectionStrings._sqlConnection.Close();
		}

		public void Update(Holiday holiday)
		{
			ConnectionStrings.ConnectionControl();

			SqlCommand sqlCommand = new SqlCommand("UPDATE Holidays SET Name=@name, Date=@date WHERE Id=@id", ConnectionStrings._sqlConnection);

			sqlCommand.Parameters.AddWithValue("@id", holiday.Id);
			sqlCommand.Parameters.AddWithValue("@name", holiday.Name);
			sqlCommand.Parameters.AddWithValue("@date", holiday.Date);

			sqlCommand.ExecuteNonQuery();

			ConnectionStrings._sqlConnection.Close();
		}

		public void Delete(int id)
		{
			ConnectionStrings.ConnectionControl();

			SqlCommand sqlCommand = new SqlCommand("DELETE FROM Holidays WHERE Id=@id", ConnectionStrings._sqlConnection);
			sqlCommand.Parameters.AddWithValue("@id", id);

			sqlCommand.ExecuteNonQuery();

			ConnectionStrings._sqlConnection.Close();
		}
	}
}
