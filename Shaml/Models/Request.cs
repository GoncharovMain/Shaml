namespace Shaml.Models
{
	public class Request
	{
		public string Url { get; set; }
		public string Method { get; set; }
		public string MimeType { get; set; }
		public Dictionary<string, string> Query { get; set; }
		public Dictionary<string, string> Headers { get; set; }
		public string Body { get; set; }
	}
}