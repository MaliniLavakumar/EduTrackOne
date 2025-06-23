using Castle.Core.Logging;
using EduTrackOne.Application.Classes.AddInscription;
using EduTrackOne.Application.Common.Interfaces;
using EduTrackOne.Application.Utilisateurs.DeleteUser;
using EduTrackOne.Domain.Abstractions;
using EduTrackOne.Domain.Utilisateurs;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Tests.ApplicationTests.UseCases
{
    public class DeleteUserCommandHandlerTests
    {
        private readonly Mock<IUtilisateurRepository> _mockRepo;
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly Mock<ICurrentUserService> _mockCurrentUser;
        private readonly Mock<IMediator> _mockMediator;
        private readonly DeleteUserCommandHandler _handler;
        private readonly Mock<ILogger<DeleteUserCommandHandler>> _mockLogger;

        public DeleteUserCommandHandlerTests()
        {
            _mockRepo = new Mock<IUtilisateurRepository>();
            _mockUow = new Mock<IUnitOfWork>();
            _mockCurrentUser = new Mock<ICurrentUserService>();
            _mockMediator = new Mock<IMediator>();
            _mockLogger = new Mock<ILogger<DeleteUserCommandHandler>>();


            _handler = new DeleteUserCommandHandler(
                _mockRepo.Object,
                _mockUow.Object,
                _mockCurrentUser.Object,
                _mockMediator.Object,
                _mockLogger.Object

            );
        }
    }
}
