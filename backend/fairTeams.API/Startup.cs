using fairTeams.Core;
using fairTeams.DemoHandling;
using fairTeams.Steamworks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System.IO;
using System.Text.Json.Serialization;

namespace fairTeams.API
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
                .AddJsonOptions(options =>
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "fair-teams-ai api", Version = "v1" });
            });
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.AllowAnyOrigin().AllowAnyHeader();
                    }
                );
            });

            var matchesDatabasePath = Path.Combine(Settings.ApplicationFolder, "matches.db");
            services.AddDbContext<MatchRepository>(d => d.UseSqlite($"Data Source={matchesDatabasePath}", x => x.MigrationsAssembly("fairTeams.API")));

            var steamUserDatabasePath = Path.Combine(Settings.ApplicationFolder, "users.db");
            services.AddDbContext<SteamUserRepository>(d => d.UseSqlite($"Data Source={steamUserDatabasePath}", x => x.MigrationsAssembly("fairTeams.API")));

            var shareCodeDatabasePath = Path.Combine(Settings.ApplicationFolder, "sharecodes.db");
            services.AddDbContext<ShareCodeRepository>(d => d.UseSqlite($"Data Source={shareCodeDatabasePath}", x => x.MigrationsAssembly("fairTeams.API")));

            services.AddHostedService<ShareCodeCollector>();
            services.AddHostedService<MatchMakingDemoCollector>();
            services.AddHostedService<LocalDemoCollector>();
            services.AddHostedService<FTPDemoCollector>();

            services.AddTransient<SteamworksApi>();

            services.AddScoped<ITeamAssigner, SkillBasedAssigner>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "fair-teams-ai v1"));

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
