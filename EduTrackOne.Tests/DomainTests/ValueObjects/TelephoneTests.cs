using EduTrackOne.Domain.Eleves;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Tests.DomainTests.ValueObjects
{
    public class TelephoneTests
    {
        [Theory]
        [InlineData("+41791234567")]
        [InlineData("+41000000000")]
        public void Constructeur_Valide_Doit_CréerUneInstance(string valeur)
        {
            // Act
            var telephone = new Telephone(valeur);

            // Assert
            telephone.Value.Should().Be(valeur);
            telephone.ToString().Should().Be(valeur);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("41791234567")]      // manque le "+"
        [InlineData("+33123456789")]     // ne commence pas par +41
        [InlineData("+4179123456")]      // trop court (11 caractères au lieu de 12)
        [InlineData("+417912345678")]    // trop long (13 caractères au lieu de 12)
        [InlineData("+41abcdefghi")]     // contient des lettres
        [InlineData("+41 791234567")]    // espace non autorisé
        public void Constructeur_Invalide_Doit_LeverArgumentException(string valeur)
        {
            // Act
            Action act = () => new Telephone(valeur);

            // Assert
            act.Should().Throw<ArgumentException>()
               .WithMessage("Le numéro de téléphone doit commencer par +41 et être composé de 11 chiffres.");
        }

    }
}
