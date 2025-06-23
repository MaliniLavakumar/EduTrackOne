using EduTrackOne.Domain.Utilisateurs;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Tests.DomainTests.ValueObjects
{
    public class StatutUtilisateurTests
    {
        [Theory]
        [InlineData(StatutUtilisateur.StatutEnum.Actif)]
        [InlineData(StatutUtilisateur.StatutEnum.Inactif)]
        public void Constructeur_EnumValide_Doit_CréerInstance(StatutUtilisateur.StatutEnum valeur)
        {
            // Act
            var su = new StatutUtilisateur(valeur);

            // Assert
            su.Value.Should().Be(valeur);
            su.ToString().Should().Be(valeur.ToString());
        }

        [Theory]
        [InlineData("Actif", StatutUtilisateur.StatutEnum.Actif)]
        [InlineData("inactif", StatutUtilisateur.StatutEnum.Inactif)]
        [InlineData("INACTIF", StatutUtilisateur.StatutEnum.Inactif)]
        public void FromString_Valide_Doit_CréerInstance(string input, StatutUtilisateur.StatutEnum attendu)
        {
            // Act
            var su = StatutUtilisateur.FromString(input);

            // Assert
            su.Value.Should().Be(attendu);
            su.ToString().Should().Be(attendu.ToString());
        }

        [Theory]
        [InlineData("")]
        [InlineData("  ")]
        [InlineData("Unknown")]
        public void FromString_Invalide_Doit_LeverArgumentException(string input)
        {
            // Act
            Action act = () => StatutUtilisateur.FromString(input);

            // Assert
            act.Should().Throw<ArgumentException>()
               .WithMessage("Statut invalide. Doit être 'Actif' ou 'Inactif'.");
        }
    }

}
