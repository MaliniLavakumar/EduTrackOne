namespace EduTrackOne.Contracts.DTOs
{
    public record NoteForEleveDto(
        Guid EleveId,
        Guid MatiereId,
        double? Valeur,
        string? Commentaire
    );

    public record AddNotesForClasseDto(
        Guid ClasseId,
        DateTime DateExamen,
        List<NoteForEleveDto> Notes
    );

}
