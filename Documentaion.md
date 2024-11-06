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