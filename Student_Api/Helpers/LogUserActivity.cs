using Microsoft.AspNetCore.Mvc.Filters;
using Student_Api.Extensions;
using Student_Api.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Student_Api.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {

            var resultContext = await next();
            if (!resultContext.HttpContext.User.Identity.IsAuthenticated) return;
            var userName = resultContext.HttpContext.User.GetUsername();
            var repo = resultContext.HttpContext.RequestServices.GetService<IStudentRepository>();
            var user = await repo.GetUserByUsernameAsync(userName);
            user.LastActive = DateTime.Now;
            await repo.SaveAllAsync();
        }
    }
}
