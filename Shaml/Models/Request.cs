namespace Shaml.Models
{
	public class Request
	{
		public DateTime Url { get; set; }
		public Dictionary<string, string> Query { get; set; }
		public Dictionary<string, string> Headers { get; set; }
		public string Body { get; set; }
	}
}