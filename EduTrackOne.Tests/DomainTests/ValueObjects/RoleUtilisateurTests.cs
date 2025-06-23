using EduTrackOne.Domain.Utilisateurs;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Tests.DomainTests.ValueObjects
{
    public class RoleUtilisateurTests
    {
        [Theory]
        [InlineData(RoleUtilisateur.Role.Admin)]
        [InlineData(RoleUtilisateur.Role.Enseignant)]
        public void Constructeur_EnumValide_Doit_CréerInstance(RoleUtilisateur.Role role)
        {
            // Act
            var ru = new RoleUtilisateur(role);

            // Assert
            ru.Valeur.Should().Be(role);
            ru.ToString().Should().Be(role.ToString());
        }

        [Theory]
        [InlineData("Admin", RoleUtilisateur.Role.Admin)]
        [InlineData("enseignant", RoleUtilisateur.Role.Enseignant)]
        [InlineData("EnSeIgNaNt", RoleUtilisateur.Role.Enseignant)]
        public void Constructeur_StringValide_Doit_CréerInstance(string input, RoleUtilisateur.Role attendu)
        {
            // Act
            var ru = new RoleUtilisateur(input);

            // Assert
            ru.Valeur.Should().Be(attendu);
            ru.ToString().Should().Be(attendu.ToString());
            if (attendu == RoleUtilisateur.Role.Admin)
                ru.EstAdmin().Should().BeTrue();
            else
                ru.EstEnseignant().Should().BeTrue();
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("Manager")]
        [InlineData("Eleve")]
        public void Constructeur_StringInvalide_Doit_LeverArgumentException(string input)
        {
            // Act
            Action act = () => new RoleUtilisateur(input);

            // Assert
            act.Should().Throw<ArgumentException>()
               .WithMessage("Le rôle doit être 'Admin' ou 'Enseignant'.*");
        }
    }
}
