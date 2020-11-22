﻿using System.Collections.Generic;

namespace ettermi_nyilvantarto.Dbl.Entities
{
	public class BillingData
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string TaxNumber { get; set; }
		public string Address { get; set; }
		public string PhoneNumber { get; set; }
		public string Email { get; set; }
		public List<Invoice> Invoices { get; set; }
	}
}
