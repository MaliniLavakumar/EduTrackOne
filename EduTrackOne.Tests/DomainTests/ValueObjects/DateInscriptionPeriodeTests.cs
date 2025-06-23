using EduTrackOne.Domain.Inscriptions;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Tests.DomainTests.ValueObjects
{
    public class DateInscriptionPeriodeTests
    {
        [Fact]
        public void Constructeur_DateFinAvantDateDebut_Doit_LeverArgumentException()
        {
            // Arrange
            DateTime debut = new DateTime(2025, 06, 10);
            DateTime fin = new DateTime(2025, 06, 09);

            // Act
            Action act = () => new DateInscriptionPeriode(debut, fin);

            // Assert
            act.Should().Throw<ArgumentException>()
               .WithMessage("La date de fin ne peut pas être avant la date de début.");
        }

        [Fact]
        public void Constructeur_ValideSansDateFin_Doit_CréerInstancetEtÊtreActive()
        {
            // Arrange
            DateTime debut = DateTime.UtcNow.Date.AddDays(-1);

            // Act
            var periode = new DateInscriptionPeriode(debut, null);

            // Assert
            periode.DateDebut.Should().Be(debut);
            periode.DateFin.Should().BeNull();
            periode.EstActive().Should().BeTrue();
        }

        [Fact]
        public void EstActive_DateFinPassée_Doit_RetournerFalse()
        {
            // Arrange
            DateTime debut = new DateTime(2024, 01, 01);
            DateTime fin = DateTime.UtcNow.Date.AddDays(-1);

            var periode = new DateInscriptionPeriode(debut, fin);

            // Act
            bool active = periode.EstActive();

            // Assert
            active.Should().BeFalse();
        }

        [Fact]
        public void EstActive_DateFinFuture_Doit_RetournerTrue()
        {
            // Arrange
            DateTime debut = new DateTime(2025, 01, 01);
            DateTime fin = DateTime.UtcNow.Date.AddDays(10);

            var periode = new DateInscriptionPeriode(debut, fin);

            // Act
            bool active = periode.EstActive();

            // Assert
            active.Should().BeTrue();
        }
    }
}

