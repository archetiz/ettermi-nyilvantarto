using ettermi_nyilvantarto.Dbl.Entities;
using System.Threading.Tasks;

namespace ettermi_nyilvantarto.Api
{
	public interface IInvoiceService
	{
		Task<int> CreateInvoice(InvoiceCreationModel model);
	}
}
