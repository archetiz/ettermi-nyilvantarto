using System.Threading.Tasks;

namespace ettermi_nyilvantarto.Api
{
	public interface IFeedbackService
	{
		Task<PagedResult<FeedbackListModel>> GetFeedbackList(int page);
		Task<AddResult> AddFeedback(FeedbackAddModel model);
	}
}
