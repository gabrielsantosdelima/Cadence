using System.Text.RegularExpressions;

namespace Cadence.Repertoire.Domain.ValueObjects
{
    public sealed class MusicalKey
    {
        private static readonly Regex Pattern = new(
            "^(?:[A-G])(?:#|b)?\\s(?:major|minor)$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public string Value { get; }

        private MusicalKey(string value)
        {
            Value = value;
        }

        public static Result<MusicalKey> Create(string value)
        {
            string trimValue = value.Trim();

            if (!Pattern.IsMatch(trimValue))
                return Result.Failure<MusicalKey>(
                    new Error
                    (
                        "MusicalKey.Invalid",
                        "MusicalKey must be a tonic (A-G) with optional accidental (#/b), followed by 'major' or 'minor'"
                    )
                );

            return Result.Success(new MusicalKey(trimValue));
        }
    }
}
