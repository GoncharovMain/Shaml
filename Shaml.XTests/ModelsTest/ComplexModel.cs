namespace Shaml.XTests.ModelsTest;

public class User
{
    public string Name { get; set; }
    public int Age { get; set; }
}

public class ComplexModel
{
    public User User { get; set; }
    public Dictionary<string, string> Dictionary { get; set; }
    public List<string> List { get; set; }
}