using AgentsDataView.Data.Contracts;
using AgentsDataView.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AgentsDataView.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProvincesController(IRepository<Province> provinceRepo) : ControllerBase
    {
        private readonly IRepository<Province> _provinceRepo  = provinceRepo;

        [HttpGet("[action]")]
        public async Task<ActionResult> GetAll(CancellationToken cancellationToken)
        {
           var result = await _provinceRepo.QueryNoTracking.Select(p => new
            {
                p.Id,
                p.ProvinceName
            }).ToArrayAsync(cancellationToken);
            return Ok(result);
        }
    }
}
