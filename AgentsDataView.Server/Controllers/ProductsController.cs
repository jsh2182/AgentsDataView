using AgentsDataView.Common;
using AgentsDataView.Common.Utilities;
using AgentsDataView.Data.Contracts;
using AgentsDataView.Entities;
using AgentsDataView.Entities.DtoModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;

namespace AgentsDataView.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController(IRepository<Product> productRepo, IRepository<Company> companyRepo, IRepository<InvoiceDetail> invoiceDetailRepo) : ControllerBase
    {
        private readonly IRepository<Product> _productRepo = productRepo;
        private readonly IRepository<Company> _companyRepo = companyRepo;
        private readonly IRepository<InvoiceDetail> _invoiceDetailRepo = invoiceDetailRepo;

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> Get(int id, CancellationToken cancellationToken)
        {
            if (id < 1)
            {
                return BadRequest("شناسه ارسالی معتبر نیست.");
            }
            var product = await _productRepo.GetByIdAsync(cancellationToken, id).ConfigureAwait(false);
            if (product == null)
            {
                return BadRequest("اطلاعات درخواستی در سیستم وجود ندارد.");
            }
            var result = new ProductDto()
            {
                Code = product.Code,
                CreatedById = product.CreatedById,
                CreationDate = product.CreationDate,
                Id = product.Id,
                Model = product.Model,
                Name = product.Name
            };
            return Ok(result);
        }
        
        [HttpGet("[action]")]
        public async Task<ActionResult<ProductDto>> GetByCode(string code, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return BadRequest("کد ارسالی معتبر نیست.");
            }
            var identity = HttpContext.User.Identity;
            int cId = identity.GetCompanyId();
            var product = await _productRepo.QueryNoTracking.FirstOrDefaultAsync(p=>p.Code == code && p.CompanyId == cId, cancellationToken).ConfigureAwait(false);
            if (product == null)
            {
                return BadRequest("اطلاعات درخواستی در سیستم وجود ندارد.");
            }
            var result = new ProductDto()
            {
                Code = product.Code,
                CreatedById = product.CreatedById,
                CreationDate = product.CreationDate,
                Id = product.Id,
                Model = product.Model,
                Name = product.Name
            };
            return Ok(result);
        }
        [HttpGet("[action]")]
        public async Task<ActionResult<ProductDto>> GetIdByCode(string code, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return BadRequest("کد ارسالی معتبر نیست.");
            }
            var identity = HttpContext.User.Identity;
            int cId = identity.GetCompanyId();
            var productId = await _productRepo.QueryNoTracking.Where(p => p.Code == code && p.CompanyId == cId)
                .Select(p=>p.Id).FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);
            if (productId < 1)
            {
                return BadRequest("اطلاعات درخواستی در سیستم وجود ندارد.");
            }

            return Ok(productId);
        }

        [HttpGet("[action]")]
        public async Task<ActionResult<ProductDto[]>> GetAll(CancellationToken cancellationToken)
        {
            int compId = HttpContext.User.Identity.GetCompanyId();
            var result = await _productRepo.QueryNoTracking.Where(p=>p.CompanyId == compId).Select(p => new ProductDto()
            {
                Code = p.Code,
                CreatedById = p.CreatedById,
                CreationDate = p.CreationDate,
                Id = p.Id,
                Model = p.Model,
                Name = p.Name
            }).ToListAsync(cancellationToken).ConfigureAwait(false);
            return Ok(result);
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<int>> Create(ProductDto model, CancellationToken cancellationToken)
        {
            IIdentity? identity = HttpContext.User.Identity;
            int userId = identity.GetUserId<int>();
            int cId = identity.GetCompanyId();
            if (!(cId > 0))
            {
                return BadRequest("کد شرکت در سیستم وجود ندارد.");
            }
            model.Name = model.Name.CleanString2();
            model.Model = model.Model.CleanString2();
            model.Code = model.Code.CleanString2();
            var existing = await _productRepo.QueryNoTracking
                .FirstOrDefaultAsync(p => p.CompanyId == cId &&
                ((p.Name == model.Name &&
                (string.IsNullOrEmpty(model.Model) || model.Model == p.Model)) ||
                p.Code == model.Code), cancellationToken)
                .ConfigureAwait(false);
            if (existing != null)
            {
                if (existing.Code == model.Code)
                {
                    return BadRequest("کالای دیگری با این کد در سیستم وجود دارد.");
                }
                if (existing.Name == model.Name && existing.Model == model.Model)
                {
                    return BadRequest("کالای دیگری با این نام و مدل در سیستم وجود دارد.");
                }
            }
            var newProduct = new Product()
            {
                Id = 0,
                Name = model.Name,
                Code = model.Code,
                CreatedById = userId,
                CompanyId = cId,
                Model = model.Model
            };
            await _productRepo.AddAsync(newProduct, cancellationToken).ConfigureAwait(false);
            return Ok(newProduct.Id);
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<string>> CreateBatch(List<ProductDto> model, CancellationToken cancellationToken)
        {

            var indentity = HttpContext.User.Identity;
            int userId = indentity.GetUserId<int>();
            int cId = indentity.GetCompanyId();
            model.ForEach(m =>
            {
                m.Name = m.Name.CleanString2();
                m.Model = m.Model.CleanString2();
                m.Code = m.Code.CleanString2();
            });
            var allProducts = await _productRepo.QueryNoTracking
                .Where(p => p.CompanyId == cId).Select(p => new { p.Code, p.Name, p.Model }).ToListAsync(cancellationToken)
                .ConfigureAwait(false);
            var dbSetCode = new HashSet<string>(
                allProducts.Select(p => p.Code)
            );
            var duplicatesCode = model
                .Where(p => dbSetCode.Contains(p.Code))
                .ToList();
            if (duplicatesCode.Count > 0)
            {
                string msg = string.Join(',', duplicatesCode.Select(d => d.Code));
                return BadRequest($"تعدادی از کالاها پیش از این در سیستم ثبت شده اند:${msg}");
            }
            var dbSet = new HashSet<(string Code, string Name, string Model)>(
                allProducts.Select(p => (p.Code, p.Name, p.Model)),
                StringTupleComparer.Instance
            );
            var duplicates = model
                .Where(p => dbSet.Contains((p.Code, p.Name, p.Model)))
                .ToList();

            if (duplicates?.Count > 0)
            {
                string msg = string.Join(',', duplicates.Select(d => d.Code + "-" + d.Name + "-" + d.Model));
                return BadRequest($"تعدادی از کالاها پیش از این در سیستم ثبت شده اند:${msg}");
            }
            var list = model.Select(p => new Product()
            {
                Id = 0,
                Code = p.Code,
                CompanyId = cId,
                CreatedById = userId,
                Model = p.Model,
                Name = p.Name,
            }).ToList();
            await _productRepo.BulkInsertWithOutputIdsAsync(list, cancellationToken).ConfigureAwait(false);
            var result = list.Select(p => new { p.Id, p.Code });
            return Ok(result);
        }

        [HttpPut("[action]")]
        public async Task<ActionResult<string>> Update(ProductDto model, CancellationToken cancellationToken)
        {
            if (!(model.Id > 0))
            {
                return BadRequest("شناسه ارسالی معتبر نیست.");
            }

            IIdentity? identity = HttpContext.User.Identity;
            int userId = identity.GetUserId<int>();
            int cId = identity.GetCompanyId();
            if (!(cId > 0))
            {
                return BadRequest("کد شرکت در سیستم وجود ندارد.");
            }
            var product = await _productRepo.GetByIdAsync(cancellationToken, model.Id).ConfigureAwait(false);
            if (product == null || product.CompanyId != cId)
            {
                return BadRequest("اطلاعات درخواستی در سیستم وجود ندارد.");
            }
            model.Name = model.Name.CleanString2();
            model.Model = model.Model.CleanString2();
            model.Code = model.Code.CleanString2();
            var existing = await _productRepo.QueryNoTracking
                .FirstOrDefaultAsync(p => p.Id != model.Id && p.CompanyId == cId &&
                (p.Name == model.Name &&
                (string.IsNullOrEmpty(model.Model) || model.Model == p.Model) ||
                p.Code == model.Code), cancellationToken)
                .ConfigureAwait(false);
            if (existing != null)
            {
                if (existing.Code == model.Code)
                {
                    return BadRequest("کالای دیگری با این کد در سیستم وجود دارد.");
                }
                if (existing.Name == model.Name && existing.Model == model.Model)
                {
                    return BadRequest("کالای دیگری با این نام و مدل در سیستم وجود دارد.");
                }
            }
            product.Name = model.Name;
            product.Code = model.Code;
            product.CreatedById = userId;
            product.Model = model.Model;

            await _productRepo.UpdateAsync(product, cancellationToken).ConfigureAwait(false);
            return Ok("Success");
        }

        [HttpDelete("[action]")]
        public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            if(id < 1)
            {
                return BadRequest("شناسه ارسالی معتبر نیست.");
            }
            int companyId = HttpContext.User.Identity.GetCompanyId();
            var product = await _productRepo.Query.FirstOrDefaultAsync(p=>p.Id == id && p.CompanyId == companyId, cancellationToken).ConfigureAwait(false); 
            if(product == null)
            {
                return BadRequest("اطلاعات درخواستی در سیستم وجود ندارد.");
            }
            bool hasInvoice = await _invoiceDetailRepo.QueryNoTracking.AnyAsync(d => d.ProductId == id, cancellationToken);
            if (hasInvoice)
            {
                return BadRequest("برای این کالا فاکتور ثبت شده است.");
            }
            await _productRepo.DeleteAsync(product, cancellationToken).ConfigureAwait(false);
            return Ok("Success");
            

        }


        private class StringTupleComparer : IEqualityComparer<(string Code, string Name, string Model)>
        {
            public static readonly StringTupleComparer Instance = new();

            public bool Equals((string Code, string Name, string Model) x, (string Code, string Name, string Model) y)
            {
                return string.Equals(x.Code ?? string.Empty, y.Code ?? string.Empty, StringComparison.OrdinalIgnoreCase)
                    && string.Equals(x.Name ?? string.Empty, y.Name ?? string.Empty, StringComparison.OrdinalIgnoreCase)
                    && string.Equals(x.Model ?? string.Empty, y.Model ?? string.Empty, StringComparison.OrdinalIgnoreCase);
            }

            public int GetHashCode((string Code, string Name, string Model) obj)
            {
                // استفاده از StringComparer برای اطمینان از hash code مطابق با Equals (ignore case)
                unchecked
                {
                    int h = 17;
                    h = h * 23 + StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Code ?? string.Empty);
                    h = h * 23 + StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Name ?? string.Empty);
                    h = h * 23 + StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Model ?? string.Empty);
                    return h;
                }
            }
        }

    }
}
