using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ZaporArrowAPI.DbContexts;
using ZaporArrowAPI.Services;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Identity;
using ZaporArrowAPI.Entities;
using System;
using System.Threading.Tasks;

namespace ZaporArrowAPI
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

            services.AddControllers()
                .AddNewtonsoftJson(
                opt =>
                    opt.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver())
                .AddJsonOptions(opt =>
                opt.JsonSerializerOptions.PropertyNameCaseInsensitive = true);

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });


            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Configuration["Jwt:Issuer"],
                        ValidAudience = Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                        
                    };
                });

            services.AddDbContext<ZaporArrowContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("ZaporArrowDbConnection"));
            });

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ZaporArrowContext>();


            services.AddScoped<IZaporArrowRepository, ZaporArrowRepository>();


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            CreateInitAdmin(serviceProvider).Wait();

            app.UseCors("CorsPolicy");

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseStaticFiles();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                      name: "default",
                      pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private async Task CreateInitAdmin(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            
            var roleExist = await roleManager.RoleExistsAsync("Admin");

            IdentityResult identityRole;
            if(!roleExist)
            {
                identityRole = await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            var powerUser = new ApplicationUser
            {
                UserName = Configuration["AppSettings:Username"],
                IsAdmin = true,
            };

            string UserPWD = Configuration["AppSettings:Password"];
            var _user = await userManager.FindByNameAsync(Configuration["AppSettings:Username"]);

            if(_user == null)
            {
                var createPowerUser = await userManager.CreateAsync(powerUser, UserPWD);

                if (createPowerUser.Succeeded)
                {
                   var result = await userManager.AddToRoleAsync(powerUser, "Admin");

                }
            }

        }

    }
}
