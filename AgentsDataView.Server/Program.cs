using AgentsDataView.Data;
using AgentsDataView.Data.Contracts;
using AgentsDataView.Data.Repositories;
using AgentsDataView.Server.Common;
using AgentsDataView.Server.WebFramework;
using AgentsDataView.Services;
using AgentsDataView.WebFramework.Filters;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Web;
using Swashbuckle.AspNetCore.SwaggerUI;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using AgentsDataView.WebFramework.Middlewares;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.ResponseCompression;
using System.IO.Compression;
using AgentsDataView.Server.Services;

var logger = LogManager.Setup().LoadConfigurationFromFile("nlog.config").GetCurrentClassLogger();
SiteSettings? _siteSettings;
logger.Debug("init main");
try
{


    var builder = WebApplication.CreateBuilder(args);
    _siteSettings = builder.Configuration.GetSection(nameof(SiteSettings)).Get<SiteSettings>();

    #region CORS
    string allowedOrigins = "AllowedOrigins";
    string[]? addressList = _siteSettings?.AllowedCorsOrigins?.Split(",");
    if (addressList?.Length > 0)
    {
        if (addressList[0] == "*")
        {
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();

                });
            });
        }
        else
        {
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: allowedOrigins, policy =>
                {
                    policy.WithOrigins(addressList).AllowAnyMethod().AllowAnyHeader();

                });
            });
        }

    }
    #endregion

    #region ResponseCompression
    builder.Services.AddResponseCompression(options =>
    {
        options.EnableForHttps = true;
        options.Providers.Add<GzipCompressionProvider>();
        //options.Providers.Add<BrotliCompressionProvider>();
    });

    builder.Services.Configure<GzipCompressionProviderOptions>(options =>
    {
        options.Level = CompressionLevel.Fastest;
    });


    #endregion
    builder.Services.Configure<SiteSettings>(builder.Configuration.GetSection(nameof(SiteSettings)));
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
    {

        options.UseSqlServer(builder.Configuration.GetConnectionString("AgentsDataViewConnection"), s => s.MigrationsAssembly("AgentsDataView.Server"));
    });

    // Add services to the container.

    builder.Services.AddControllers(opt =>
    {
        opt.Filters.Add(new AuthorizeFilter());
        opt.Filters.Add(new ApiResultFilterAttribute());
    }).AddNewtonsoftJson(options =>
        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore).ConfigureApiBehaviorOptions(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                List<string> errors = [];
                foreach (var val in context.ModelState.Values)
                {
                    errors.Add(string.Join(',', val.Errors.Select(er => er.ErrorMessage)));
                }
                string errorMessage = string.Join(',', errors);
                var json = JsonConvert.SerializeObject(new { Error = errorMessage });
                return new BadRequestObjectResult(errorMessage);
            };
        });
    builder.Host.UseNLog();
    builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
    //builder.Services.AddScoped(typeof(IUserRepository), typeof(UserRepository));
    //builder.Services.AddScoped(typeof(IReportDataRepository), typeof(ReportDataRepository));
    builder.Services.AddScopedServices(typeof(IRepository<>), typeof(Repository<>));
    builder.Services.AddScoped(typeof(IInvoiceService), typeof(InvoiceService));
    builder.Services.AddScoped(typeof(IReportDataService), typeof(ReportDataService));
    builder.Services.AddSingleton(typeof(IIsUpdatingInvoices), typeof(IsUpdatingInvoices));
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();
    builder.Services.AddScoped<IJwtService, JwtService>();
    if (_siteSettings == null)
    {
        throw new Exception("Site Settings Is Null. Check The Config File");
    }
    builder.Services.AddJwtAuthentication(_siteSettings.JwtSettings);
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(config =>
    {
        var xmlFiles = Directory.GetFiles(AppContext.BaseDirectory, "*.xml", SearchOption.TopDirectoryOnly);
        foreach (var xmlFile in xmlFiles)
        {
            config.IncludeXmlComments(xmlFile, includeControllerXmlComments: true);
            config.SchemaFilter<EnumSchemaFilter>(xmlFile);
        }
        config.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "Bearer",
            Description = "Enter Your Token Without The \"Bearer\" Prefix"
        });
        config.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
    });

    var app = builder.Build();
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Database.Migrate();
    }
    app.UseDefaultFiles();
    app.UseStaticFiles();
    app.UseCustomExceptionHandler();
    app.UseResponseCompression();
    // Configure the HTTP request pipeline.
    //if (app.Environment.IsDevelopment())
    //{
    app.UseSwagger(c =>
    {
        c.RouteTemplate = "api_docs/{documentName}/doc.json";
    });
    app.UseSwaggerUI(config =>
    {
        config.SwaggerEndpoint("/api_docs/v1/doc.json", "AgentsDataView API V1");
        config.InjectStylesheet("/swagger-ui/swagger-custom.css");
        config.InjectJavascript("/swagger-ui/swagger-autoset-token.js");
        config.RoutePrefix = "api_docs";
        config.DocumentTitle = "AgentsDataView";
        config.DocExpansion(DocExpansion.None);
        config.DefaultModelsExpandDepth(-1);
    });
    //}

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.MapFallbackToFile("/index.html");

    app.Run();
}
catch (Exception ex)
{

    logger.Error(ex, "Stopped Program");
    throw;
}
finally
{
    LogManager.Shutdown();
}
