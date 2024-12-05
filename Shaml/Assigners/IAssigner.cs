using System.Diagnostics.CodeAnalysis;

namespace Shaml.Assigners;

public interface IAssigner
{
    void Assign([NotNull] ref object instance);
}