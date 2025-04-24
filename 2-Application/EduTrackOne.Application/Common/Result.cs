using System;

namespace EduTrackOne.Application.Common
{
    public class Result<T>
    {
       
        public bool IsSuccess { get; }
        public T Value { get; }
        public string Error { get; }

        // Constructeur pour succès
        public Result(T value)
        {
            IsSuccess = true;
            Value = value;
            Error = string.Empty;
        }

        // Constructeur pour échec
        public Result(string error)
        {
            IsSuccess = false;
            Error = error;
            Value = default;
        }

        // Méthode statique pour retourner un succès avec valeur
        public static Result<T> Success(T value) => new Result<T>(value);

        // Méthode statique pour retourner une erreur
        public static Result<T> Failure(string error) => new Result<T>(error);
    }
}
