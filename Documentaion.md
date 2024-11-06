<h1>Documentation</h1>

**Notes**
<br>
CSharp + yaml => Shaml

**Features:**

The C language has static typing, and the data types in the `shaml` configuration file will depend on the data types of the object's properties.

```
person:
  name: John
  age: 18
```

<p>First case value 18 will be convert to <b>int</b> type.</p>

```
class Person
{
  public string Name { get; set; }
  public int Age { get; set; }
}
```

Example template **scalar to scalar**

```
text: scalar
foo: ${text}

result: foo => "scalar"
```



<h1>Motivation</h1>





```
text: scalar
foo1: $"Logging text in database: parameter1: {text} parameter2: {text}"
foo2: $"Logging text in database: parameter1: {{text}} parameter2: {{text}}"
foo3:
	$"""
	Logging text in database:
		parameter1: {text} parameter2: {text}
	"""
foo4:
	"""
	Logging text in database:
		parameter1: ${text} parameter2: ${text}
	"""

result: foo => "scalar"
```

Model

```
public class Sample
{
	public Person Person { get; set; }
	public string Welcome { get; set; }
	public string Text { get; set; }
	public string Foo { get; set; }
}
```
Shaml
```
Person:
	Name: John
	Age: 18
welcome: welcome to company, ${Person.Name}
text: scalar
foo: Logging text in database: parameter1: ${text} parameter2: ${text}

result: foo => "scalar"
```

<h2>Deserialize vs Assign</h2>

<h3>Deserialize</h3>

```
Sample sample = ShamlConverter.Deserialize<Sample>(buffer);

Console.WriteLine(sample.Foo);
Console.WriteLine(sample.Welcome);

// Logging text in database: parameter1: scalar parameter2: scalar
```

<h3>Assign</h3>

```
Sample sample = ShamlConverter.Deserialize<Sample>(buffer);

Console.WriteLine(sample.Foo);
Console.WriteLine(sample.Welcome);


Shaml shaml = new Shaml(buffer, sample);
shaml.SetValue("Patrick", "Person.Name");
shaml.Assign();


Shaml shaml = Shaml.Parse(buffer);


shaml.SetValue("Patrick", "Person.Name");

shaml.SelectToken("Person.Name").SetValue("Patrick");
shaml.Assign(sample);

ShamlToken personName_token = shaml.SelectToken("person.name");
personName_token = "Patrick";

personName_token.Assign(sample);

Console.WriteLine(sample.Welcome);

// Logging text in database: parameter1: scalar parameter2: scalar
// welcome to company, John
// welcome to company, Patrick
```

