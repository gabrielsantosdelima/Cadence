namespace Cadence.Practice.Domain.Ids
{
    public readonly record struct SessionId(Guid Value)
    {
        public static SessionId New() => new(Guid.CreateVersion7());
        public static SessionId From(Guid value) => new(value);
        public override string ToString() => Value.ToString();
    };
}
