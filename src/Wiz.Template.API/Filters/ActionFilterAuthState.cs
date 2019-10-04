using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Wiz.Template.API.Filters
{
    public class ActionFilterAuthState : IAsyncResultFilter
    {
        public bool _authenticated;
        public string _authentication;

        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            await next();
        }
    }
}
