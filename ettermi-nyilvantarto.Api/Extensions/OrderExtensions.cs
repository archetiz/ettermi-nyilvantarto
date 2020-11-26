using ettermi_nyilvantarto.Dbl.Entities;
using System;
using System.Linq;

namespace ettermi_nyilvantarto.Api
{
	public static class OrderExtensions
	{
		public static int CalculatePrice(this Order order)
		{
			if (order.Items == null || (order.Items.Count() > 0 && order.Items[0].MenuItem == null))
				throw new ArgumentException("Order must include it's items and the corresponding menu items!");

			int sum = 0;
			order.Items.ForEach(oi =>
			{
				sum += oi.Quantity * oi.MenuItem.Price;
			});
			return sum;
		}
	}
}
