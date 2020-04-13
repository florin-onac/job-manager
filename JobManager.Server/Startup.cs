using System.Net;
using AutoMapper;
using JobManager.Server.AppServices;
using JobManager.Server.Configuration.Mapper;
using JobManager.Server.Database;
using JobManager.Server.DataServices;
using JobManager.Server.Quartz;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;

namespace JobManager.Server
{
    public class Startup
    {
        private readonly string corsPolicy = "defaultCorsPolicy";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(
                options =>
                {
                    options.AddPolicy(
                        corsPolicy,
                        builder =>
                            builder
                                .AllowAnyOrigin()
                                .AllowAnyMethod()
                                .AllowAnyHeader());
                });

            services
                .AddControllers()
                .AddNewtonsoftJson(
                    options =>
                    {
                        options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                        options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                    })
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            services.AddHealthChecks();

            services.AddHostedService<QuartzService>();

            services.AddAutoMapper(typeof(MapperProfile));

            services.AddDbContext<JobManDbContext>(
                x =>
                x.UseSqlServer(Configuration.GetConnectionString("JobManDb")));

            ConfigureDataServices(services);
            ConfigureAppServices(services);
            ConfigureServicesForSwagger(services);

            ConfigureQuartzServices(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, JobManDbContext context)
        {
            ConfigureExceptionHandling(app);
            ConfigureDatabase(context);
            ConfigureSwagger(app);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseCors(corsPolicy);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("/health");
                endpoints.MapControllers();
            });
        }

        private void ConfigureDatabase(JobManDbContext context)
        {
            bool.TryParse(Configuration["AutomaticMigrations"], out var applyMigrations);

            if (!applyMigrations)
                return;

            context.Database.Migrate();
        }

        private void ConfigureExceptionHandling(IApplicationBuilder app)
        {
            app.UseExceptionHandler(appBuilder =>
            {
                appBuilder.Run(async context =>
                {
                    context.Response.StatusCode = (int) HttpStatusCode.BadRequest;
                    context.Response.ContentType = "application/json";

                    var handler = context.Features.Get<IExceptionHandlerPathFeature>();

                    var response = new
                    {
                        Error = handler?.Error?.Message ?? "UNKNOWN ERROR?!?"
                    };

                    await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
                });
            });
        }

        private void ConfigureDataServices(IServiceCollection services)
        {
            services.AddScoped<IJobDataService, JobDataService>();
            services.AddScoped<IOrderDataService, OrderDataService>();
        }

        private void ConfigureAppServices(IServiceCollection services)
        {
            services.AddScoped<IJobProcessor, OrderExporter>();
            services.AddScoped<IJobProcessor, OrderImporter>();
        }

        private void ConfigureQuartzServices(IServiceCollection services)
        {
            services.AddSingleton<IJobFactory, JobFactory>();
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();

            services.AddScoped<JobRunner>();

            services.AddSingleton(new JobSchedule(typeof(JobRunner), Configuration["JobRunner:CronExpression"]));
        }

        private void ConfigureServicesForSwagger(IServiceCollection services)
        {
            services.AddOpenApiDocument(settings =>
            {
                settings.PostProcess = document =>
                {
                    document.Info.Title = "JobManager API";
                    document.Info.Description = "JobManager API";
                };
            });
        }

        private void ConfigureSwagger(IApplicationBuilder app)
        {
            app.UseOpenApi();
            app.UseSwaggerUi3();
        }
    }
}