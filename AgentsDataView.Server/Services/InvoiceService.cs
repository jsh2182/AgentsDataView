using AgentsDataView.Common.Exceptions;
using AgentsDataView.Data.Contracts;
using AgentsDataView.Data.Helper;
using AgentsDataView.Entities;
using AgentsDataView.Entities.DtoModels;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.Design;
using System.Globalization;

namespace AgentsDataView.Services
{
    public class InvoiceService(IRepository<FinancialPeriod> financialPeriodRepo,
        IRepository<InvoiceDetail> invoiceDetailRepo,
                                IRepository<Invoice> invoiceRepo,
                                IRepository<Product> productRepo,
                                IRepository<Setting> settingRepo) : IInvoiceService
    {
        private readonly IRepository<FinancialPeriod> _financialPeriodRepo = financialPeriodRepo;
        private readonly IRepository<InvoiceDetail> _invoiceDetailRepo = invoiceDetailRepo;
        private readonly IRepository<Invoice> _invoiceRepo = invoiceRepo;
        private readonly IRepository<Product> _productRepo = productRepo;
        private readonly IRepository<Setting> _settingRepo = settingRepo;

        public async Task<InvoiceDto> Find(long invoiceId, int companyId, CancellationToken cancellationToken)
        {
            if (invoiceId < 1)
            {
                throw new BadRequestException("شناسه فاکتور معتبر نیست.");
            }

            Invoice? invoice = await _invoiceRepo.QueryNoTracking.Include(i => i.InvoiceDetails)
                                                                 .ThenInclude(d => d.Product)
                                                                 .FirstOrDefaultAsync(i => i.Id == invoiceId && i.CompanyId == companyId, cancellationToken)
                                                                 .ConfigureAwait(false)
                ?? throw new BadRequestException("اطلاعات درخواستی در سیستم وجود ندارد.");
            InvoiceDto result = new()
            {
                ClientUserName = invoice.ClientUserName,
                CreatedById = invoice.CreatedById,
                CreationDate = invoice.CreationDate,
                CustomerId = invoice.CustomerId,
                FinancialPeriodId = invoice.FinancialPeriodId,
                Id = invoice.Id,
                InvoiceDate = invoice.InvoiceDate,
                InvoiceDescription = invoice.InvoiceDescription,
                InvoiceNumber = invoice.InvoiceNumber,
                InvoiceState = invoice.InvoiceState,
                InvoiceType = invoice.InvoiceType,
                UpdateDate = invoice.UpdateDate,
                UpdatedById = invoice.UpdatedById,
                InvoiceDetails = invoice.InvoiceDetails?.Select(d => new InvoiceDetailDto()
                {
                    Currency = d.Currency,
                    DetailDescription = d.DetailDescription,
                    DiscountPercent = d.DiscountPercent,
                    DiscountValue = d.DiscountValue,
                    InvoiceId = d.InvoiceId,
                    ProductId = d.ProductId,
                    ProductModel = d.Product?.Model ?? "",
                    ProductName = d.Product?.Name ?? "",
                    Quantity = d.Quantity,
                    TaxPercent = d.TaxPercent,
                    TaxValue = d.TaxValue,
                    UnitPrice = d.UnitPrice,
                    CostOfGoodsSold = d.CostOfGoodsSold,
                })?.ToList() ?? []
            };
            return result;
        }
        public async Task<InvoiceDto> Find(int invoiceNumber, BaseInfoes.InvoiceTypes invoiceType, int companyId, int financialPeriodId, CancellationToken cancellationToken)
        {
            if (invoiceNumber < 1)
            {
                throw new BadRequestException("شماره فاکتور معتبر نیست.");
            }
            if (financialPeriodId < 1)
            {
                throw new BadRequestException("دوره مالی معتبر نیست.");
            }

            Invoice? invoice = await _invoiceRepo.QueryNoTracking.Include(i => i.InvoiceDetails)
                                                                 .ThenInclude(d => d.Product)
                                                                 .FirstOrDefaultAsync(i => i.CompanyId == companyId && i.FinancialPeriodId == financialPeriodId && i.InvoiceNumber == invoiceNumber && i.InvoiceType == invoiceType, cancellationToken)
                                                                 .ConfigureAwait(false)
                ?? throw new BadRequestException("اطلاعات درخواستی در سیستم وجود ندارد.");
            InvoiceDto result = new()
            {
                ClientUserName = invoice.ClientUserName,
                CreatedById = invoice.CreatedById,
                CreationDate = invoice.CreationDate,
                CustomerId = invoice.CustomerId,
                FinancialPeriodId = invoice.FinancialPeriodId,
                Id = invoice.Id,
                InvoiceDate = invoice.InvoiceDate,
                InvoiceDescription = invoice.InvoiceDescription,
                InvoiceNumber = invoice.InvoiceNumber,
                InvoiceState = invoice.InvoiceState,
                InvoiceType = invoice.InvoiceType,
                UpdateDate = invoice.UpdateDate,
                UpdatedById = invoice.UpdatedById,
                InvoiceDetails = invoice.InvoiceDetails?.Select(d => new InvoiceDetailDto()
                {
                    Currency = d.Currency,
                    DetailDescription = d.DetailDescription,
                    DiscountPercent = d.DiscountPercent,
                    DiscountValue = d.DiscountValue,
                    InvoiceId = d.InvoiceId,
                    ProductId = d.ProductId,
                    ProductModel = d.Product?.Model ?? "",
                    ProductName = d.Product?.Name ?? "",
                    Quantity = d.Quantity,
                    TaxPercent = d.TaxPercent,
                    TaxValue = d.TaxValue,
                    UnitPrice = d.UnitPrice,
                    CostOfGoodsSold = d.CostOfGoodsSold,
                }).ToList() ?? []
            };
            return result;
        }
        public async Task<long> FindId(int invoiceNumber, BaseInfoes.InvoiceTypes invoiceType, int companyId, int financialPeriodId, CancellationToken cancellationToken)
        {
            if (invoiceNumber < 1)
            {
                throw new BadRequestException("شماره فاکتور معتبر نیست.");
            }
            if (financialPeriodId < 1)
            {
                throw new BadRequestException("دوره مالی معتبر نیست.");
            }

            long invoiceId = await _invoiceRepo.QueryNoTracking
                .Where(i => i.CompanyId == companyId &&
                i.FinancialPeriodId == financialPeriodId &&
                i.InvoiceNumber == invoiceNumber &&
                i.InvoiceType == invoiceType)
                .Select(i => i.Id).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
            if (invoiceId < 1)
                throw new BadRequestException("اطلاعات درخواستی در سیستم وجود ندارد.");
            return invoiceId;
        }
        public async Task<InvoiceDto[]> FindAll(InvoiceSearchFiltersDto dto, CancellationToken cancellationToken)
        {
            var qry = _invoiceRepo.QueryNoTracking.Where(i => i.CompanyId == dto.CompanyId);
            qry = DynamicFilterHelper.ApplyFilters(qry, dto);
            List<Invoice> invoices = await qry
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
            InvoiceDto[] result = invoices.Select(i => new InvoiceDto()
            {
                ClientUserName = i.ClientUserName,
                CreatedById = i.CreatedById,
                CreationDate = i.CreationDate,
                CustomerId = i.CustomerId,
                FinancialPeriodId = i.FinancialPeriodId,
                Id = i.Id,
                InvoiceDate = i.InvoiceDate,
                InvoiceDescription = i.InvoiceDescription,
                InvoiceNumber = i.InvoiceNumber,
                InvoiceState = i.InvoiceState,
                InvoiceType = i.InvoiceType,
                UpdateDate = i.UpdateDate,
                UpdatedById = i.UpdatedById,
            }).ToArray();
            return result;
        }

        public async Task<long> Add(InvoiceDto dto, int companyId, CancellationToken cancellationToken)
        {
            if (companyId < 1)
            {
                throw new BadRequestException("شرکت درخواست دهنده معتبر نیست.");
            }
            bool exists = await _invoiceRepo.QueryNoTracking.AnyAsync(i => i.InvoiceNumber == dto.InvoiceNumber && i.CompanyId == companyId && i.InvoiceType == dto.InvoiceType, cancellationToken);
            if (exists)
            {
                throw new BadRequestException($"{BaseInfoes.PInvoiceTypes[dto.InvoiceType]} دیگری با  شماره {dto.InvoiceNumber} در سیستم وجود دارد.");
            }
            bool fpIdIsValid = await _financialPeriodRepo.QueryNoTracking.AnyAsync(f => f.Id == dto.FinancialPeriodId, cancellationToken);
            if (!fpIdIsValid)
            {
                throw new BadRequestException("دوره مالی ارسالی معتبر نیست.");
            }
            var products = await _productRepo.QueryNoTracking.Where(p => p.CompanyId == companyId)
                                .Select(p => p.Id).ToListAsync(cancellationToken);
            var pIdHash = new HashSet<int>(products);
            IEnumerable<int> invalidProducts = dto.InvoiceDetails.Where(d => !pIdHash.Contains(d.ProductId))
                                                                    .Select(d => d.ProductId);
            if (invalidProducts.Any())
            {
                string msg = string.Join(',', invalidProducts);
                throw new BadRequestException($"کالاهای زیر در سیستم وجود ندارد: {msg}");
            }

            Invoice invoice = new()
            {
                CreationDate = DateTimeOffset.UtcNow,
                ClientUserName = dto.ClientUserName,
                CompanyId = companyId,
                CreatedById = dto.CreatedById,
                CustomerId = dto.CustomerId,
                DiscountValue = dto.DiscountValue,
                ExtraCosts = dto.ExtraCosts,
                FinancialPeriodId = dto.FinancialPeriodId,
                InvoiceDate = dto.InvoiceDate,
                InvoiceDescription = dto.InvoiceDescription,
                InvoiceType = dto.InvoiceType,
                Id = 0,
                InvoiceNetPrice = dto.InvoiceNetPrice,
                InvoiceNumber = dto.InvoiceNumber,
                InvoiceState = BaseInfoes.InvoiceStates.Draft,
                TaxValue = dto.TaxValue,

                InvoiceDetails = dto.InvoiceDetails.Select(d => new InvoiceDetail()
                {
                    Currency = d.Currency,
                    DetailDescription = d.DetailDescription,
                    DiscountPercent = d.DiscountPercent,
                    DiscountValue = d.DiscountValue,
                    Id = 0,
                    ProductId = d.ProductId,
                    Quantity = d.Quantity,
                    TaxPercent = d.TaxPercent,
                    TaxValue = d.TaxValue,
                    UnitPrice = d.UnitPrice,
                    CostOfGoodsSold = d.CostOfGoodsSold,
                }).ToList()
            };
            await _invoiceRepo.AddAsync(invoice, cancellationToken);
            return invoice.Id;


        }
        public async Task<List<(long Id, int InvoiceNumber, BaseInfoes.InvoiceTypes InvoiceType)>> Add(List<InvoiceDto> dto, int companyId, int userId, CancellationToken cancellationToken)
        {

            if (companyId < 1)
            {
                throw new BadRequestException("شرکت درخواست دهنده معتبر نیست.");
            }
            var dupRows = dto.GroupBy(d => new { d.InvoiceType, d.InvoiceNumber }).Select(d => new { Count = d.Count(), d.Key.InvoiceNumber, d.Key.InvoiceType }).Where(i => i.Count > 1);
            if (dupRows.Any())
            {
                string msg = string.Join(',', dupRows.Select(d => BaseInfoes.PInvoiceTypes[d.InvoiceType] + "-" + d.InvoiceNumber));
                throw new Exception($"ردیفهای زیر تکراری است: {msg}");
            }
            var allInvoices = await _invoiceRepo.QueryNoTracking
                                                .Where(i => i.CompanyId == companyId)
                                                .Select(i => new { i.InvoiceNumber, i.InvoiceType })
                                                .ToListAsync(cancellationToken);
            var dbSet = new HashSet<(int InvoiceNumber, BaseInfoes.InvoiceTypes InvoiceType)>(
                        allInvoices.Select(i => (i.InvoiceNumber, i.InvoiceType)),
                        InvoiceTupleComparer.Instance
                        );
            var duplicates = dto.Where(i => dbSet.Contains((i.InvoiceNumber, i.InvoiceType)));
            if (duplicates.Any())
            {
                string msg = string.Join(',', duplicates.Select(d => BaseInfoes.PInvoiceTypes[d.InvoiceType] + "-" + d.InvoiceNumber));
                throw new BadRequestException($"فاکتورهای زیر پیش از این ثبت شده اند: {msg}");
            }
            var fpIds = dto.Select(d => d.FinancialPeriodId).Distinct().ToList();
            var dbFpIDs = await _financialPeriodRepo.QueryNoTracking.Select(f => f.Id).ToListAsync(cancellationToken);
            bool fpIdINotsValid = fpIds.Any(f => !dbFpIDs.Contains(f));
            if (fpIdINotsValid)
            {
                throw new BadRequestException("دوره مالی ارسالی در برخی ردیفها معتبر نیست.");
            }
            var products = await _productRepo.QueryNoTracking.Where(p => p.CompanyId == companyId)
                                .Select(p => p.Id).ToListAsync(cancellationToken);
            var pIdHash = new HashSet<int>(products);
            IEnumerable<int> invalidProducts = dto.SelectMany(d => d.InvoiceDetails).Where(d => !pIdHash.Contains(d.ProductId))
                                                                    .Select(d => d.ProductId);
            if (invalidProducts.Any())
            {
                string msg = string.Join(',', invalidProducts);
                throw new BadRequestException($"کالاهای زیر در سیستم وجود ندارد: {msg}");
            }
            List<Invoice> invoices = dto.Select(i => new Invoice()
            {
                CreationDate = DateTimeOffset.UtcNow,
                ClientUserName = i.ClientUserName,
                CompanyId = companyId,
                InvoiceNumber = i.InvoiceNumber,
                InvoiceType = i.InvoiceType,
                CreatedById = userId,
                CustomerId = i.CustomerId,
                FinancialPeriodId = i.FinancialPeriodId,
                Id = i.Id,
                InvoiceDate = i.InvoiceDate,
                InvoiceDescription = i.InvoiceDescription,
                InvoiceState = BaseInfoes.InvoiceStates.Draft,
                InvoiceDetails = i.InvoiceDetails.Select(d => new InvoiceDetail()
                {
                    Id = 0,
                    Currency = d.Currency,
                    DetailDescription = d.DetailDescription,
                    DiscountPercent = d.DiscountPercent,
                    DiscountValue = d.DiscountValue,
                    ProductId = d.ProductId,
                    Quantity = d.Quantity,
                    TaxPercent = d.TaxPercent,
                    TaxValue = d.TaxValue,
                    UnitPrice = d.UnitPrice,
                    CostOfGoodsSold = d.CostOfGoodsSold,
                }).ToList()

            }).ToList();
            var conn = (SqlConnection)_invoiceRepo.DbConnection;
            if (conn.State != System.Data.ConnectionState.Open)
                conn.Open();
            using var tr = (SqlTransaction)(await conn.BeginTransactionAsync(cancellationToken));
            try
            {
                await _invoiceRepo.BulkInsertWithOutputIdsAsync(invoices, cancellationToken, tr);
                foreach (var i in invoices)
                {
                    foreach (var d in i.InvoiceDetails)
                    {
                        d.InvoiceId = i.Id;
                    }
                }
                var details = invoices.SelectMany(i => i.InvoiceDetails);
                await _invoiceDetailRepo.BulkInsertAsync(details, tr, cancellationToken);
                tr.Commit();
            }
            catch
            {
                await tr.RollbackAsync(cancellationToken);
                throw;
            }
            var result = invoices.Select(i => (i.Id, i.InvoiceNumber, i.InvoiceType)).ToList();
            return result;


        }
        public async Task<long> Edit(InvoiceDto dto, int companyId, CancellationToken cancellationToken)
        {

            if (dto.Id < 1)
            {
                throw new BadRequestException("شناسه ارسالی معتبر نیست.");
            }
            Invoice? invoice = await _invoiceRepo.Query.Include(i => i.InvoiceDetails)
                .FirstOrDefaultAsync(i => i.Id == dto.Id && i.CompanyId == companyId && i.InvoiceType == dto.InvoiceType, cancellationToken);
            if (invoice == null)
            {
                throw new BadRequestException("اطلاعات درخواستی در سیستم وجود ندارد.");
            }
            bool exists = await _invoiceRepo.QueryNoTracking.AnyAsync(i => i.Id != dto.Id && i.InvoiceNumber == dto.InvoiceNumber && i.CompanyId == companyId && i.InvoiceType == dto.InvoiceType, cancellationToken);
            if (exists)
            {
                throw new BadRequestException("فاکتور دیگری با این شماره در سیستم وجود دارد.");
            }
            var products = await _productRepo.QueryNoTracking.Where(p => p.CompanyId == companyId)
                                .Select(p => p.Id).ToListAsync(cancellationToken);
            var pIdHash = new HashSet<int>(products);
            IEnumerable<int> invalidProducts = dto.InvoiceDetails.Where(d => !pIdHash.Contains(d.ProductId))
                                                                    .Select(d => d.ProductId);
            if (invalidProducts.Any())
            {
                string msg = string.Join(',', invalidProducts);
                throw new BadRequestException($"کالاهای زیر در سیستم وجود ندارد: {msg}");
            }
            {
                invoice.ClientUserName = dto.ClientUserName;
                invoice.UpdatedById = dto.UpdatedById;
                invoice.UpdateDate = DateTimeOffset.UtcNow;
                invoice.CustomerId = dto.CustomerId;
                invoice.InvoiceDate = dto.InvoiceDate;
                invoice.InvoiceDescription = dto.InvoiceDescription;
                invoice.InvoiceNumber = dto.InvoiceNumber;
                // فاکتوری که ویرایش می شود از اعتبار ساقط می شود.
                // این فاکتور بروزرسانی دوباره معتبر خواهد شد.
                invoice.InvoiceState = BaseInfoes.InvoiceStates.Draft;
                if (invoice.InvoiceDetails != null)
                    await _invoiceDetailRepo.DeleteRangeAsync(invoice.InvoiceDetails, cancellationToken, false);
                foreach (var d in dto.InvoiceDetails)
                {
                    invoice.InvoiceDetails?.Add(new InvoiceDetail()
                    {
                        Currency = d.Currency,
                        DetailDescription = d.DetailDescription,
                        DiscountPercent = d.DiscountPercent,
                        DiscountValue = d.DiscountValue,
                        Id = 0,
                        ProductId = d.ProductId,
                        Quantity = d.Quantity,
                        TaxPercent = d.TaxPercent,
                        TaxValue = d.TaxValue,
                        UnitPrice = d.UnitPrice,
                        CostOfGoodsSold = d.CostOfGoodsSold,
                    });
                }
            };
            await _invoiceRepo.UpdateAsync(invoice, cancellationToken);
            return invoice.Id;


        }

        public async Task Remove(long id, int companyId, CancellationToken cancellationToken)
        {
            var invoice = await _invoiceRepo.Query.FirstOrDefaultAsync(i => i.Id == id && i.CompanyId == companyId, cancellationToken);
            if (invoice == null)
            {
                throw new BadRequestException("اطلاعات درخواستی یافت نشد.");
            }
            invoice.SignedForDelete = true;
            await _invoiceRepo.UpdateAsync(invoice, cancellationToken);
        }
        /// <summary>
        /// فاکتورهای تمام شرکت ها باید بروزرسانی شود
        /// </summary>
        public async Task UpdateAllInvoices(int userId, CancellationToken cancellationToken)
        {

            var deletedInvoices = _invoiceRepo.QueryNoTracking.Where(i => i.SignedForDelete);
            await deletedInvoices.ExecuteDeleteAsync(cancellationToken);
            var updatingInvoices = _invoiceRepo.QueryNoTracking.Where(i => i.InvoiceState == BaseInfoes.InvoiceStates.Draft);
            await updatingInvoices.ExecuteUpdateAsync(d => d.SetProperty(d => d.InvoiceState, d => BaseInfoes.InvoiceStates.Approved), cancellationToken);
            var qrySetting = _settingRepo.QueryNoTracking.Where(s => s.SettingName == "LastUpdateDate");
            var setting = await _settingRepo.Query.FirstAsync(s => s.SettingName == "LastUpdateDate", cancellationToken);
            setting.SettingValue = DateTimeOffset.UtcNow.ToString("o", CultureInfo.InvariantCulture);
            await _settingRepo.UpdateAsync(setting, cancellationToken);
        }

        public async Task<Dictionary<int, (DateTimeOffset MaxCreationDate, DateTimeOffset MaxInvoiceDate)>> GetMaxInvoiceDates(CancellationToken cancellationToken)
        {
            var result = await _invoiceRepo.QueryNoTracking
        .GroupBy(i => i.CompanyId)
        .Select(g => new
        {
            CompanyId = g.Key,
            MaxCreationDate = g.Max(x => x.CreationDate),
            MaxInvoiceDate = g.Max(x => x.InvoiceDate)
        })
        .ToDictionaryAsync(
            x => x.CompanyId,
            x => (x.MaxCreationDate, x.MaxInvoiceDate),
            cancellationToken
        )
        .ConfigureAwait(false);
            return result;
        }

        private class InvoiceTupleComparer : IEqualityComparer<(int InvoiceNumber, BaseInfoes.InvoiceTypes InvoiceType)>
        {
            public static readonly InvoiceTupleComparer Instance = new();

            public bool Equals((int InvoiceNumber, BaseInfoes.InvoiceTypes InvoiceType) x, (int InvoiceNumber, BaseInfoes.InvoiceTypes InvoiceType) y)
            {
                return x.InvoiceNumber == y.InvoiceNumber && x.InvoiceType == y.InvoiceType;
            }

            public int GetHashCode((int InvoiceNumber, BaseInfoes.InvoiceTypes InvoiceType) obj)
            {
                unchecked
                {
                    int h = 17;
                    h = h * 23 + StringComparer.OrdinalIgnoreCase.GetHashCode(obj.InvoiceNumber);
                    h = h * 23 + StringComparer.OrdinalIgnoreCase.GetHashCode(obj.InvoiceType);
                    return h;
                }
            }
        }
    }
}
