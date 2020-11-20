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
		private RestaurantDbContext DbContext { get; set; }
		public FeedbackService(RestaurantDbContext dbContext)
		{
			DbContext = dbContext;
		}

		public async Task<IEnumerable<FeedbackListModel>> GetFeedbackList()
			=> (await DbContext.Feedback.ToListAsync()).Select(f => new FeedbackListModel
			{
				Id = f.Id,
				OrderId = f.OrderId,
				Rating = f.Rating,
				Comment = f.Comment,
				Date = f.Date
			});

		public async Task<int> AddFeedback(FeedbackAddModel model)
		{
			var feedback = DbContext.Feedback.Add(new Feedback()
			{
				OrderId = model.OrderId,
				Rating = model.Rating,
				Comment = model.Comment,
				Date = DateTime.Now
			});

			await DbContext.SaveChangesAsync();

			return feedback.Entity.Id;
		}
	}
}
