namespace blazor.jwttest.Shared
{
  public class Todo : IViewModel
  {
    public int Id { get; set; }
    public string Title { get; set; }
    public string FullDescription { get; set; }
    public bool Done { get; set; }

  }
}
