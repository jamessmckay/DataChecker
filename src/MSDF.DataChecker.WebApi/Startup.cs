// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.

using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AutofacSerilogIntegration;
using AutoMapper;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using MSDF.DataChecker.Domain;
using MSDF.DataChecker.WebApi.Modules;
using Serilog;
using Swashbuckle.AspNetCore.Filters;

namespace MSDF.DataChecker.WebApi
{
    public class Startup
    {
        private const string CorsPolicy = "CorsPolicy";

        public Startup(IWebHostEnvironment env, IConfiguration configuration)
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public ILifetimeScope Container { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Log.Debug("Configuring Services");

            services.AddSingleton(Configuration);

            services.AddDbContext<DatabaseContext>(
                options =>
                    options.UseSqlServer("name=ConnectionStrings:DataCheckerStore"));

            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            services.AddScoped(
                serviceProvider =>
                {
                    var actionContext = serviceProvider.GetRequiredService<IActionContextAccessor>().ActionContext;
                    var factory = serviceProvider.GetRequiredService<IUrlHelperFactory>();
                    return factory.GetUrlHelper(actionContext);
                });

            services.AddCors(
                options =>
                {
                    options.AddPolicy(
                        CorsPolicy,
                        builder => builder
                            .AllowAnyOrigin()
                            .AllowAnyMethod()
                            .AllowAnyHeader()
                            .WithExposedHeaders("*"));
                });

            services
                .AddControllers()
                .AddJsonOptions(options => options.JsonSerializerOptions.WriteIndented = true)
                .AddControllersAsServices();

            services.AddSwaggerGen(
                c =>
                {
                    c.SwaggerDoc(
                        "v1", new OpenApiInfo
                        {
                            Title = "DataChecker API",
                            Version = "1.0.0"
                        });

                    c.EnableAnnotations();
                    c.OperationFilter<AddResponseHeadersFilter>();
                    c.OrderActionsBy(apiDesc => $"{apiDesc.ActionDescriptor.RouteValues["controller"]}_{apiDesc.HttpMethod}");
                });

            services.AddHangfire(
                config =>
                    config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                        .UseSimpleAssemblyNameTypeSerializer()
                        .UseRecommendedSerializerSettings()
                        .UseSqlServerStorage(
                            Configuration.GetConnectionString("RulesExecutorStore"),
                            new SqlServerStorageOptions
                            {
                                CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                                SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                                QueuePollInterval = TimeSpan.Zero,
                                UseRecommendedIsolationLevel = true,
                                DisableGlobalLocks = true
                            }));

            if (Configuration.GetValue<bool?>("JobExecutor:Host") ?? false)
            {
                // Add the processing server as IHostedService
                // support previous implementation
                services.AddHangfireServer(
                    options => { options.WorkerCount = Configuration.GetValue<int?>("JobExecutor:Processes") ?? 2; });
            }

            if (Configuration.GetValue<bool?>("UseReverseProxy") ?? false)
            {
                services.Configure<ForwardedHeadersOptions>(
                    options =>
                    {
                        options.ForwardedHeaders = ForwardedHeaders.XForwardedFor
                                                   & ForwardedHeaders.XForwardedHost
                                                   & ForwardedHeaders.XForwardedProto;
                    });
            }
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // ConfigureContainer is where you can register things directly
        // with Autofac. This runs after ConfigureServices so the things
        // here will override registrations made in ConfigureServices.
        // Don't build the container; that gets done for you by the factory.
        public void ConfigureContainer(ContainerBuilder builder)
        {
            Log.Debug("Configuring Autofac");
            builder.RegisterLogger();
            builder.RegisterModule<WebApiModule>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseSerilogRequestLogging();

            Container = app.ApplicationServices.GetAutofacRoot();

            app.UseForwardedHeaders();

            if (env.IsDevelopment())
            {
                var mapperConfiguration = Container.Resolve<MapperConfiguration>();
                mapperConfiguration.AssertConfigurationIsValid();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");

                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // app.ConfigureExceptionHandler();
            app.UseHttpsRedirection();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger(c => { c.RouteTemplate = "api-docs/{documentName}/openapi.json"; });

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(
                c =>
                {
                    c.RoutePrefix = "api-docs";
                    c.SwaggerEndpoint("/api-docs/v1/openapi.json", "DataChecker V1");
                });

            app.UseRouting();
            app.UseStaticFiles();
            app.UseCors(CorsPolicy);

            // required to get the base controller working
            app.UseEndpoints(
                endpoints =>
                {
                    endpoints.MapControllers();
                    endpoints.MapHangfireDashboard("/dashboards/hangfire");
                });
        }
    }
}
