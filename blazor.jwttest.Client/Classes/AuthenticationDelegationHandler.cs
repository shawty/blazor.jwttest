using blazor.jwttest.Client.Exceptions;
using Microsoft.AspNetCore.Blazor.Browser.Http;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace blazor.jwttest.Client.Classes
{
  public class AuthenticationDelegationHandler : DelegatingHandler
  {
    // This handler overrides all HttpClient calls to the backend server, allowing us to better 
    // handle different error codes in our pages and components. Credit goes to
    // @kswoll (Kirk Woll) in the Blazor gitter group for pointing the way.
    public AuthenticationDelegationHandler() : base (new BrowserHttpMessageHandler())
    { }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
      var response = await base.SendAsync(request, cancellationToken);

      if (response.StatusCode == HttpStatusCode.Unauthorized)
      {
        throw new HttpUnauthorizedException();
      }

      if (response.StatusCode == HttpStatusCode.Forbidden)
      {
        throw new HttpForbiddenException();
      }

      if (response.StatusCode == HttpStatusCode.InternalServerError)
      {
        throw new HttpInternalServerErrorException();
      }

      return response;

    }

  }
}
