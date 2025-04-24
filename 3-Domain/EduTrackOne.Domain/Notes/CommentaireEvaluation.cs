using EduTrackOne.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Domain.Notes
{
    public class CommentaireEvaluation : ValueObject
    {
        public string? Value { get; }
       protected CommentaireEvaluation() { }
        public CommentaireEvaluation(string? value)
        {
            Value = string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value ?? string.Empty;
        }

        public override string ToString() => Value ?? string.Empty;
    }
}
