using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using EduTrackOne.Domain.Abstractions;

namespace EduTrackOne.Domain.Eleves
{
    public class Email : ValueObject
    {
        public string Value { get; }
        protected Email() { }
        public Email(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || !IsValidEmail(value))
            {
                throw new ArgumentException("L'email fourni n'est pas valide.");
            }

            Value = value;
        }

        private bool IsValidEmail(string email)
        {
            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            return emailRegex.IsMatch(email);
        }

        public override string ToString() => Value;

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
