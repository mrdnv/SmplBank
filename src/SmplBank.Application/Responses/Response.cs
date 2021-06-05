using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SmplBank.Application.Responses
{
    public record Response<T>
    {
        internal Response(params string[] errorMessages)
        {
            if (!errorMessages.Any())
                throw new ArgumentException("Error messages must be specified.");

            this.IsValid = false;
            this.ErrorMessages = errorMessages;
            this.Value = default;
        }

        internal Response(T value)
        {
            this.Value = value ?? throw new ArgumentNullException(nameof(value));
            this.IsValid = true;
            this.ErrorMessages = Enumerable.Empty<string>();
        }

        public IEnumerable<string> ErrorMessages { get; private set; }
        public bool IsValid { get; private set; }
        public T Value { get; private set; }
    }

    public record Response : Response<Unit>
    {
        internal Response(params string[] errorMessages) : base(errorMessages)
        {
        }

        internal Response() : base(Unit.Value)
        {
        }

        internal static Response Success() => new();
        internal static Response<T> Success<T>(T value) => new(value);
        internal static Response Fail(params string[] errorMessages) => new(errorMessages);
        internal static Response<T> Fail<T>(params string[] errorMessages) => new(errorMessages);
    }
}
