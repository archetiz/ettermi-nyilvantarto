using ettermi_nyilvantarto.Dbl;
using ettermi_nyilvantarto.Dbl.Configurations;
using ettermi_nyilvantarto.Dbl.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ettermi_nyilvantarto.Api
{
	public class FeedbackService : IFeedbackService
	{
		private RestaurantDbContext DbContext { get; }
		private PagingConfiguration PagingConfig { get; }
		public FeedbackService(RestaurantDbContext dbContext, IOptions<PagingConfiguration> pagingConfig)
		{
			this.DbContext = dbContext;
			this.PagingConfig = pagingConfig.Value;
		}

		public async Task<PagedResult<FeedbackListModel>> GetFeedbackList(int page)
			=> (await DbContext.Feedback
							.OrderByDescending(f => f.Date)
							.GetPaged(page, PagingConfig.PageSize, out int totalPages)
							.Select(f => new FeedbackListModel
							{
								Id = f.Id,
								OrderSessionId = f.OrderSessionId,
								Rating = f.Rating,
								Comment = f.Comment,
								Date = f.Date
							}).ToListAsync()).GetPagedResult(page, PagingConfig.PageSize, totalPages);

		public async Task<AddResult> AddFeedback(FeedbackAddModel model)
		{
			if (!(model.Rating >= 0 && model.Rating <= 5))
				throw new RestaurantBadRequestException("Az értékelés értékének 0 és 5 közé kell esnie!");

			var orderSession = await DbContext.OrderSessions.FindAsync(model.OrderSessionId);
			if (orderSession == null)
				throw new RestaurantNotFoundException("A megadott rendelési folyamat nem létezik!");

			var feedback = DbContext.Feedback.Add(new Feedback()
			{
				OrderSessionId = model.OrderSessionId,
				Rating = model.Rating,
				Comment = model.Comment,
				Date = DateTime.Now
			});

			await DbContext.SaveChangesAsync();

			return new AddResult(feedback.Entity.Id);
		}
	}
}
