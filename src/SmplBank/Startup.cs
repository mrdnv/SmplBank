using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using SmplBank.Authentication;
using SmplBank.Domain.Common;
using SmplBank.Domain.Repository;
using SmplBank.Domain.Service;
using SmplBank.Domain.Service.Interface;
using SmplBank.Filters;
using SmplBank.Infrastructure.Common;
using SmplBank.Infrastructure.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace SmplBank
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
            services
                .AddRouting(routeOptions => routeOptions.LowercaseUrls = true)
                .AddSwagger(Configuration)
                .AddBasicAuthentication()
                .AddSqlDbConnection(Configuration)
                .AddRepositories(Configuration)
                .AddCommonServices(Configuration)
                .AddServices(Configuration)
                .AddBackgroundJobs(Configuration)
                .AddControllers(options => options.Filters.Add(typeof(HttpGlobalExceptionFilter)));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IBackgroundJobClient backgroundJobClient, ITransactionProcessor transactionProcessor)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app
                .UseSwagger()
                .UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SmplBank API V1");
                });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app
                .UseBackgroundJobs(backgroundJobClient)
                .AddBackgroundJobService(transactionProcessor);
        }
    }

    public static class ServiceConfigurationExtensions
    {
        public static IServiceCollection AddBasicAuthentication(this IServiceCollection services)
        {
            services.AddAuthentication("BasicAuthentication")
                .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);

            return services;
        }

        public static IServiceCollection AddSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "SmplBank - Web API",
                    Version = "v1",
                    Description = "A simple bank service."
                });

                options.AddSecurityDefinition("Basic", new OpenApiSecurityScheme
                {
                    Description = "Basic auth added to authorization header",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Scheme = "basic",
                    Type = SecuritySchemeType.Http
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Basic" }
                        },
                        new List<string>()
                    }
                });
            });

            return services;

        }

        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            return services.AddScoped<IUserService, UserService>()
                .AddScoped<IAccountService, AccountService>()
                .AddScoped<ITransactionService, TransactionService>()
                .AddScoped<ITransactionProcessor, TransactionProcessor>()
                ;
        }

        public static IServiceCollection AddCommonServices(this IServiceCollection services, IConfiguration configuration)
        {
            return services.AddScoped<ISecurityService, SecurityService>()
                ;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services, IConfiguration configuration)
        {
            return services.AddScoped(typeof(IRepository<>), typeof(Repository<>))
                .AddScoped<IAccountRepository, AccountRepository>()
                .AddScoped<ITransactionRepository, TransactionRepository>()
                .AddScoped<IUserRepository, UserRepository>()
                ;
        }

        public static IServiceCollection AddSqlDbConnection(this IServiceCollection services, IConfiguration configuration)
        {
            return services.AddScoped<IDbConnection, SqlConnection>((_) => new SqlConnection(configuration["ConnectionStrings:Default"]));
        }

        public static IServiceCollection AddBackgroundJobs(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration["ConnectionStrings:Default"];

            return services
                .AddHangfire(configuration => configuration
                    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UseSqlServerStorage(connectionString, new SqlServerStorageOptions
                    {
                        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                        QueuePollInterval = TimeSpan.Zero,
                        UseRecommendedIsolationLevel = true,
                        DisableGlobalLocks = true
                    }))
                .AddHangfireServer();
        }

        public static IApplicationBuilder UseBackgroundJobs(this IApplicationBuilder appBuilder, IBackgroundJobClient backgroundJobs)
        {
            appBuilder.UseHangfireDashboard();
            backgroundJobs.Enqueue(() => Console.WriteLine("Hello world from Hangfire!"));

            return appBuilder;
        }

        public static IApplicationBuilder AddBackgroundJobService<T>(this IApplicationBuilder appBuilder, T service) where T : IBackgroundService
        {
            RecurringJob.AddOrUpdate(
                () => service.ExecuteAsync(),
                Cron.Minutely);

            return appBuilder;
        }
    }
}
