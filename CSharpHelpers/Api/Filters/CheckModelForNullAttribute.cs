using System.Linq;
using System.Web.Http.Controllers;

namespace CSharpHelpers.Api.Filters
{
    public class CheckModelForNullAttribute : System.Web.Http.Filters.ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            actionContext.ActionArguments.Where(x => x.Value == null).ToList().ForEach(x =>
            {
                actionContext.ModelState.AddModelError(x.Key, "Required parameter is null");
            });
        }
    }
}
