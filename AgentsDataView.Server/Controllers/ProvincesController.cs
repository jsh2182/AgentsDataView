using AgentsDataView.Common;
using AgentsDataView.Data.Contracts;
using AgentsDataView.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;

namespace AgentsDataView.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProvincesController(IRepository<Province> provinceRepo, IRepository<Company> companyRepo) : ControllerBase
    {
        private readonly IRepository<Province> _provinceRepo = provinceRepo;
        private readonly IRepository<Company> _companyRepo = companyRepo;

        [HttpGet("[action]")]
        public async Task<ActionResult> GetAll(bool filterOnUser, CancellationToken cancellationToken)
        {

            IIdentity identity = HttpContext.User.Identity;
            int[] companyIds = identity.GetCompanyIds();

            var qry = _provinceRepo.QueryNoTracking;

            if (filterOnUser && companyIds.Length > 0)
            {
                var provinceIds = await _companyRepo.QueryNoTracking
                    .Where(c => c.ProvinceId.HasValue &&
                                companyIds.Contains(c.Id))
                    .Select(c => c.ProvinceId)
                    .ToArrayAsync(cancellationToken);
                if (provinceIds.Length > 0)
                    qry = qry.Where(q => provinceIds.Contains(q.Id));
            }

            var result = await qry.Select(p => new
            {
                p.Id,
                p.ProvinceName
            }).ToArrayAsync(cancellationToken);
            return Ok(result);
        }
    }
}
