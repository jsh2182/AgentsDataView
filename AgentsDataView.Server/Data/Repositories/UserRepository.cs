using AgentsDataView.Common.Exceptions;
using AgentsDataView.Common.Utilities;
using AgentsDataView.Entities;
using Microsoft.EntityFrameworkCore;

namespace AgentsDataView.Data.Repositories
{
    public class UserRepository(ApplicationDbContext dbContext) : Repository<SystemUser>(dbContext), IUserRepository
    {

        public async Task AddAsync(SystemUser user, CancellationToken cancellationToken)
        {

            var exists = await QueryNoTracking.FirstOrDefaultAsync(p => p.UserName == user.UserName || (!string.IsNullOrEmpty(user.UserMobile) && p.UserMobile == user.UserMobile), cancellationToken).ConfigureAwait(false);
            if (exists != null)
            {
                if(exists.UserMobile == user.UserMobile)
                {
                    throw new BadRequestException("کاربر دیگری با این شماره همراه در سیستم وجود دارد.");
                }
                throw new BadRequestException("نام کاربری تکراری است.");
            }

            if (user.Password.HasValue())
            {
                string passwordHash = SecurityHelper.GetSha256Hash(user.Password);
                user.Password = passwordHash;
            }
            await base.AddAsync(user, cancellationToken);
        }

        public async Task UpdateAsync(SystemUser user, CancellationToken cancellationToken)
        {
            if (user.Id < 1)
            {
                throw new Exception("شناسه کاربر صحیح نیست");
            }
            var exists = await Query.AnyAsync(p => (p.UserName == user.UserName || p.UserMobile == user.UserMobile) && p.Id != user.Id, cancellationToken).ConfigureAwait(false);
            if (exists)
            {
                throw new Exception("نام کاربری یا شماره همراه تکراری است");
            }
            await base.UpdateAsync(user, cancellationToken);
        }

        public async Task DeleteAsync(int userID, CancellationToken cancellationToken)
        {
            if (userID < 1)
            {
                throw new Exception("شناسه کاربر صحیح نیست");
            }
            await _checkDeleteIsValid(userID, cancellationToken);
            await Query.Where(u => u.Id == userID).ExecuteDeleteAsync(cancellationToken);
        }

        public async Task<bool> UserIsValid(int userID, CancellationToken cancellationToken)
        {
            bool result = await QueryNoTracking.AnyAsync(u => u.Id == userID && u.IsActive, cancellationToken);
            return result;
        }

        public Task<SystemUser?> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<SystemUser?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<SystemUser?> FindByUserAndPass(string username, string password, CancellationToken cancellationToken)
        {
            string passwordHash = SecurityHelper.GetSha256Hash(password);
            username = username.FixPersianChars().ToLower();
            var user = await QueryNoTracking.FirstOrDefaultAsync(u => (u.UserName == username || u.UserMobile == username) && u.Password == passwordHash, cancellationToken);
            return user;
        }

        public async Task UpdateLastLoginDateAsync(SystemUser user, CancellationToken cancellationToken)
        {
            await Entities.Where(u => u.Id == user.Id).ExecuteUpdateAsync(setters => setters.SetProperty(u => u.LastLoginDate, DateTime.UtcNow), cancellationToken);
        }

        private async Task _checkDeleteIsValid(int userID, CancellationToken cancellationToken)
        {
     
            //bool delIsForbidden = await DbContext.Set<Company>().AsNoTracking().AnyAsync(c => c.ManagerUserID == userID, cancellationToken).ConfigureAwait(false);
            //if (delIsForbidden)
            //{
            //    throw new Exception("این کاربر مدیر شرکت است و حذف آن مجاز نیست.");
            //}
            //delIsForbidden = await DbContext.Set<Person>().AsNoTracking().AnyAsync(p => p.CreatingUserID == userID, cancellationToken).ConfigureAwait(false);
            //if (delIsForbidden)
            //{
            //    throw new Exception("این کاربر یک یا چند شخص جدید ثبت کرده و حذف آن مجاز نیست.");
            //}
            //delIsForbidden = await DbContext.Set<ServiceReception>().AsNoTracking().AnyAsync(s=>s.ArchiverUserID == userID || s.CreateByID == userID || s.UpdateByID == userID, cancellationToken).ConfigureAwait(false);
            //if (delIsForbidden)
            //{
            //    throw new Exception("این کاربر یک یا چند سرویس ثبت، ویرایش و یا بایگانی کرده است و حذف آن مجاز نیست.");
            //}
            //delIsForbidden = await DbContext.Set<GuaranteeCard>().AsNoTracking().AnyAsync(g => g.CreatingUserID == userID || g.UpdatingUserID == userID, cancellationToken).ConfigureAwait(false);
            //if (delIsForbidden)
            //{
            //    throw new Exception("این کاربر یک یا چند کارت گارانتی ثبت/ویرایش کرده است و حذف آن مجاز نیست.");
            //}
            //delIsForbidden = await DbContext.Set<StageUserList>().AsNoTracking().AnyAsync(u=>u.UserID == userID, cancellationToken).ConfigureAwait(false);
            //if (delIsForbidden)
            //{
            //    throw new Exception("برای این کاربر یک یا چند مرحله کاری تعریف شده و حذف آن مجاز نیست.");
            //}
            //delIsForbidden = await DbContext.Set<UserWorkBook>().AsNoTracking().AnyAsync(u => u.ReferredByID == userID || u.ReferredToID == userID, cancellationToken).ConfigureAwait(false);
            //if (delIsForbidden)
            //{
            //    throw new Exception("این کاربر گردش کار دارد و حذف آن مجاز نیست.");
            //}
        }
    }
}