using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ettermi_nyilvantarto.Api
{
	[ApiController]
	[Route("api/feedback")]
	[Produces("application/json")]
	public class FeedbackController : ControllerBase
	{
		private IFeedbackService FeedbackService { get; }
		public FeedbackController(IFeedbackService feedbackService)
		{
			this.FeedbackService = feedbackService;
		}

		[HttpGet]
		[HttpGet("page/{page}")]
		[Authorize(Roles = "Owner")]
		public async Task<PagedResult<FeedbackListModel>> GetFeedbackList(int page = 1)
			=> await FeedbackService.GetFeedbackList(page);

		[HttpPost]
		[Authorize(Roles = "Owner, Waiter")]
		public async Task<int> AddFeedback(FeedbackAddModel feedback)
			=> await FeedbackService.AddFeedback(feedback);
	}
}
