using System.ComponentModel;

namespace ettermi_nyilvantarto.Dbl.Entities
{
	public enum PaymentMethod
	{
		[Description("Készpénz")]
		Cash,
		[Description("Kártya")]
		Card
	}
}
