using System.Security.Claims;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using BookShareApp.API.DataAccess;

namespace BookShareApp.API.Framework
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next();
            int userId = 
                   int.TryParse(resultContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value, out userId)
                   ? userId : -1;

            var repo = resultContext.HttpContext.RequestServices.GetService<IBookShareRepository>();
            var user = await repo.GetUser(userId);
            user.LastActive = DateTime.Now;

            await repo.SaveAll();
        }
    }
}