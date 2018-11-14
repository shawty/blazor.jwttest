using System.Net.Http;
using System.Text;
using Microsoft.JSInterop;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System;
using System.Collections.Generic;
using Blazor.Extensions.Storage;
using blazor.jwttest.Shared;

namespace blazor.jwttest.Client.Classes
{
  public class ApplicationState
  {
    private readonly HttpClient _httpClient;
    private readonly LocalStorage _localStorage;
    private readonly JwtDecode _jwtDecoder;

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
      JwtDecode jwtDecoder
    )
    {
      _httpClient = httpClient;
      _localStorage = localStorage;
      _jwtDecoder = jwtDecoder;
      UserName = String.Empty;
      UserRoles = new List<string>();
    }

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

    public async Task Logout()
    {
      await _localStorage.RemoveItem(AuthTokenName);

      IsLoggedIn = false;
      UserName = FullName = Email = String.Empty;
      UserRoles.Clear();

      LogoutSucceeded?.Invoke(this, null);
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

  }
}
