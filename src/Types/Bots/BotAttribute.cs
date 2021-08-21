namespace Sequence
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class BotAttribute : Attribute
    {
        public BotAttribute(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name must not be empty or white space.", nameof(name));
            }

            Name = name;
        }

        public string Name { get; }
    }
}
