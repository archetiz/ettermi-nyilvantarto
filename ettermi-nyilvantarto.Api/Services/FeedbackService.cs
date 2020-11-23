using ettermi_nyilvantarto.Dbl;
using ettermi_nyilvantarto.Dbl.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ettermi_nyilvantarto.Api
{
	public class FeedbackService : IFeedbackService
	{
		private RestaurantDbContext DbContext { get; }
		public FeedbackService(RestaurantDbContext dbContext)
		{
			this.DbContext = dbContext;
		}

		public async Task<IEnumerable<FeedbackListModel>> GetFeedbackList()
			=> (await DbContext.Feedback.OrderByDescending(f => f.Date).ToListAsync()).Select(f => new FeedbackListModel
			{
				Id = f.Id,
				OrderSessionId = f.OrderSessionId,
				Rating = f.Rating,
				Comment = f.Comment,
				Date = f.Date
			});

		public async Task<int> AddFeedback(FeedbackAddModel model)
		{
			var feedback = DbContext.Feedback.Add(new Feedback()
			{
				OrderSessionId = model.OrderSessionId,
				Rating = model.Rating,
				Comment = model.Comment,
				Date = DateTime.Now
			});

			await DbContext.SaveChangesAsync();

			return feedback.Entity.Id;
		}
	}
}
