using Abp.Application.Services.Dto;

namespace Odco.PointOfSales.Roles.Dto
{
    public class PagedRoleResultRequestDto : PagedResultRequestDto
    {
        public string Keyword { get; set; }
    }
}

