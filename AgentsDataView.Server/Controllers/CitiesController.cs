using AgentsDataView.Data.Contracts;
using AgentsDataView.Entities;
using AgentsDataView.Entities.DtoModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AgentsDataView.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CitiesController(IRepository<City> cityRepo) : ControllerBase
    {
        IRepository<City> _cityRepo = cityRepo;

        [HttpGet("[action]")]
        public async Task<ActionResult<CityDto[]>> GetAll(int provinceId, CancellationToken cancellation)
        {
           var result = await _cityRepo.QueryNoTracking.Where(c=>c.ProvinceId == provinceId).Select(c=> new CityDto()
            {
                
               Id = c.Id,
               CityName = c.CityName,
                Code = c.Code,
                ProvinceId = c.ProvinceId,
            }).ToArrayAsync(cancellation).ConfigureAwait(false);
            return Ok(result);
        }
    }
}
