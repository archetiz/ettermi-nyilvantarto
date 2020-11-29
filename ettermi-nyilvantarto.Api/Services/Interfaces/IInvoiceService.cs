using ettermi_nyilvantarto.Dbl.Entities;
using System.IO;
using System.Threading.Tasks;

namespace ettermi_nyilvantarto.Api
{
	public interface IInvoiceService
	{
		Task<Stream> GetInvoice(int id);
		Task<int> CreateInvoice(InvoiceCreationModel model);
	}
}
