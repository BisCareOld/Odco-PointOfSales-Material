using Abp.Authorization;
using Odco.PointOfSales.Authorization.Roles;
using Odco.PointOfSales.Authorization.Users;

namespace Odco.PointOfSales.Authorization
{
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {
        }
    }
}
