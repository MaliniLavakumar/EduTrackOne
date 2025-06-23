using EduTrackOne.Domain.Notes;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Tests.DomainTests.ValueObjects
{
    public class ValeurNoteTests
    {
        [Theory]
        [InlineData(1.0)]
        [InlineData(1.5)]
        [InlineData(3.0)]
        [InlineData(5.5)]
        [InlineData(6.0)]
        public void Constructeur_Valide_Doit_CréerUneInstance(double valeur)
        {
            // Act
            var note = new ValeurNote(valeur);

            // Assert
            note.Value.Should().Be(valeur);
            note.EstAbsent.Should().BeFalse();
            note.ToString().Should().Be(valeur.ToString("0.0"));
        }

        [Theory]
        [InlineData(0.5)]
        [InlineData(6.5)]
        [InlineData(2.3)]
        public void Constructeur_Invalide_Doit_LeverArgumentOutOfRangeException(double valeur)
        {
            // Act
            Action act = () => new ValeurNote(valeur);

            // Assert
            act.Should().Throw<ArgumentOutOfRangeException>()
               .Where(ex => ex.ParamName == "value")
               .WithMessage("La note doit être comprise entre 1 et 6.*");
        }

        [Fact]
        public void Factory_Absent_Doit_RetournerInstanceAbsent()
        {
            // Act
            var absentNote = ValeurNote.Absent();

            // Assert
            absentNote.EstAbsent.Should().BeTrue();
            absentNote.Value.Should().BeNull();
            absentNote.ToString().Should().Be("Absent");
        }
    }
}
