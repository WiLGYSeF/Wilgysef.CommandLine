namespace Wilgysef.CommandLine;

internal class Trie<T>
{
    private readonly Node _root = Node.Root();

    public void Add(string key, T value)
    {
        if (key.Length == 0)
        {
            throw new ArgumentException("Key length must be greater than 0", nameof(key));
        }

        var node = GetOrCreate(_root, key);
        node.AddLeaf(value);
    }

    public IEnumerable<T>? GetValues(string str, out int? prefix)
    {
        var nodes = new Stack<Node>();
        nodes.Push(_root);

        foreach (var ch in str)
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

    public class Node
    {
        private readonly Dictionary<char, Node> _children = [];

        public Node(char value)
        {
            Value = value;
        }

        public char Value { get; }

        public List<T> LeafValues { get; } = [];

        public static Node Root()
        {
            return new Node((char)0);
        }

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
                node = new Node(ch);
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
