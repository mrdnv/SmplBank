using Microsoft.AspNetCore.Mvc.Filters;
using System.Data;
using System.Threading.Tasks;
using System.Transactions;

namespace SmplBank.Filters
{
    public class TransactionAttribute : ActionFilterAttribute
    {
        public TransactionAttribute()
        {
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var dbConnection = context.HttpContext.RequestServices.GetService(typeof(IDbConnection)) as IDbConnection;

            if (dbConnection == null)
            {
                await base.OnActionExecutionAsync(context, next);
                return;
            }

            using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                using(dbConnection)
                {
                    await base.OnActionExecutionAsync(context, next);
                    transactionScope.Complete();
                }
            }
        }
    }
}
