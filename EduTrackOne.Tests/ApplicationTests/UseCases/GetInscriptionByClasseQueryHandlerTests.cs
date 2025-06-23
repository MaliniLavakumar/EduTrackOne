using EduTrackOne.Application.Inscriptions.GetInscriptionsByClasse;
using EduTrackOne.Domain.Eleves;
using EduTrackOne.Domain.Inscriptions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Tests.ApplicationTests.UseCases
{
    public class GetInscriptionByClasseQueryHandlerTests
    {
        [Fact]
        public async Task Handle_ShouldReturnInscriptions_WhenClasseIdIsValid()
        {
            // Arrange
            var classeId = Guid.NewGuid();
            var eleveId = Guid.NewGuid();
            var inscriptionId = Guid.NewGuid();
            var dateDebut = new DateOnly(2024, 9, 1);
            var dateFin = new DateOnly(2025, 6, 30);
            var periode = new DateInscriptionPeriode(
                dateDebut.ToDateTime(TimeOnly.MinValue),
                dateFin.ToDateTime(TimeOnly.MinValue)
                );


            var eleve = new Eleve(
                eleveId,
                new NomComplet("Jean", "Dupont"),
                new DateNaissance(new DateTime(2010, 5, 20)),
                new Sexe(Sexe.SexeType.Garçon),
                new Adresse("10 rue des Lilas", "75000", "Paris"),
                new Email("parent.jean@example.com"),
                new Telephone("+41783450912"),
                null, 
                "ELV12345"
            );
            var inscription = new Inscription(inscriptionId, periode, classeId, eleveId);
            inscription.SetEleve(eleve); // Tu dois ajouter cette méthode si elle n’existe pas

            var repoMock = new Mock<IInscriptionRepository>();
            repoMock.Setup(r => r.GetByClasseAsync(classeId, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new List<Inscription> { inscription });

            var validatorMock = new Mock<IValidator<GetInscriptionsByClasseQuery>>();
            validatorMock.Setup(v => v.ValidateAsync(It.IsAny<GetInscriptionsByClasseQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new ValidationResult());

            var handler = new GetInscriptionsByClasseQueryHandler(repoMock.Object, validatorMock.Object);
            var query = new GetInscriptionsByClasseQuery(classeId);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Single(result.Value);
            Assert.Equal(eleveId, result.Value[0].EleveId);
            Assert.Equal("Jean Dupont", result.Value[0].EleveNom);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenValidationFails()
        {
            // Arrange
            var query = new GetInscriptionsByClasseQuery(Guid.Empty);

            var validatorMock = new Mock<IValidator<GetInscriptionsByClasseQuery>>();
            validatorMock.Setup(v => v.ValidateAsync(It.IsAny<GetInscriptionsByClasseQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new ValidationResult(new List<ValidationFailure> {
                            new ValidationFailure("ClasseId", "ClasseId invalide")
                         }));

            var repoMock = new Mock<IInscriptionRepository>();

            var handler = new GetInscriptionsByClasseQueryHandler(repoMock.Object, validatorMock.Object);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Validation échouée", result.Error);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenNoInscriptionsFound()
        {
            // Arrange
            var classeId = Guid.NewGuid();
            var repoMock = new Mock<IInscriptionRepository>();
            repoMock.Setup(r => r.GetByClasseAsync(classeId, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new List<Inscription>());

            var validatorMock = new Mock<IValidator<GetInscriptionsByClasseQuery>>();
            validatorMock.Setup(v => v.ValidateAsync(It.IsAny<GetInscriptionsByClasseQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new ValidationResult());

            var handler = new GetInscriptionsByClasseQueryHandler(repoMock.Object, validatorMock.Object);
            var query = new GetInscriptionsByClasseQuery(classeId);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Empty(result.Value);
        }

    }
}
