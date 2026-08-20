using Cadence.Practice.Domain.Ids;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Cadence.Practice.Infrastructure.Converters
{
    public sealed class SessionIdConverter : ValueConverter<SessionId, Guid>
    {
        public SessionIdConverter()
            : base(sessionId => sessionId.Value, value => SessionId.From(value))
        {
        }
    }

    public sealed class SessionIdComparer : ValueComparer<SessionId>
    {
        public SessionIdComparer()
            : base(
                (left, right) => left.Value == right.Value,
                sessionId => sessionId.Value.GetHashCode(),
                sessionId => SessionId.From(sessionId.Value))
        {
        }
    }
}
