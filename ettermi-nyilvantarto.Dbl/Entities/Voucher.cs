using System;

namespace ettermi_nyilvantarto.Dbl.Entities
{
	public class Voucher
	{
		public int Id { get; set; }
		public int VoucherNumber { get; set; }  //or string??
		public int DiscountThreshold { get; set; } = 0;
		public int? DiscountPercentage { get; set; }
		public int? DiscountAmount { get; set; }
		public DateTime ActiveFrom { get; set; }
		public DateTime ActiveTo { get; set; }
		public bool IsActive { get; set; } = true;	//=status
	}
}
