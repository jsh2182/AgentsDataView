using AgentsDataView.Data.Contracts;
using AgentsDataView.Entities;
using AgentsDataView.Entities.DtoModels;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using static AgentsDataView.Entities.BaseInfoes;
using static AgentsDataView.Entities.BaseInfoes.InvoiceTypes;

namespace AgentsDataView.Services
{
    public class ReportDataService(IRepository<Invoice> invoiceRepo,
        IRepository<Company> companyRepo,
        IRepository<InvoiceDetail> invoiceDetailsRepo,
        IInvoiceService invoiceService) : IReportDataService
    {
        private readonly IRepository<Invoice> _invoiceRepo = invoiceRepo;
        private readonly IRepository<Company> _companyRepo = companyRepo;
        private readonly IRepository<InvoiceDetail> _invoiceDetailsRepo = invoiceDetailsRepo;
        private readonly IInvoiceService _invoiceService = invoiceService;
        private readonly InvoiceTypes[] _inputTypes =
        [
            AdvancedProductionReceipt,
            SalesReturn,
            Purchase,
            GoodsReceipt,
            AdvancedProductionCompletionReceipt,
            WIPCompletionReceipt,
            BeginingInventoryReceipt,
            InventoryAdjustment_Increase
        ];

        private readonly InvoiceTypes[] _outputTypes =
        [
            Sales,
            PurchaseReturn,
            GoodsIssue,
            InventoryAdjustment_Decrease
        ];
        public async Task<ReportResultDto[]> GetReportByProvince(int[]? provinceIds, CancellationToken cancellationToken)
        {

            provinceIds ??= [];

            // ------------------------------
            // کوئری کشوری
            // ------------------------------
            DateTime dateFrom = new(2025, 3, 21);
            DateTime dateTo = new(2026, 3, 20);
            var inputCountryQuery = _invoiceDetailsRepo.QueryNoTracking
                .Where(d =>
                d.Invoice.InvoiceState == InvoiceStates.Approved &&
                d.Invoice.InvoiceDate >= dateFrom &&
                d.Invoice.InvoiceDate <= dateTo &&
                _inputTypes.Contains(d.Invoice.InvoiceType) &&
                d.Invoice.Company.ProvinceId != null)
                .GroupBy(d => new { d.Product.Code, d.Product.Name })
                .Select(g => new ProductAggregationDto
                {
                    Code = g.Key.Code,
                    Name = g.Key.Name,
                    InputCount = g.Sum(d => d.Quantity),
                    InputTotalPrice = g.Sum(d => d.TotalPrice),
                    OutputCount = 0,
                    OutputTotalPrice = 0
                });

            var outputCountryQuery = _invoiceDetailsRepo.QueryNoTracking
                .Where(d =>
                d.Invoice.InvoiceState == InvoiceStates.Approved &&
                _outputTypes.Contains(d.Invoice.InvoiceType) &&
                d.Invoice.Company.ProvinceId != null)
                .GroupBy(d => new { d.Product.Code, d.Product.Name })
                .Select(g => new ProductAggregationDto
                {
                    Code = g.Key.Code,
                    Name = g.Key.Name,
                    InputCount = 0,
                    InputTotalPrice = 0,
                    OutputCount = g.Sum(d => d.Quantity),
                    OutputTotalPrice = g.Sum(d => d.TotalPrice)
                });

            var countryGrouped = await inputCountryQuery
                .Concat(outputCountryQuery)
                .GroupBy(x => new { x.Code, x.Name })
                .Select(g => new ProductAggregationDto
                {
                    Code = g.Key.Code,
                    Name = g.Key.Name,
                    InputCount = g.Sum(x => x.InputCount),
                    InputTotalPrice = g.Sum(x => x.InputTotalPrice),
                    OutputCount = g.Sum(x => x.OutputCount),
                    OutputTotalPrice = g.Sum(x => x.OutputTotalPrice)
                })
                .ToArrayAsync(cancellationToken)
                .ConfigureAwait(false);

            // ------------------------------
            // کوئری استانی (در صورت وجود ProvinceId)
            // ------------------------------
            ProductAggregationDto[] provinceGrouped = [];

            if (provinceIds.Length > 0)
            {
                var inputProvinceQuery = _invoiceDetailsRepo.QueryNoTracking
                    .Where(d =>
                    d.Invoice.InvoiceState == InvoiceStates.Approved &&
                    provinceIds.Contains(d.Invoice.Company.ProvinceId.Value) &&
                    _inputTypes.Contains(d.Invoice.InvoiceType))
                    .GroupBy(d => new { d.Product.Code, d.Product.Name })
                    .Select(g => new ProductAggregationDto
                    {
                        Code = g.Key.Code,
                        Name = g.Key.Name,
                        InputCount = g.Sum(d => d.Quantity),
                        InputTotalPrice = g.Sum(d => d.TotalPrice),
                        OutputCount = 0,
                        OutputTotalPrice = 0
                    });

                var outputProvinceQuery = _invoiceDetailsRepo.QueryNoTracking
                    .Where(d =>
                    d.Invoice.InvoiceState == InvoiceStates.Approved &&
                    provinceIds.Contains(d.Invoice.Company.ProvinceId.Value) &&
                    _outputTypes.Contains(d.Invoice.InvoiceType))
                    .GroupBy(d => new { d.Product.Code, d.Product.Name })
                    .Select(g => new ProductAggregationDto
                    {
                        Code = g.Key.Code,
                        Name = g.Key.Name,
                        InputCount = 0,
                        InputTotalPrice = 0,
                        OutputCount = g.Sum(d => d.Quantity),
                        OutputTotalPrice = g.Sum(d => d.TotalPrice)
                    });

                provinceGrouped = await inputProvinceQuery
                    .Concat(outputProvinceQuery)
                    .GroupBy(x => new { x.Code, x.Name })
                    .Select(g => new ProductAggregationDto
                    {
                        Code = g.Key.Code,
                        Name = g.Key.Name,
                        InputCount = g.Sum(x => x.InputCount),
                        InputTotalPrice = g.Sum(x => x.InputTotalPrice),
                        OutputCount = g.Sum(x => x.OutputCount),
                        OutputTotalPrice = g.Sum(x => x.OutputTotalPrice)
                    })
                    .ToArrayAsync(cancellationToken)
                    .ConfigureAwait(false);
            }

            // ------------------------------
            // JOIN کشوری + استانی
            // ------------------------------
            var result =
                from c in countryGrouped
                join p in provinceGrouped
                    on new { c.Code, c.Name } equals new { p.Code, p.Name }
                    into gp
                from province in gp.DefaultIfEmpty()
                select new ReportResultDto
                {
                    ProductCode = c.Code,
                    ProductName = c.Name,

                    CountryInputCount = Math.Round(c.InputCount),
                    CountryInputTotal = Math.Round(c.InputTotalPrice),
                    CountryOutputCount = Math.Round(c.OutputCount),
                    CountryOutputTotal = Math.Round(c.OutputTotalPrice),

                    ProvinceInputCount = Math.Round(province?.InputCount ?? 0),
                    ProvinceInputTotal = Math.Round(province?.InputTotalPrice ?? 0),
                    ProvinceOutputCount = Math.Round(province?.OutputCount ?? 0),
                    ProvinceOutputTotal = Math.Round(province?.OutputTotalPrice ?? 0)
                };

            return [.. result.OrderBy(p => p.ProductName)];
        }

        public async Task<IEnumerable> GetReportByProvince_Cumulative(int[]? provinceIds,CancellationToken cancellationToken)
        {
            provinceIds ??= [];
            DateTime dateFrom = new(2025, 3, 21);
            DateTime dateTo = new(2026, 3, 20);
            var inputQuery = _invoiceDetailsRepo.QueryNoTracking
                .Where(d =>
                d.Invoice.InvoiceState == InvoiceStates.Approved &&
                d.Invoice.InvoiceDate >= dateFrom &&
                            d.Invoice.InvoiceDate <= dateTo &&
                _inputTypes.Contains(d.Invoice.InvoiceType) && 
                d.Invoice.Company.ProvinceId != null // && 
                //(provinceIds.Length ==0 || provinceIds.Contains(d.Invoice.Company.ProvinceId.Value))
                )
                .GroupBy(d => d.Invoice.Company.Province.ProvinceName)
                .Select(g => new ReportResultDto
                {
                    ProvinceName = g.Key,
                    ProvinceInputCount = g.Sum(d => d.Quantity),
                    ProvinceInputTotal = g.Sum(d => d.TotalPrice),
                    ProvinceOutputCount = 0,
                    ProvinceOutputTotal = 0
                });

            var outputQuery = _invoiceDetailsRepo.QueryNoTracking
                .Where(d =>
                d.Invoice.InvoiceState == InvoiceStates.Approved &&
                _outputTypes.Contains(d.Invoice.InvoiceType) && d.Invoice.Company.ProvinceId != null)
                .GroupBy(d => d.Invoice.Company.Province.ProvinceName)
                .Select(g => new ReportResultDto
                {
                    ProvinceName = g.Key,
                    ProvinceInputCount = 0,
                    ProvinceInputTotal = 0,
                    ProvinceOutputCount = g.Sum(d => d.Quantity),
                    ProvinceOutputTotal = g.Sum(d => d.TotalPrice)
                });

            List<ReportResultDto> result = await inputQuery
                .Concat(outputQuery)
                .GroupBy(x => x.ProvinceName)
                .Select(g => new ReportResultDto
                {
                    ProvinceName = g.Key,
                    ProvinceInputCount = Math.Round(g.Sum(x => x.ProvinceInputCount)),
                    ProvinceInputTotal = Math.Round(g.Sum(x => x.ProvinceInputTotal)),
                    ProvinceOutputCount = Math.Round(g.Sum(x => x.ProvinceOutputCount)),
                    ProvinceOutputTotal = Math.Round(g.Sum(x => x.ProvinceOutputTotal))
                })
                .OrderBy(g => g.ProvinceName)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
            var existingProvinces = result.Select(r => r.ProvinceName).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var provinceList = await _companyRepo.QueryNoTracking.Where(c => c.ProvinceId != null).Select(c => c.Province.ProvinceName).ToListAsync(cancellationToken);
            foreach (var p in provinceList)
            {
                if (!existingProvinces.Contains(p))
                {
                    result.Add(new ReportResultDto()
                    {
                        ProvinceName = p
                    });
                }
            }

            return result.OrderBy(r => r.ProvinceName);
        }

        public async Task<IEnumerable> GetReportByCompanyAndProduct(int[]? provinceIds, CancellationToken cancellationToken)
        {

            provinceIds ??= [];

            // -----------------------------
            // Input Query
            // -----------------------------

            DateTime dateFrom = new(2025, 3, 21);
            DateTime dateTo = new(2026, 3, 20);
            var inputQuery = _invoiceDetailsRepo.QueryNoTracking
                .Where(d => d.Invoice.InvoiceState == InvoiceStates.Approved &&
                            d.Invoice.InvoiceDate >= dateFrom &&
                            d.Invoice.InvoiceDate <= dateTo &&
                            _inputTypes.Contains(d.Invoice.InvoiceType) &&
                            d.Invoice.Company.ProvinceId != null &&
                            (provinceIds.Length == 0 || provinceIds.Contains(d.Invoice.Company.ProvinceId.Value)))
                .GroupBy(d => new
                {
                    CompanyName = d.Invoice.Company.Name,
                    d.Product.Code,
                    d.Product.Name
                })
                .Select(g => new ProductAggregationDto
                {
                    CompanyName = g.Key.CompanyName,
                    Code = g.Key.Code,
                    Name = g.Key.Name,
                    InputCount = g.Sum(d => d.Quantity),
                    InputTotalPrice = g.Sum(d => d.TotalPrice),
                    OutputCount = 0,
                    OutputTotalPrice = 0
                });

            // -----------------------------
            // Output Query
            // -----------------------------
            var outputQuery = _invoiceDetailsRepo.QueryNoTracking
                .Where(d => d.Invoice.InvoiceState == InvoiceStates.Approved &&
                            _outputTypes.Contains(d.Invoice.InvoiceType) &&
                            d.Invoice.Company.ProvinceId != null &&
                            (provinceIds.Length == 0 || provinceIds.Contains(d.Invoice.Company.ProvinceId.Value)))
                .GroupBy(d => new
                {
                    CompanyName = d.Invoice.Company.Name,
                    d.Product.Code,
                    d.Product.Name
                })
                .Select(g => new ProductAggregationDto
                {
                    CompanyName = g.Key.CompanyName,
                    Code = g.Key.Code,
                    Name = g.Key.Name,
                    InputCount = 0,
                    InputTotalPrice = 0,
                    OutputCount = g.Sum(d => d.Quantity),
                    OutputTotalPrice = g.Sum(d => d.TotalPrice)
                });

            // -----------------------------
            // ترکیب Input و Output و GroupBy نهایی
            // -----------------------------
            var combinedQuery = inputQuery
                .Concat(outputQuery)
                .GroupBy(x => new { x.CompanyName, x.Code, x.Name })
                .Select(g => new ProductAggregationDto
                {
                    CompanyName = g.Key.CompanyName,
                    Code = g.Key.Code,
                    Name = g.Key.Name,
                    InputCount = g.Sum(x => x.InputCount),
                    InputTotalPrice = g.Sum(x => x.InputTotalPrice),
                    OutputCount = g.Sum(x => x.OutputCount),
                    OutputTotalPrice = g.Sum(x => x.OutputTotalPrice)
                });

            // -----------------------------
            // GroupBy دوم: هر شرکت + آرایه محصولات
            // -----------------------------
            var firstResult = await combinedQuery.ToArrayAsync(cancellationToken).ConfigureAwait(false);
            var result = firstResult
                .GroupBy(x => new { x.CompanyName })
                .Select(g => new
                {
                    g.Key.CompanyName,
                    Products = g.Select(p => new
                    {
                        ProductCode = p.Code,
                        ProductName = p.Name,
                        InputCount = Math.Round(p.InputCount),
                        InputTotalPrice = Math.Round(p.InputTotalPrice),
                        OutputCount = Math.Round(p.OutputCount),
                        OutputTotalPrice = Math.Round(p.OutputTotalPrice)
                    }).ToArray()
                })
                .OrderBy(g => g.CompanyName)
                .ToArray();

            return result;
        }


        public async Task<ReportResultDto[]> GetProfitReportByCompany(int[]? provinceIds, CancellationToken cancellationToken)
        {
            provinceIds ??= [];
            DateTime dateFrom = new(2025, 3, 21);
            DateTime dateTo = new(2026, 3, 20);
            var result = await _invoiceDetailsRepo.QueryNoTracking
                .Where(d => d.Invoice.InvoiceType == Sales &&
                            d.Invoice.InvoiceDate >= dateFrom &&
                            d.Invoice.InvoiceDate <= dateTo &&
                d.Invoice.InvoiceState == InvoiceStates.Approved &&
                d.Invoice.Company.ProvinceId != null &&
                (!(provinceIds.Length > 0)  || provinceIds.Contains(d.Invoice.Company.ProvinceId??0)))
                .GroupBy(d => new { CompanyCode = d.Invoice.Company.Code, CompanyName = d.Invoice.Company.Name, d.Invoice.CompanyId })
                .Select(g => new ReportResultDto
                {
                    CompanyCode = g.Key.CompanyCode,
                    CompanyName = g.Key.CompanyName,
                    CompanyId = g.Key.CompanyId,
                    CompanyOutputCount = Math.Round(g.Sum(x => x.Quantity)),
                    CompanyOutputTotal = Math.Round(g.Sum(x => x.TotalPrice)),
                    CostOfGoodsSold = Math.Round(g.Sum(x => x.Quantity * x.CostOfGoodsSold)),
                    ProfitLoss = Math.Round(g.Sum(x => x.TotalPrice - x.Quantity * x.CostOfGoodsSold))
                }).OrderBy(r => r.CompanyCode).ToListAsync(cancellationToken).ConfigureAwait(false);

            var invoiceDiscounts = await _invoiceRepo.QueryNoTracking
                  .Where(i => i.InvoiceType == Sales &&
                  i.InvoiceState == InvoiceStates.Approved &&
                  i.Company.ProvinceId != null &&
                  (!(provinceIds.Length > 0) || provinceIds.Contains( i.Company.ProvinceId??0)))
                  .GroupBy(i => i.CompanyId)
                  .Select(g => new
                  {
                      CompanyId = g.Key,
                      Discount = g.Sum(gg => gg.DiscountValue)
                  }).ToDictionaryAsync(i => i.CompanyId, i => i.Discount, cancellationToken).ConfigureAwait(false);

            var invDict = await _invoiceService.GetMaxInvoiceDates(cancellationToken);

            foreach (var item in result)
            {
                invoiceDiscounts.TryGetValue(item.CompanyId, out decimal discount);
                item.ProfitLoss -= discount;
                item.ProfitLossPercent = Math.Round((item.ProfitLoss / item.CompanyOutputTotal) * 100, 2);
                invDict.TryGetValue(item.CompanyId, out var inv);
                item.CompanyMaxInvoiceCreationDate = inv.MaxCreationDate;
                item.CompanyMaxInvoiceDate = inv.MaxInvoiceDate;
            }
            HashSet<int> existingComps = result.Select(r => r.CompanyId).ToHashSet(); ;
            if (provinceIds.Length == 0 || provinceIds[0] != -1)
            {
                
                var allComps = _companyRepo.QueryNoTracking.Where(c => c.ProvinceId != null && (provinceIds.Length ==0 || provinceIds.Contains(c.ProvinceId.Value))).Select(c => new { c.Name, c.Id, c.Code });
                foreach (var comp in allComps)
                {
                    if (!existingComps.Contains(comp.Id))
                    {
                        result.Add(new ReportResultDto()
                        {
                            CompanyId = comp.Id,
                            CompanyCode = comp.Code,
                            CompanyName = comp.Name
                        });
                        existingComps.Add(comp.Id);
                    }
                }
            }
            return [.. result.OrderBy(c => c.CompanyCode)];
        }
        public async Task<ReportResultDto[]> GetProfitReportByProvince(int[]? provinceIds, CancellationToken cancellationToken)
        {
            provinceIds ??= [];
            // Query کشوری
            DateTime dateFrom = new(2025, 3, 21);
            DateTime dateTo = new(2026, 3, 20);
            var countryQuery = _invoiceDetailsRepo.QueryNoTracking
                .Where(d =>
                d.Invoice.InvoiceDate >= dateFrom &&
                d.Invoice.InvoiceDate <= dateTo &&
                d.Invoice.InvoiceState == InvoiceStates.Approved &&
                d.Invoice.InvoiceType == Sales &&
                d.Invoice.Company.ProvinceId != null)
                .GroupBy(d => 1)
                .Select(g => new ReportResultDto
                {
                    ProvinceId = 0,
                    ProvinceName = "تجمیع کشوری",
                    ProvinceOutputCount = Math.Round(g.Sum(x => x.Quantity)),
                    ProvinceOutputTotal = Math.Round(g.Sum(x => x.TotalPrice)),
                    CostOfGoodsSold = Math.Round(g.Sum(x => x.Quantity * x.CostOfGoodsSold)),
                    ProfitLoss = Math.Round(g.Sum(x => x.TotalPrice - x.Quantity * x.CostOfGoodsSold))
                });

            // Query استانی
            var provinceQuery = _invoiceDetailsRepo.QueryNoTracking
                .Where(d =>
                d.Invoice.InvoiceState == InvoiceStates.Approved &&
                d.Invoice.InvoiceType == Sales &&
                d.Invoice.Company.ProvinceId != null &&
                (provinceIds.Length == 0 || provinceIds.Contains( d.Invoice.Company.ProvinceId.Value))
                )
                .GroupBy(d => new { d.Invoice.Company.Province.ProvinceName, d.Invoice.Company.ProvinceId })
                .Select(g => new ReportResultDto
                {
                    ProvinceId = g.Key.ProvinceId,
                    ProvinceName = g.Key.ProvinceName,
                    ProvinceOutputCount = Math.Round(g.Sum(x => x.Quantity)),
                    ProvinceOutputTotal = Math.Round(g.Sum(x => x.TotalPrice)),
                    CostOfGoodsSold = Math.Round(g.Sum(x => x.Quantity * x.CostOfGoodsSold)),
                    ProfitLoss = Math.Round(g.Sum(x => x.TotalPrice - x.Quantity * x.CostOfGoodsSold))
                });

            // ترکیب دو query و اجرای نهایی
            var result = await countryQuery
                .Concat(provinceQuery)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
            var invoiceDiscounts = await _invoiceRepo.QueryNoTracking
                .Where(i =>
                i.InvoiceState == InvoiceStates.Approved &&
                i.InvoiceType == Sales &&
                i.Company.ProvinceId != null &&
                (provinceIds.Length == 0 || provinceIds.Contains(i.Company.ProvinceId.Value)))
                .GroupBy(g => g.Company.ProvinceId.Value)
                .Select(g => new
                {
                    ProvinceId = g.Key,
                    Discount = g.Sum(gg => gg.DiscountValue)
                }).ToDictionaryAsync(i => i.ProvinceId, i => i.Discount, cancellationToken).ConfigureAwait(false);

            var existingProvinces = result.Select(r => r.ProvinceId).ToHashSet();
            var provinceList = await _companyRepo.QueryNoTracking.Where(c => c.ProvinceId != null).Select(c => new { c.ProvinceId, c.Province.ProvinceName }).ToListAsync(cancellationToken);
            foreach (var p in provinceList)
            {
                if (!existingProvinces.Contains(p.ProvinceId))
                {
                    result.Add(new ReportResultDto()
                    {
                        ProvinceName = p.ProvinceName,
                        ProvinceId = p.ProvinceId
                    });
                    existingProvinces.Add(p.ProvinceId);
                }
            }
            foreach (var item in result)
            {

                invoiceDiscounts.TryGetValue(item.ProvinceId.Value, out decimal discount);
                item.ProfitLoss -= discount;
                if (item.ProvinceOutputTotal == 0)
                {
                    continue;
                }
                item.ProfitLossPercent = Math.Round((item.ProfitLoss / item.ProvinceOutputTotal) * 100, 2);
            }
            return result.OrderBy(d => d.ProvinceName == "تجمیع کشوری" ? "" : d.ProvinceName).ToArray();
        }



    }
}
