namespace CourseWebsiteDotNet.Models
{
    public class Response
    {
        public bool State { get; set; }
        public string Message { get; set; }
        public int? InsertedId { get; set; }
        public int? EffectedRows { get; set; }

    }
}
