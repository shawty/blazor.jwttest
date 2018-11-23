using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using blazor.jwttest.Server.Services;
using blazor.jwttest.Server.Services.Exceptions;
using blazor.jwttest.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace gisportal.Server.Controllers
{
  [Route("api/[controller]")]
  public class AuthenticationController : Controller
  {
    private readonly IConfiguration _configuration;
    private readonly Users _users;

    public AuthenticationController(
      IConfiguration configuration,
      Users users
      )
    {
      _configuration = configuration;
      _users = users;
    }

    [HttpPost("[action]")]
    [AllowAnonymous]
    public IActionResult Login([FromBody] LoginDetails login)
    {
      User thisUser = null;
      try
      {
        thisUser = _users.ValidateLogin(login.Username, login.Password);
      }
      catch (UserNotValidatedException)
      {
        // NOTE: If you want to check the exception at this point, there's a validation reason on it that tells you
        // what actually failed, however it's generally good practice not to tell that info to the UI as it gives
        // anyone trying to gain access maliciously and idea of what's right and what's not :-)
        return BadRequest("Username and password are invalid.");
      }

      DateTime issueTime = DateTime.UtcNow;

      // Add required and basic JWT claims to the token
      List<Claim> claims = new List<Claim>
      {
        new Claim(JwtRegisteredClaimNames.Sub, thisUser.LoginName),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(DateTime.UtcNow).ToUniversalTime().ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
        new Claim(JwtRegisteredClaimNames.Email, thisUser.Email),
        new Claim(JwtRegisteredClaimNames.GivenName, thisUser.FullName)
      };

      // Add our users roles to the JWT
      // DO NOT Change this claim name, ASP.NET core role auth requires this exact claim name for controller roles
      claims.AddRange(
        thisUser.AllowedRoles
        .Select(role => new Claim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", role, ClaimValueTypes.String)));

      // Build the actual token
      int expiryLengthInMinutes = Convert.ToInt32(_configuration["JwtExpiryInMinutes"]);
      DateTime now = DateTime.UtcNow;
      TimeSpan expirationTime = new TimeSpan(0, expiryLengthInMinutes, 0);
      var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JwtSecurityKey"]));
      var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

      var jwt = new JwtSecurityToken(
        _configuration["JwtIssuer"],
        _configuration["JwtIssuer"],
        claims,
        expires: now.Add(expirationTime),
        signingCredentials: signingCredentials);

      var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

      // Create response and send token back to caller
      var response = new
      {
        token = encodedJwt,
        expires_in = (int)expirationTime.TotalSeconds
      };

      return Ok(response);

    }

  }
}