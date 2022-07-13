using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OneMedify.API.Config;
using OneMedify.Infrastructure.Contracts;
using OneMedify.Infrastructure.Data;
using OneMedify.Infrastructure.Repositories;
using OneMedify.Services.Contracts;
using OneMedify.Services.Contracts.DoctorActionLogContracts;
using OneMedify.Services.Contracts.DoctorContracts;
using OneMedify.Services.Contracts.PatientContracts;
using OneMedify.Services.Contracts.PharmacyContracts;
using OneMedify.Services.Contracts.PrescriptionContracts;
using OneMedify.Services.Services;
using OneMedify.Services.Services.BackgroundServices;
using OneMedify.Services.Services.DoctorActionLogServices;
using OneMedify.Services.Services.DoctorServices;
using OneMedify.Services.Services.PatientServices;
using OneMedify.Services.Services.PharmacyServices;
using OneMedify.Services.Services.PrescriptionServices;
using OneMedify.Shared.Contracts;
using OneMedify.Shared.Services;
using PolicyServer;
using System.IO;
using System.Net.Http;

namespace OneMedify.API
{
    public class Startup
    {
        public OneMedifyConfiguration Configuration { get; }

        [System.Obsolete]
        public Startup(Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);
            builder.AddEnvironmentVariables();
            Configuration = new OneMedifyConfiguration(builder.Build());
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

            ////1Authority Setup

            //services.AddJwtBearer(options =>
            //{
            //    options.Authority = Configuration["1Authority:Authority"];
            //    options.RequireHttpsMetadata = Convert.ToBoolean(Configuration["1Authority:RequireHttpsMetadata"]);
            //    options.Audience = Configuration["1Authority:ApiName"];
            //});

            //services.AddOneAuthorityService(options =>
            //{
            //    options.Authority = "http://172.16.1.225:8501";
            //    options.RequireHttpsMetadata = false;
            //    options.ApiName = "1TrakItFMAPI";
            //    options.ApiSecret = "secret";
            //});

            //services.AddAuthentication();
            services.AddAuthentication("Bearer")
             .AddIdentityServerAuthentication(options =>
             {
                 var option = Configuration.Authority;
                 options.Authority = option.Authority;
                 options.RequireHttpsMetadata = option.RequireHttpsMetadata;
                 options.ApiName = option.ApiName;
                 options.ApiSecret = option.ApiSecret;
             });

            services.AddAuthorizationCore();

            ////Policy Server Setup
            services.AddPolicyServerClient(options =>
            {
                options.Provider = Configuration["1Authority:PolicyProvider"];
                options.ApplicationName = Configuration["1Authority:PolicyApplicationName"];
                options.PolicyName = Configuration["1Authority:PolicyName"];
                options.ClientId = Configuration["1Authority:PolicyClientId"];
                options.Authority = Configuration["1Authority:Authority"];
                options.ClientSecret = Configuration["1Authority:PolicyClientSecret"];
                options.ApiName = Configuration["1Authority:PolicyApiName"];
            });

            //Business Layer Services
            services.AddTransient<ICityService, CityService>();
            services.AddTransient<IDieasesService, DieasesService>();
            services.AddTransient<IDoctorService, DoctorService>();
            services.AddTransient<IDoctorRegistrationService, DoctorRegistrationService>();
            services.AddTransient<IDoctorUpdateService, DoctorUpdateService>();
            services.AddTransient<IGetDoctorByDoctorMobileNoService, GetDoctorByDoctorMobileNoService>();
            services.AddTransient<IGetDoctorsCountService, GetDoctorsCountService>();
            services.AddTransient<IGetDoctorsListService, GetDoctorsListService>();
            services.AddTransient<IGetPatientsCreatedPrescriptionCount, GetPatientsCreatedPrescriptionCount>();
            services.AddTransient<IPatientsPharmacyCountService, PatientsPharmacyCountService>();
            services.AddTransient<IMedicineService, MedicineService>();
            services.AddTransient<IPatientService, PatientService>();
            services.AddTransient<IPatientUpdateService, PatientUpdateService>();
            services.AddTransient<IGetPatientProfile, GetPatientProfile>();
            services.AddTransient<IGetPatientByDoctorMobile, GetPatientByDoctorMobile>();
            services.AddTransient<IGetUploadedPrescriptionsByPatientMobileNumber, GetUploadedPrescriptionsByPatientMobileNumber>();
            services.AddTransient<IGetUploadedPrescriptionsByPharmacyMobileNumber, GetUploadedPrescriptionsByPharmacyMobileNumber>();
            services.AddTransient<IPharmacyService, PharmacyService>();
            services.AddTransient<IPharmacyRegistrationService, PharmacyRegistrationService>();
            services.AddTransient<IPharmacyUpdateService, PharmacyUpdateService>();
            services.AddTransient<IPrescriptionService, PrescriptionService>();
            services.AddTransient<ISendForApprovalService, SendForApprovalService>();
            services.AddTransient<ICreatePrescriptionService, CreatePrescriptionService>();
            services.AddTransient<IGetApprovedPrescriptionService, GetApprovedPrescriptionService>();
            services.AddTransient<IGetPrescriptionsSentForApprovalByPatientByDoctorMobileService, GetPrescriptionsSentForApprovalByPatientByDoctorMobileService>();
            services.AddTransient<IStateService, StateService>();
            services.AddTransient<IDoctorChangeForPrescriptionService, DoctorChangeForPrescriptionService>();
            services.AddTransient<IGetCreatedPrescriptionDetailsByPrescriptionId, GetCreatedPrescriptionDetailsByPrescriptionId>();
            services.AddTransient<IDoctorActionLogService, DoctorActionLogService>();
            services.AddTransient<IPrescriptionListSentForApprovalByPharmacy, PrescriptionListSentForApprovalByPharmacy>();
            services.AddTransient<IPrintCreatedPrescriptionService, PrintCreatedPrescriptionService>();

            //Data Access Layer Repositories
            services.AddTransient<ICityRepository, CityRepository>();
            services.AddTransient<IDiseaseRepository, DieasesRepository>();
            services.AddTransient<IDoctorRepository, DoctorRepository>();
            services.AddTransient<IMedicineRepository, MedicineRepository>();
            services.AddTransient<IPatientDiseaseRepository, PatientDiseaseRepository>();
            services.AddTransient<IPatientRepository, PatientRepository>();
            services.AddTransient<IPharmacyRepository, PharmacyRepository>();
            services.AddTransient<IPrescriptionMedicineRepository, PrescriptionMedicineRepository>();
            services.AddTransient<IPrescriptionRepository, PrescriptionRepository>();
            services.AddTransient<IStateRepository, StateRepository>();
            services.AddTransient<IUploadPrescriptionRepository, UploadPrescriptionRepository>();
            services.AddTransient<IDoctorActionLogRepository, DoctorActionLogRepository>();
            

            //Shared Layer Services
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<IFileService, FileService>();
            services.AddTransient<IFileUpload, FileUpload>();
            services.AddTransient<IFileValidations, FileValidations>();
            services.AddTransient<IOneAuthorityService, OneAuthorityService>();
            services.AddScoped<IRestServiceClient, RestServiceClient>();
            services.AddTransient<IUserValidations, UserValidations>();

            services.AddHostedService<DoctorChangeService>();
            services.AddScoped<HttpClient>();
            services.AddSingleton<Configuration>();

            //Application DbContext
            services.AddDbContext<OneMedifyDbContext>(options
                => options.UseSqlServer(Configuration.GetConnectionString("OneMedifyConnectionString")));

            //Cors Service
            services.AddCors(x => x.AddPolicy("CorsPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
            }));

            services.AddControllersWithViews()
             .AddNewtonsoftJson(options =>
             options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
             );
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors("CorsPolicy");
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UsePolicyServerClaims();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}