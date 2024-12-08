namespace Shaml.Models
{
	public class GoogleApi
	{
		public Request Request { get; set; }
		public User User { get; set; }
		public List<string> List { get; set; }
		public string Text { get; set; }

		public string Host { get; set; }
		public Request GoogleInitialize { get; set; }
		public Request GoogleUser { get; set; }
		public Request GooglePage { get; set; }
	}
}