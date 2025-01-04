using System.Diagnostics.CodeAnalysis;
using Shaml.Tokens;

namespace Shaml.Assigners;

public interface IAssigner
{
    Cache Cache { get; }
    void Assign([NotNull] ref object instance);
    void InitializeContext(string pathRoot, Dictionary<string, Cache> globalContext);
}