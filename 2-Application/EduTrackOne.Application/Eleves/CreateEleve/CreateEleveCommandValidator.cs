using EduTrackOne.Domain.Eleves;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Application.Eleves.CreateEleve
{
    public class CreateEleveCommandValidator : AbstractValidator<CreateEleveCommand>
    {
        public CreateEleveCommandValidator()
        {
            RuleFor(x => x.Prenom)
                .NotEmpty().WithMessage("Le prénom est requis.")
                .Length(2, 100).WithMessage("Le prénom doit comporter entre 2 et 100 caractères.");

            RuleFor(x => x.Nom)
                .NotEmpty().WithMessage("Le nom est requis.")
                .Length(2, 100).WithMessage("Le nom doit comporter entre 2 et 100 caractères.");

            RuleFor(x => x.DateNaissance)
                .LessThan(DateTime.Today).WithMessage("La date de naissance doit être antérieure à aujourd'hui.");

            RuleFor(x => x.Sexe)
                .NotEmpty().WithMessage("Le sexe est requis.")
                .Must(value =>
                {
                    // On vérifie que la string correspond à un type valide de ton VO Sexe
                    return Enum.TryParse<Sexe.SexeType>(value, ignoreCase: true, out _);
                })
                .WithMessage("Le sexe doit être 'Garçon' ou 'Fille'.");

            RuleFor(x => x.Rue)
                .NotEmpty().WithMessage("La rue est requise.");

            RuleFor(x => x.CodePostal)
                .NotEmpty().WithMessage("Le code postal est requis.")
                .Matches(@"^\d{4,5}$").WithMessage("Le code postal doit comporter 4 ou 5 chiffres.");

            RuleFor(x => x.Ville)
                .NotEmpty().WithMessage("La ville est requise.");

            RuleFor(x => x.EmailParent)
                .NotEmpty().WithMessage("L'email du parent est requis.")
                .EmailAddress().WithMessage("L'email du parent doit être valide.");

            RuleFor(x => x.Tel1)
                .NotEmpty().WithMessage("Le téléphone principal est requis.")
                .Must(value =>
                {
                    try { _ = new Telephone(value); return true; }
                    catch { return false; }
                })
                .WithMessage("Le téléphone principal doit commencer par +41 et contenir 12 caractères.");

            When(x => !string.IsNullOrEmpty(x.Tel2), () =>
            {
                RuleFor(x => x.Tel2!)
                    .Must(value =>
                    {
                        try { _ = new Telephone(value); return true; }
                        catch { return false; }
                    })
                    .WithMessage("Le téléphone secondaire doit commencer par +41 et contenir 13 caractères.");
                RuleFor(x => x.NoImmatricule)
                .NotEmpty().WithMessage("Le No.Immatricule est requis.");

            });
        }
    }
}


