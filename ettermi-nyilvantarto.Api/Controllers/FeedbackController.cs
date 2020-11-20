using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ettermi_nyilvantarto.Api
{
	[ApiController]
	[Route("api/feedback")]
	[Produces("application/json")]
	public class FeedbackController : ControllerBase
	{
		private IFeedbackService FeedbackService { get; set; }
		public FeedbackController(IFeedbackService feedbackService)
		{
			this.FeedbackService = feedbackService;
		}

		[HttpGet]
		[Authorize(Roles = "Owner")]
		public async Task<IEnumerable<FeedbackListModel>> GetFeedbackList()
			=> await FeedbackService.GetFeedbackList();

		[HttpPost]
		[Authorize(Roles = "Owner, Waiter")]
		public async Task<int> AddFeedback(FeedbackAddModel feedback)
			=> await FeedbackService.AddFeedback(feedback);
	}
}
