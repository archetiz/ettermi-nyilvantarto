using System.Collections.Generic;
using System.Threading.Tasks;

namespace ettermi_nyilvantarto.Api
{
	public interface IFeedbackService
	{
		Task<IEnumerable<FeedbackListModel>> GetFeedbackList();
		Task<int> AddFeedback(FeedbackAddModel model);
	}
}
