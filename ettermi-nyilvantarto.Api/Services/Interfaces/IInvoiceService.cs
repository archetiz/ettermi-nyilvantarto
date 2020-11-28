using ettermi_nyilvantarto.Dbl.Entities;

namespace ettermi_nyilvantarto.Api
{
	public interface IInvoiceService
	{
		void CreateInvoice(InvoiceCreationModel model);
	}
}
