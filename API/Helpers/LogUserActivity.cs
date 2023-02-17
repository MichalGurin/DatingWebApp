using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Helpers
{
      public class LogUserActivity : IAsyncActionFilter
      {
            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                var result = await next();

                if(!result.HttpContext.User.Identity.IsAuthenticated) return;

                var userId = result.HttpContext.User.GetUserId();

                var uow = result.HttpContext.RequestServices.GetRequiredService<IUnitOfWork>();
                var user = await uow.UserRepository.GetUserByIdAsync(userId);
                user.LastActive = DateTime.UtcNow;
                await uow.Complete();
            }
      }
}