namespace CourseWebsiteDotNet.Models
{
    public class Response
    {
        public bool state { get; set; }
        public string message { get; set; }
        public int? insertedId { get; set; }
        public int? effectedRows { get; set; }

    }
}
