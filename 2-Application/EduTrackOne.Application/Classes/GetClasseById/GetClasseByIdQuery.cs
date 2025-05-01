using EduTrackOne.Application.Common;
using EduTrackOne.Domain.Classes;
using MediatR;
using System;

namespace EduTrackOne.Application.Classes.GetClasseById
{
    public record GetClasseByIdQuery(Guid Id) : IRequest<Result<ClasseDto>>;
}
