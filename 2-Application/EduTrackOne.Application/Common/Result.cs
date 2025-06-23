using System;

namespace EduTrackOne.Application.Common
{
    public class Result<T>
    {
       
        public bool IsSuccess { get; }
        public T? Value { get; }
        public string? Error { get; }

       
        private Result(T? value, bool isSuccess, string? error)
        {
            Value = value;
            IsSuccess = isSuccess;
            Error = error;
        }

       
        // Méthode statique pour succès
        public static Result<T> Success(T value) =>
            new Result<T>(value, true, null); // ← ici l'erreur est null

        // Méthode statique pour échec
        public static Result<T> Failure(string error) =>
            new Result<T>(default, false, error);
    }
}
