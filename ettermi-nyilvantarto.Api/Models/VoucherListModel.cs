﻿using System;

namespace ettermi_nyilvantarto.Api
{
	public class VoucherListModel
	{
		public int Id { get; set; }
		public string Code { get; set; }
		public int DiscountThreshold { get; set; }
		public int? DiscountPercentage { get; set; }
		public int? DiscountAmount { get; set; }
		public DateTime ActiveFrom { get; set; }
		public DateTime ActiveTo { get; set; }
	}
}
