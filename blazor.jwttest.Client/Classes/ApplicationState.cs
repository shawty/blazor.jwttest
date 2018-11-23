using System.Net.Http;
using System.Text;
using Microsoft.JSInterop;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System;
using System.Collections.Generic;
using Blazor.Extensions.Storage;
using blazor.jwttest.Shared;
using Microsoft.AspNetCore.Blazor.Services;
using System.Linq;

namespace blazor.jwttest.Client.Classes
{
  public class ApplicationState
  {
    private readonly HttpClient _httpClient;
    private readonly LocalStorage _localStorage;
    private readonly JwtDecode _jwtDecoder;
    private readonly IUriHelper _uriHelper;

    private const string AuthTokenName = "authToken";

    public event EventHandler<string> LoginSucceeded;
    public event EventHandler<string> LogoutSucceeded;

    public bool IsLoggedIn { get; private set; }
    public string UserName { get; private set; }
    public string FullName { get; private set; }
    public string Email { get; private set; }
    public List<string> UserRoles { get; private set; }

    public ApplicationState(
      HttpClient httpClient,
      LocalStorage localStorage,
      JwtDecode jwtDecoder,
      IUriHelper uriHelper
    )
    {
      _httpClient = httpClient;
      _localStorage = localStorage;
      _jwtDecoder = jwtDecoder;
      _uriHelper = uriHelper;
      UserName = String.Empty;
      UserRoles = new List<string>();
    }

    /// <summary>
    /// Contacts the backend API using the route /api/authentication/login and passes the provided LoginDetails to it
    /// if the login is successfull a JWT Token is returned and stored in the session store, a flag is set to show login
    /// was successfull, and things like the user name, roles etc are made available to be used in the app, followed by
    /// a LoginSuccessfull event being raised for any callers that require it
    /// </summary>
    /// <param name="loginDetails">LoginDetails class holding username & password to be used for authentication</param>
    /// <returns>Async Task</returns>
    public async Task Login(LoginDetails loginDetails)
    {
      var response = await _httpClient.PostAsync("/api/authentication/login",
        new StringContent(Json.Serialize(loginDetails),
        Encoding.UTF8,
        "application/json"));

      if (response.IsSuccessStatusCode)
      {
        await SaveToken(response);
        await SetAuthorizationHeader();
        await _jwtDecoder.LoadToken(AuthTokenName);

        UserName = _jwtDecoder.GetName();
        FullName = _jwtDecoder.GetFullName();
        Email = _jwtDecoder.GetEmail();
        UserRoles = _jwtDecoder.GetRoles();

        IsLoggedIn = true;

        LoginSucceeded?.Invoke(this, null);

      }
    }

    /// <summary>
    /// Removes any stored JWT Token, resets all the logged in user data then raises a LogoutSucceeded event to
    /// notify any event listners that need to know a logout has happened.
    /// </summary>
    /// <returns>Async Task</returns>
    public async Task Logout()
    {
      await _localStorage.RemoveItem(AuthTokenName);
      RemoveAuthorizationHeader();

      IsLoggedIn = false;
      UserName = FullName = Email = String.Empty;
      UserRoles.Clear();

      LogoutSucceeded?.Invoke(this, null);
    }

    /// <summary>
    /// Checks against any logged in user data it holds to determine if the current operation (Usually a page navigation)
    /// should be allowed to proceed. takes 2 required parameters and 1 optional parameter.
    /// </summary>
    /// <param name="allowedRoles">List<string> of roles that should be checked against user logged in roles to determine if user is allowed access</string></param>
    /// <param name="failReason">out parameter of type NavigationFailReason, used to communicate reason for failiure back to caller</param>
    /// <param name="notLoggedInRoute">[Optional] if provided, and user is not logged in, will attempt to redirect to the supplied route</param>
    /// <returns>false if page navigation should be halted, otherwise true</returns>
    public bool IsAllowedToNavigate(in List<string> allowedRoles, out NavigationFailReason failReason, in string notLoggedInRoute = "")
    {
      bool result = false;

      if (!IsLoggedIn)
      {
        if(!String.IsNullOrEmpty(notLoggedInRoute))
        {
          _uriHelper.NavigateTo(notLoggedInRoute);
        }
        failReason = NavigationFailReason.NotLoggedIn;
        return result;
      }

      if (!UserRoles.Intersect(allowedRoles).Any())
      {
        failReason = NavigationFailReason.RoleNotAllowed;
        return result;
      }

      result = true;
      failReason = NavigationFailReason.NoFail;
      return result;
    }

    private async Task SaveToken(HttpResponseMessage response)
    {
      var responseContent = await response.Content.ReadAsStringAsync();
      var jwt = Json.Deserialize<JwToken>(responseContent);

      await _localStorage.SetItem<string>(AuthTokenName, jwt.Token);
    }

    private async Task SetAuthorizationHeader()
    {
      if (!_httpClient.DefaultRequestHeaders.Contains("Authorization"))
      {
        var token = await _localStorage.GetItem<string>(AuthTokenName);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
      }
    }

    private void RemoveAuthorizationHeader()
    {
      if (_httpClient.DefaultRequestHeaders.Contains("Authorization"))
      {
        _httpClient.DefaultRequestHeaders.Remove("Authorization");
      }
    }

  }
}
