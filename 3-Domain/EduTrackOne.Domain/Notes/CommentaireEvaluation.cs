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
        public string? Commentaire { get; }

        public CommentaireEvaluation(string? commentaire)
        {
            Commentaire = string.IsNullOrWhiteSpace(commentaire) ? null : commentaire.Trim();
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Commentaire ?? string.Empty;
        }

        public override string ToString() => Commentaire ?? string.Empty;
    }
}
