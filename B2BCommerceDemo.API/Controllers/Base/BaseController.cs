using B2BCommerceDemo.Core.Interfaces.Users;
using Microsoft.AspNetCore.Mvc;

namespace B2BCommerceDemo.API.Controllers.Base
{
    public abstract class BaseController : ControllerBase
    {
        protected readonly IUserContext UserContext;

        protected BaseController(IUserContext userContext)
        {
            UserContext = userContext;
        }

        protected int? TryGetCompanyId()
        {
            return UserContext.CompanyId;
        }

        protected int GetCompanyId()
        {
            return UserContext.CompanyId
                ?? throw new UnauthorizedAccessException("Missing CompanyId");
        }

        protected string GetUserId()
        {
            return UserContext.UserId
                ?? throw new UnauthorizedAccessException("Missing UserId");
        }

        protected bool IsAdmin => UserContext.IsAdmin;
        protected bool IsAuthenticated => UserContext.IsAuthenticated;
    }
}
