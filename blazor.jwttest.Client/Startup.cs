using blazor.jwttest.Client.Classes;
using Blazor.Extensions.Storage;
using Microsoft.AspNetCore.Blazor.Browser.Services;
using Microsoft.AspNetCore.Blazor.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;

namespace blazor.jwttest.Client
{
  public class Startup
  {
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddStorage();

      services.AddSingleton<ApplicationState>();
      services.AddTransient<JwtDecode>();

      services.AddSingleton(x => new HttpClient(new AuthenticationDelegationHandler())
      {
        BaseAddress = new Uri(BrowserUriHelper.Instance.GetBaseUri())
      });

    }

    public void Configure(IBlazorApplicationBuilder app)
    {
      app.AddComponent<App>("app");
    }

  }
}
