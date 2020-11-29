using System;

namespace ettermi_nyilvantarto.Dbl.Entities
{
	public class Reservation
	{
		public int Id { get; set; }
		public int TableId { get; set; }
		public Table Table { get; set; }
		public DateTime TimeFrom { get; set; }
		public DateTime TimeTo { get; set; }
		public string CustomerName { get; set; }
		public string CustomerPhoneNumber { get; set; }
		public bool IsActive { get; set; } = true;	//=status
	}
}
