namespace Shaml.Extension;

public static class DictionaryExtensions
{
    public static bool HasCyclicReferences<T>(Dictionary<T, T[]> tree) where T : notnull
    {
        HashSet<T> visited = new(tree.Comparer);
        HashSet<T> recursionStack = new(tree.Comparer);

        foreach (T node in tree.Keys)
        {
            if (HasCycle(node, tree, visited, recursionStack))
            {
                return true;
            }
        }

        return false;
    }

    private static bool HasCycle<T>(T node, Dictionary<T, T[]> tree, HashSet<T> visited, HashSet<T> recursionStack) where T : notnull
    {
        if (recursionStack.Contains(node))
        {
            return true;
        }

        if (visited.Contains(node))
        {
            return false;
        }

        visited.Add(node);
        recursionStack.Add(node);

        if (tree.ContainsKey(node))
        {
            foreach (T child in tree[node])
            {
                if (HasCycle(child, tree, visited, recursionStack))
                {
                    return true;
                }
            }
        }

        recursionStack.Remove(node);
        return false;
    }
}