using System;

namespace PersonelVardiyaOtomasyonu.Entities
{
	public class Leave
	{
		public int id { get; set; }
		public string name { get; set; }
		public string surname { get; set; }

		public int sicil_no { get; set; }

		public DateTime start_date { get; set; }
		public DateTime end_date { get; set; }


	}
}
