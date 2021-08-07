using System.Threading.Tasks;
using Odco.PointOfSales.Models.TokenAuth;
using Odco.PointOfSales.Web.Controllers;
using Shouldly;
using Xunit;

namespace Odco.PointOfSales.Web.Tests.Controllers
{
    public class HomeController_Tests: PointOfSalesWebTestBase
    {
        [Fact]
        public async Task Index_Test()
        {
            await AuthenticateAsync(null, new AuthenticateModel
            {
                UserNameOrEmailAddress = "admin",
                Password = "123qwe"
            });

            //Act
            var response = await GetResponseAsStringAsync(
                GetUrl<HomeController>(nameof(HomeController.Index))
            );

            //Assert
            response.ShouldNotBeNullOrEmpty();
        }
    }
}