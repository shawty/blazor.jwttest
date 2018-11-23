using System.Net.Http;

namespace blazor.jwttest.Client.Exceptions
{
  public class HttpInternalServerErrorException : HttpRequestException
  {
    public HttpInternalServerErrorException() : base("500 (Internal Server Error)") { }
  }
}
