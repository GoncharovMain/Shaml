namespace Shaml.Models
{
	public interface IRequest
	{
		Task ExecuteAsync();
	}
	public class GetRequest : Request, IRequest
	{
		public Task ExecuteAsync()
		{
			throw new NotImplementedException();
		}
	}
	public class PostRequest : Request, IRequest
	{
		public IBody Body { get; set; }
		public Task ExecuteAsync()
		{
			throw new NotImplementedException();
		}
	}
	public class GoogleApi
	{
		public Request Request { get; set; }
		public User User { get; set; }
		public List<string> List { get; set; }
		public string Text { get; set; }

		public IRequest GoogleInitialize { get; set; }
		public IRequest GoogleUser { get; set; }
	}
}