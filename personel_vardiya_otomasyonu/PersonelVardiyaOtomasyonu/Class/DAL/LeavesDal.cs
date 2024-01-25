using PersonelVardiyaOtomasyonu.Class.Helper;
using PersonelVardiyaOtomasyonu.Entities;

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace PersonelVardiyaOtomasyonu.Class.DAL
{
	public class LeavesDal
	{
		public List<Leave> GetAll()
		{
			ConnectionStrings.ConnectionControl();

			SqlCommand sqlCommand = new SqlCommand("SELECT * FROM Leaves", ConnectionStrings._sqlConnection);
			SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

			List<Leave> leavesList = new List<Leave>();

			while (sqlDataReader.Read())
			{
				Leave leaves = new Leave
				{
					id = Convert.ToInt32(sqlDataReader["id"]),
					name = sqlDataReader["name"].ToString(),
					surname = sqlDataReader["surname"].ToString(),
					sicil_no = (int) sqlDataReader["sicil_no"],
					start_date = Convert.ToDateTime(sqlDataReader["start_date"]),
					end_date = Convert.ToDateTime(sqlDataReader["end_date"])
				};

				leavesList.Add(leaves);
			}

			sqlDataReader.Close();
			ConnectionStrings._sqlConnection.Close();

			return leavesList;
		}

		public Leave GetById(int id)
		{
			ConnectionStrings.ConnectionControl();

			SqlCommand sqlCommand = new SqlCommand("SELECT * FROM Leaves WHERE Id=@id", ConnectionStrings._sqlConnection);
			sqlCommand.Parameters.AddWithValue("@id", id);

			SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

			while (sqlDataReader.Read())
			{
				Leave leaves = new Leave
				{
					id = Convert.ToInt32(sqlDataReader["id"]),
					name = sqlDataReader["name"].ToString(),
					surname = sqlDataReader["surname"].ToString(),
					sicil_no = (int) sqlDataReader["sicil_no"],
					start_date = Convert.ToDateTime(sqlDataReader["start_date"]),
					end_date = Convert.ToDateTime(sqlDataReader["end_date"])
				};

				sqlDataReader.Close();
				ConnectionStrings._sqlConnection.Close();

				return leaves;
			}

			sqlDataReader.Close();
			ConnectionStrings._sqlConnection.Close();

			return null;
		}

		public void Add(Leave leaves)
		{
			ConnectionStrings.ConnectionControl();

			SqlCommand sqlCommand = new SqlCommand("INSERT INTO Leaves (name, surname, sicil_no, start_date, end_date) VALUES (@name, @surname, @sicil_no, @start_date, @end_date)", ConnectionStrings._sqlConnection);

			sqlCommand.Parameters.AddWithValue("@name", leaves.name);
			sqlCommand.Parameters.AddWithValue("@surname", leaves.surname);
			sqlCommand.Parameters.AddWithValue("@sicil_no", leaves.sicil_no);
			sqlCommand.Parameters.AddWithValue("@start_date", leaves.start_date);
			sqlCommand.Parameters.AddWithValue("@end_date", leaves.end_date);

			MessageBox.Show("name " + leaves.name + " surname " + leaves.surname + " sicil_no " + leaves.sicil_no);

			sqlCommand.ExecuteNonQuery();

			ConnectionStrings._sqlConnection.Close();
		}

		public void Update(Leave leaves)
		{
			ConnectionStrings.ConnectionControl();

			SqlCommand sqlCommand = new SqlCommand("UPDATE Leaves SET name=@name, surname=@surname, sicil_no=@sicil_no, start_date=@start_date, end_date=@end_date WHERE id=@id", ConnectionStrings._sqlConnection);

			sqlCommand.Parameters.AddWithValue("@id", leaves.id);
			sqlCommand.Parameters.AddWithValue("@name", leaves.name);
			sqlCommand.Parameters.AddWithValue("@surname", leaves.surname);
			sqlCommand.Parameters.AddWithValue("@sicil_no", leaves.sicil_no);
			sqlCommand.Parameters.AddWithValue("@start_date", leaves.start_date);
			sqlCommand.Parameters.AddWithValue("@end_date", leaves.end_date);

			sqlCommand.ExecuteNonQuery();

			ConnectionStrings._sqlConnection.Close();
		}

		public void Delete(int id)
		{
			ConnectionStrings.ConnectionControl();

			SqlCommand sqlCommand = new SqlCommand("DELETE FROM Leaves WHERE id=@id", ConnectionStrings._sqlConnection);
			sqlCommand.Parameters.AddWithValue("@id", id);

			sqlCommand.ExecuteNonQuery();

			ConnectionStrings._sqlConnection.Close();
		}
	}
}
