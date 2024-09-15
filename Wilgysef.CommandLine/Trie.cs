namespace Wilgysef.CommandLine;

/// <summary>
/// Trie.
/// </summary>
/// <typeparam name="T">Value type.</typeparam>
internal class Trie<T>
{
    private readonly Node _root = new();

    /// <summary>
    /// Adds the value to the trie.
    /// </summary>
    /// <param name="key">Key.</param>
    /// <param name="value">Value.</param>
    public void Add(string key, T value)
    {
        var node = GetOrCreate(_root, key);
        node.AddLeaf(value);
    }

    /// <summary>
    /// Gets values by key.
    /// </summary>
    /// <param name="lookup">Key lookup.</param>
    /// <param name="prefix">Prefix length of key match.</param>
    /// <returns>Values.</returns>
    public IEnumerable<T>? GetValues(string lookup, out int? prefix)
    {
        var nodes = new Stack<Node>();
        nodes.Push(_root);

        foreach (var ch in lookup)
        {
            var next = nodes.Peek().Get(ch);
            if (next == null)
            {
                break;
            }

            nodes.Push(next);
        }

        while (nodes.Count > 0)
        {
            var node = nodes.Pop();
            if (node.LeafValues.Count > 0)
            {
                prefix = nodes.Count;
                return node.LeafValues;
            }
        }

        prefix = null;
        return null;
    }

    private static Node GetOrCreate(Node root, ReadOnlySpan<char> chars)
    {
        var current = root;

        for (var i = 0; i < chars.Length; i++)
        {
            current = current.GetOrCreate(chars[i]);
        }

        return current;
    }

    private class Node
    {
        private readonly Dictionary<char, Node> _children = [];

        public List<T> LeafValues { get; } = [];

        public Node? Get(char ch)
        {
            if (!_children.TryGetValue(ch, out var node))
            {
                return null;
            }

            return node;
        }

        public Node GetOrCreate(char ch)
        {
            if (!_children.TryGetValue(ch, out var node))
            {
                node = new Node();
                _children[ch] = node;
            }

            return node;
        }

        public void AddLeaf(T value)
        {
            LeafValues.Add(value);
        }
    }
}
