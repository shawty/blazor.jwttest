using blazor.jwttest.Server.Database;
using blazor.jwttest.Server.Services;
using Microsoft.AspNetCore.Blazor.Server;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Net.Mime;

namespace blazor.jwttest.Server
{
  public class Startup
  {
    public IConfigurationRoot Configuration { get; }

    public Startup(IHostingEnvironment env)
    {
      var builder = new ConfigurationBuilder()
          .SetBasePath(env.ContentRootPath)
          .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
          .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
          .AddEnvironmentVariables();

      Configuration = builder.Build();
    }

    public void ConfigureServices(IServiceCollection services)
    {
      // Setup DC
      services.AddDbContext<EfDataContext>(options =>
      {
        options.UseNpgsql(Configuration.GetConnectionString("PostgresDbConnection"));
      });

      // Setup Dependency injected services
      services.AddTransient<Users>();
      services.AddTransient<Todos>();

      // All the rest
      services.AddMvc();

      services.AddResponseCompression(options =>
      {
        options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
          {
            MediaTypeNames.Application.Octet,
            WasmMediaTypeNames.Application.Wasm,
          });
      });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
      {
        // Make sure our database tables are created
        // NOTE: This will ONLY be acted upon if the database you are creating is EMPTY, if even just ONE table
        // exists, doesn't matter if it's a same name as tables in this application, then creation will be skipped
        // If table creation is skipped, and the table names that the entities in the datacontext look for
        // are missing, then you'll get an exception when you try to read and write data, telling you there are no tables.
        // you'll probably want to kill this and use migrations or something for a larger app, THIS IS JUST FOR TESTING
        serviceScope.ServiceProvider.GetService<EfDataContext>().Database.EnsureCreated();
      }

      app.UseResponseCompression();

      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      app.UseMvc(routes =>
      {
        routes.MapRoute(name: "default", template: "{controller}/{action}/{id?}");
      });

      app.UseBlazor<Client.Startup>();
    }
  }
}
