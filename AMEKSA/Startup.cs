using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AMEKSA.Context;
using AMEKSA.Models;
using AMEKSA.Privilage;
using AMEKSA.Repo;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft;
using Microsoft.AspNetCore.Identity.UI.Services;
using AMEKSA.Services;

namespace AMEKSA
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddIdentity<ExtendIdentityUser, ExtendIdentityRole>(op =>
            {
                op.Password.RequiredLength = 7;
                op.Password.RequireDigit = false;
                op.Password.RequireLowercase = false;
                op.Password.RequireNonAlphanumeric = false;
                op.Password.RequireUppercase = false;
                op.User.RequireUniqueEmail = true;
                op.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                op.SignIn.RequireConfirmedEmail = true;
                
                
            }).AddDefaultTokenProviders().AddEntityFrameworkStores<DbContainer>();
            
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            
            
            services.AddDbContextPool<DbContainer>(op => op.UseSqlServer(Configuration.GetConnectionString("con")));
            services.AddHttpContextAccessor();
            services.AddCors(options =>
            {
                options.AddPolicy("allow",
                                    a => a.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
                                  );
               
            });
            services.AddScoped<IStringPropertiesRep, StringPropertiesRep>();
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddScoped<IAttendRep, AttendRep>();
            services.AddScoped<IChangeCategoryRep, ChangeCategoryRep>();
            services.AddScoped<IVisitsCollectionRep, VisitsCollectionRep>();
            services.AddScoped<IHrRep, HrRep>();
            services.AddScoped<ISystemAdminRep, SystemAdminRep>();
            services.AddScoped<IKpisRep, KpisRep>();
            services.AddScoped<IContactMonthlyPlanRep, ContactMonthlyPlanRep>();
            services.AddScoped<IAccountMonthlyPlanRep, AccountMonthlyPlanRep>();
            services.AddTransient<IFirstManagerRep, FirstManagerRep>();
            services.AddTransient<ITopManagerRep, TopManagerRep>();
            services.AddTransient<IUserRep, UserRep>();
            services.AddTransient<IAccountRep, AccountRep>();
            services.AddScoped<IAccountTypeRep, AccountTypeRep>();
            services.AddScoped<ITimeRep, TimeRep>();
            services.AddScoped<IChatRep, ChatRep>();
            services.AddTransient<ITimeOffRep, TimeOffRep>();
            services.AddTransient<IRepRep, RepRep>();
            services.AddTransient<IContactRep, ContactRep>();
            services.AddScoped<IContactTypeRep, ContactTypeRep>();
            services.AddTransient<IContactMedicalVisitRep, ContactMedicalVisitRep>();
            services.AddTransient<IAccountMedicalVisitRep,AccountMedicalVisitRep>();
            services.AddScoped<ICityRep, CityRep>();
            services.AddScoped<IDisrictRep, DistrictRep>();
            services.AddScoped<IBrandRep, BrandRep>();
            services.AddScoped<IProductRep, ProductRep>();
            services.AddScoped<IPurchaseTypeRep, PurchaseTypeRep>();
            services.AddTransient<ICategoryRep, CategoryRep>();
            services.AddScoped<ISalesAidRep, SalesAidRep>();
            services.AddTransient<IAccountSalesVisitRep, AccountSalesVisitRep>();
            services.AddScoped<IChangeContactTargetRep, ChangeContactTargetRep>();
            services.AddAuthentication();
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            app.UseMvc();
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseCors();
            app.UseHsts();


        }
    }
}
