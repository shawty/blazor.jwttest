using blazor.jwttest.Client.Classes;
using Blazor.Extensions.Storage;
using Microsoft.AspNetCore.Blazor.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace blazor.jwttest.Client
{
  public class Startup
  {
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddStorage();

      services.AddSingleton<ApplicationState>();
      services.AddTransient<JwtDecode>();

    }

    public void Configure(IBlazorApplicationBuilder app)
    {
      app.AddComponent<App>("app");
    }

  }
}
