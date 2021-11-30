using Abp.Application.Services;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Odco.PointOfSales.Application.GeneralDto;
using Odco.PointOfSales.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Odco.PointOfSales.Application.Common
{
    public class CommonAppService : ApplicationService, ICommonAppService
    {
        private readonly IRepository<Bank, Guid> _bankRepository;

        public CommonAppService(IRepository<Bank, Guid> bankRepository)
        {
            _bankRepository = bankRepository;
        }

        public async Task<List<CommonKeyValuePairDto>> GetAllBanksAsync()
        {
            return await _bankRepository.GetAll()
                .Where(b => b.IsActive)
                .Select(b => new CommonKeyValuePairDto
                {
                    Id = b.Id,
                    Name = b.Name
                }).ToListAsync();
        }
    }
}
