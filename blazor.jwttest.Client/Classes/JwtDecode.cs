using Blazor.Extensions.Storage;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace blazor.jwttest.Client.Classes
{
  public class JwtDecode
  {
    // TODO: Figure out how to get the JWT Auth .net libs into blazor withoout totaly screwing things up
    // and replace all the JwtClaim names in this file with proper constants.

    private string _header { get; set; }
    private string _payload { get; set; }
    private string _token { get; set; }
    private LocalStorage _localStorage { get; }
    private string _authTokenName { get; set; }

    public JwtDecode(LocalStorage localStorage)
    {
      _localStorage = localStorage;
    }

    private void DecodeToken()
    {
      string[] parts = _token.Split('.');
      _header = parts[0];
      _payload = parts[1];

      if (_header.Length % 4 != 0) // B64 strings must be a length that is a multiple of 4 to decode them
      {
        var lengthToBe = _header.Length.GetNextHighestMultiple(4);
        _header = _header.PadRight(lengthToBe, '=');
      }

      if (_payload.Length % 4 != 0) // B64 strings must be a length that is a multiple of 4 to decode them
      {
        var lengthToBe = _payload.Length.GetNextHighestMultiple(4);
        _payload = _payload.PadRight(lengthToBe, '=');
      }

    }

    public async Task<bool> LoadToken(string tokenName)
    {
      _authTokenName = tokenName;

      _token = await _localStorage.GetItem<string>(_authTokenName);
      return true;
    }

    public Dictionary<string, object> GetPayload()
    {
      DecodeToken();

      var payload = Encoding.UTF8.GetString(Convert.FromBase64String(_payload));
      var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(payload);

      // Testing
      //foreach(var key in data)
      //{
      //  Console.WriteLine($"PAYLOAD: {key.Key} = {key.Value} (TYP: {key.Value.GetType().ToString()})");
      //}

      return data;
    }

    public Dictionary<string, object> GetHeader()
    {
      DecodeToken();

      var header = Encoding.UTF8.GetString(Convert.FromBase64String(_header));
      var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(header);

      // Testing
      //foreach (var key in data)
      //{
      //  Console.WriteLine($"HEADER: {key.Key} = {key.Value}");
      //}

      return data;
    }

    public string GetName()
    {
      string result = "Not Set";
      string nameClaim = "sub";

      Dictionary<string, object> payload = GetPayload();
      if (payload.ContainsKey(nameClaim))
      {
        result = payload[nameClaim] as string;
      }

      return result;
    }

    public string GetFullName()
    {
      string result = "Not Set";
      string givenNameClaim = "given_name";

      Dictionary<string, object> payload = GetPayload();
      if (payload.ContainsKey(givenNameClaim))
      {
        result = payload[givenNameClaim] as string;
      }

      return result;
    }

    public string GetEmail()
    {
      string result = "Not Set";
      string emailClaim = "email";

      Dictionary<string, object> payload = GetPayload();
      if (payload.ContainsKey(emailClaim))
      {
        result = payload[emailClaim] as string;
      }

      return result;
    }

    public List<string> GetRoles()
    {
      // DO NOT Change this claim name, ASP.NET Core roles auth requires this exact role for roles on a controller to work
      const string roleType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";

      List<string> result = new List<string>();

      Dictionary<string, object> payload = GetPayload();
      if (payload.ContainsKey(roleType))
      {
        // Role claims can either be a single string or a JArray (since where using Json.Net), we need to detect which
        if (payload[roleType] is JArray)
        {
          result.AddRange((payload[roleType] as JArray).ToObject<List<string>>());
        }

        if (payload[roleType] is string)
        {
          result.Add(payload[roleType] as string);
        }

      }

      return result;
    }

    public bool HasTokenExpired()
    {
      bool result = false; // Always default to NO
      string expiryClaim = "exp";

      Dictionary<string, object> payload = GetPayload();
      if (payload.ContainsKey(expiryClaim))
      {
        double timeStamp = Convert.ToDouble(payload[expiryClaim]);
        DateTime expiryTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        expiryTime = expiryTime.AddSeconds(timeStamp);
        if(expiryTime < DateTime.UtcNow)
        {
          result = true;
        }
      }

      return result;
    }

    public string GetIssuer()
    {
      string result = "NOT SET";
      string issuerClaim = "iss";

      Dictionary<string, object> payload = GetPayload();
      if (payload.ContainsKey(issuerClaim))
      {
        result = payload[issuerClaim] as string;
      }

      return result;
    }
  }
}
