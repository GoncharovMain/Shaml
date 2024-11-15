using Shaml.XTests.ModelsTest;
using Xunit;

namespace Shaml.XTests;

public class ShamlTests
{
    [Fact]
    public void PrimitiveTypeTest()
    {
        // Arrange
        using StreamReader reader = new("ShamlModel/PrimitiveTypes.shaml");

        ReadOnlyMemory<char> buffer = reader.ReadToEnd().AsMemory();
        
        // Act
        PrimitiveTypes primitiveTypes = ShamlConverter.Deserialize<PrimitiveTypes>(buffer);
        
        // Assert
        Assert.Equal(200, primitiveTypes.ByteProperty);
        Assert.Equal(200, primitiveTypes.ByteField);
        Assert.Equal(8871, primitiveTypes.UShortProperty);
        Assert.Equal(8871, primitiveTypes.UShortField);
        Assert.Equal(8871, primitiveTypes.ShortProperty);
        Assert.Equal(8871, primitiveTypes.ShortField);
        Assert.Equal(0xffffffff, primitiveTypes.UIntProperty);
        Assert.Equal(0xffffffff, primitiveTypes.UIntField);
        Assert.Equal(0x7fffffff, primitiveTypes.IntProperty);
        Assert.Equal(0x7fffffff, primitiveTypes.IntField);
        Assert.Equal(0xffffffffffffffffL, primitiveTypes.ULongProperty);
        Assert.Equal(0xffffffffffffffffL, primitiveTypes.ULongField);
        Assert.Equal(9223372036854775806, primitiveTypes.LongProperty);
        Assert.Equal(9223372036854775805, primitiveTypes.LongField);
        
        Assert.Equal(2.7181999683380127, primitiveTypes.FloatProperty);
        Assert.Equal(3.1414999961853027, primitiveTypes.FloatField);
        Assert.Equal(2.718281828459045, primitiveTypes.DoubleProperty);
        Assert.Equal(3.141592653589793, primitiveTypes.DoubleField);
        Assert.Equal(2.718281828459045m, primitiveTypes.DecimalProperty);
        Assert.Equal(3.141592653589793m, primitiveTypes.DecimalField);
        
        Assert.Equal(true, primitiveTypes.BoolProperty);
        Assert.Equal(false, primitiveTypes.BoolField);
        Assert.Equal(new DateTime(2015, 07, 20, 0, 0, 0), primitiveTypes.DateTimeProperty);
        Assert.Equal(new DateTime(2015, 07, 20, 0, 0, 0), primitiveTypes.DateTimeField);
        Assert.Equal("Hello, Shaml property", primitiveTypes.StringProperty);
        Assert.Equal("Hello, Shaml field", primitiveTypes.StringField);
        
        Assert.Equal('$', primitiveTypes.CharProperty);
        Assert.Equal('$', primitiveTypes.CharField);
        
    }
}
