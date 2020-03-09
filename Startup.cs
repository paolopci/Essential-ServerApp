using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ServerApp.Models;
using Microsoft.OpenApi.Models;
using System;

namespace ServerApp
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
      string connectionString = Configuration["ConnectionStrings:DefaultConnection"];
      services.AddDbContext<DataContext>(options => options.UseSqlServer(connectionString));
      services.AddControllersWithViews().AddJsonOptions(opts =>
      {
        opts.JsonSerializerOptions.IgnoreNullValues = true;
      });
      services.AddControllersWithViews();

      services.AddSwaggerGen(opts =>
      {
        opts.SwaggerDoc("v1", new OpenApiInfo
        {
          Title = "SportsStore API", Version = "v1"
        });
      });

      // SEssion Sql-cache Capther 9
      services.AddDistributedSqlServerCache(opts =>
      {
        opts.ConnectionString = connectionString;
        opts.SchemaName = "dbo";
        opts.TableName = "SessionData";
      });

      services.AddSession(opts =>
      {
        opts.Cookie.Name = "SportsStore.Session";
        opts.IdleTimeout = System.TimeSpan.FromHours(48);
        opts.Cookie.HttpOnly = false;
        opts.Cookie.IsEssential = true;
      });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider services)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
      }

      app.UseHttpsRedirection();
      app.UseStaticFiles();
      // Session Chapter 9
      app.UseSession();

      app.UseRouting();

      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllerRoute(
          name: "default",
          pattern: "{controller=Home}/{action=Index}/{id?}");

        endpoints.MapControllerRoute(
          name: "Angular_fallback",
          pattern: "{target:regex(store|cart|checkout)}/{*catchall}",
          defaults: new {controller = "Home", action = "Index"});
      });


      // Swagger
      app.UseSwagger();
      app.UseSwaggerUI(opts => { opts.SwaggerEndpoint("/swagger/v1/swagger.json", "SportsStore API"); });


      app.UseSpa(spa =>
      {
        string strategy = Configuration.GetValue<string>("DevTools:ConnectionStrategy");
        if (strategy == "proxy")
        {
          spa.UseProxyToSpaDevelopmentServer("http://127.0.0.1:4200");
        }
        else if (strategy == "managed")
        {
          spa.Options.SourcePath = "../ClientApp";
          spa.UseAngularCliServer("start");
        }
      });

      SeedData.SeedDatabase(services.GetRequiredService<DataContext>());
    }
  }
}