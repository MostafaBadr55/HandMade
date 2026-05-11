using System;
using System.Collections.Generic;
using System.Text;

namespace HandMade.Application.Shared
{
    public class RepoResult<T>
    {
        public bool IsSuccess { get; private set; }
        public T? Value { get; private set; }
        public List<string> Errors { get; private set; } = [];

        private RepoResult() { }

        public static RepoResult<T> Success(T value) => new()
        {
            IsSuccess = true,
            Value = value
        };

        public static RepoResult<T> Failure(List<string> errors) => new()
        {
            IsSuccess = false,
            Errors = errors
        };

        public static RepoResult<T> Failure(string error) => Failure([error]);
    }
}
