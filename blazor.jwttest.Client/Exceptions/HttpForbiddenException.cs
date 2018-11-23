using System;
using System.Net.Http;

namespace blazor.jwttest.Client.Exceptions
{
  public class HttpForbiddenException : HttpRequestException
  {
    public HttpForbiddenException() : base("403 (Forbidden)") { }
  }
}
