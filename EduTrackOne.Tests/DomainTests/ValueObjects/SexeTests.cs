using EduTrackOne.Domain.Eleves;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Tests.DomainTests.ValueObjects
{
    public class SexeTests
    {
        [Theory]
        [InlineData(Sexe.SexeType.Garçon)]
        [InlineData(Sexe.SexeType.Fille)]
        public void Constructeur_Valide_Doit_CréerUneInstance(Sexe.SexeType valeur)
        {
            // Act
            var sexe = new Sexe(valeur);

            // Assert
            sexe.Value.Should().Be(valeur);
            sexe.ToString().Should().Be(valeur.ToString());
        }

        [Fact]
        public void Constructeur_Invalide_Doit_LeverArgumentException()
        {
            // Simuler un cast hors-énum
            Sexe.SexeType invalide = (Sexe.SexeType)(-1);

            // Act
            Action act = () => new Sexe(invalide);

            // Assert
            act.Should().Throw<ArgumentException>()
               .WithMessage("Valeur invalide pour le sexe.");
        }
    }
}
