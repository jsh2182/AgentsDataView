using AgentsDataView.Data.Contracts;
using AgentsDataView.Entities;
using AgentsDataView.Entities.DtoModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AgentsDataView.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController(IRepository<Setting> settingRepo) : ControllerBase
    {
        private IRepository<Setting> _settingRepo = settingRepo;

        [HttpGet("[action]")]
        public async Task<ActionResult<SettingDto>> GetSetting(string name, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest("نام تنظیمات درست نیست.");
            }
            var setting = await _settingRepo.QueryNoTracking.FirstOrDefaultAsync(s => s.SettingName.ToLower() == name.ToLower(), cancellationToken);
            if (setting == null)
            {
                return BadRequest("اطلاعات درخواستی یافت نشد.");
            }
            SettingDto result = new()
            {
                SettingName = setting.SettingName,
                SettingValue = setting.SettingValue
            };
            return Ok(result);
        }
        [HttpPut("[action]")]
        public async Task<ActionResult<SettingDto>> UpdateSetting(SettingDto dto, CancellationToken cancellationToken)
        {
            var setting = await _settingRepo.Query.FirstOrDefaultAsync(s => s.SettingName.ToLower() == dto.SettingName.ToLower(), cancellationToken);
            if (setting == null)
            {
                return BadRequest("اطلاعات درخواستی یافت نشد.");
            }
            setting.SettingValue = dto.SettingValue;
            await _settingRepo.UpdateAsync(setting, cancellationToken);
            return Ok("Success");
        }
    }
}
