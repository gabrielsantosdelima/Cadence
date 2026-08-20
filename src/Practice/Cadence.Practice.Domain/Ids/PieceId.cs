namespace Cadence.Practice.Domain.Ids
{
    public readonly record struct PieceId(Guid Value)
    {
        public static PieceId New() => new(Guid.CreateVersion7());
        public static PieceId From(Guid value) => new(value);
        public override string ToString() => Value.ToString();
    };
}
