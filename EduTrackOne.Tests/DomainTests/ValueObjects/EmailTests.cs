using EduTrackOne.Domain.Eleves;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Tests.DomainTests.ValueObjects
{
    public class EmailTests
    {
        [Theory]
        [InlineData("sophie.durand@example.com")]
        [InlineData("jean.dupont@domain.fr")]
        [InlineData("user123@test.co.uk")]
        public void Constructeur_Valide_Doit_CréerUneInstance(string valeur)
        {
            // Act
            var email = new Email(valeur);

            // Assert
            email.Value.Should().Be(valeur);
            email.ToString().Should().Be(valeur);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("sans-at-symbole.com")]
        [InlineData("user@")]
        [InlineData("@domain.com")]
        [InlineData("user@domain")]
        [InlineData("user@domain.")]
        [InlineData("user@.com")]
        public void Constructeur_Invalide_Doit_LeverArgumentException(string valeur)
        {
            // Act
            Action act = () => new Email(valeur);

            // Assert
            act.Should().Throw<ArgumentException>()
               .WithMessage("L'email fourni n'est pas valide.");
        }

    }
}
