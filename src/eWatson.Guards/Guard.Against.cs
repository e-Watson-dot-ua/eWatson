using eWatson.Guards.Exceptions;
using eWatson.Guards.Resources;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace eWatson.Guards;

public static partial class Guard
{
    /// <summary>
    /// Provides guard clause validation methods.
    /// </summary>
    public static class Against
    {
        private static string Unknown => GuardMessages.UnknownParameter;

        /// <summary>
        /// Throws <see cref="GuardException"/> if the argument is null.
        /// </summary>
        public static void Null<T>(
            [NotNull] T? argument,
            [CallerArgumentExpression(nameof(argument))] string? parameterName = null)
        {
            if (argument is null)
            {
                throw new GuardException(
                    GuardMessages.ParameterCannotBeNull(parameterName ?? Unknown));
            }
        }

        /// <summary>
        /// Throws <see cref="GuardException"/> if the string is null or empty.
        /// </summary>
        public static void NullOrEmpty(
            [NotNull] string? argument,
            [CallerArgumentExpression(nameof(argument))] string? parameterName = null)
        {
            if (string.IsNullOrEmpty(argument))
            {
                throw new GuardException(
                    GuardMessages.ParameterCannotBeNullOrEmpty(
                        parameterName ?? Unknown));
            }
        }

        /// <summary>
        /// Throws <see cref="GuardException"/> if the string is null, empty, or
        /// consists only of white-space characters.
        /// </summary>
        public static void NullOrWhiteSpace(
            [NotNull] string? argument,
            [CallerArgumentExpression(nameof(argument))] string? parameterName = null)
        {
            if (string.IsNullOrWhiteSpace(argument))
            {
                throw new GuardException(
                    GuardMessages.ParameterCannotBeNullOrWhiteSpace(
                        parameterName ?? Unknown));
            }
        }

        /// <summary>
        /// Throws <see cref="GuardException"/> if the collection is null or empty.
        /// </summary>
        public static void NullOrEmpty<T>(
            [NotNull] IEnumerable<T>? argument,
            [CallerArgumentExpression(nameof(argument))] string? parameterName = null)
        {
            if (argument is null || !argument.Any())
            {
                throw new GuardException(
                    GuardMessages.ParameterCannotBeNullOrEmpty(
                        parameterName ?? Unknown));
            }
        }

        /// <summary>
        /// Throws <see cref="GuardException"/> if the number is negative.
        /// </summary>
        public static void Negative<T>(
            T argument,
            [CallerArgumentExpression(nameof(argument))] string? parameterName = null)
            where T : INumber<T>
        {
            if (argument < T.Zero)
            {
                throw new GuardException(
                    GuardMessages.ParameterCannotBeNegative(
                        parameterName ?? Unknown,
                        argument));
            }
        }

        /// <summary>
        /// Throws <see cref="GuardException"/> if the number is negative or zero.
        /// </summary>
        public static void NegativeOrZero<T>(
            T argument,
            [CallerArgumentExpression(nameof(argument))] string? parameterName = null)
            where T : INumber<T>
        {
            if (argument <= T.Zero)
            {
                throw new GuardException(
                    GuardMessages.ParameterMustBeGreaterThanZero(
                        parameterName ?? Unknown,
                        argument));
            }
        }

        /// <summary>
        /// Throws <see cref="GuardException"/> if the number is out of the
        /// specified range.
        /// </summary>
        public static void OutOfRange<T>(
            T argument,
            T min,
            T max,
            [CallerArgumentExpression(nameof(argument))] string? parameterName = null)
            where T : IComparable<T>
        {
            if (argument.CompareTo(min) < 0 || argument.CompareTo(max) > 0)
            {
                throw new GuardException(
                    GuardMessages.ParameterMustBeBetween(
                        parameterName ?? Unknown,
                        min,
                        max,
                        argument));
            }
        }

        /// <summary>
        /// Throws <see cref="GuardException"/> if the value is equal to the
        /// invalid value.
        /// </summary>
        public static void InvalidValue<T>(
            T argument,
            T invalidValue,
            [CallerArgumentExpression(nameof(argument))] string? parameterName = null)
            where T : notnull
        {
            if (EqualityComparer<T>.Default.Equals(argument, invalidValue))
            {
                throw new GuardException(
                    GuardMessages.ParameterCannotBeValue(
                        parameterName ?? Unknown,
                        invalidValue));
            }
        }

        /// <summary>
        /// Throws <see cref="GuardException"/> if the Guid is empty.
        /// </summary>
        public static void EmptyGuid(
            Guid argument,
            [CallerArgumentExpression(nameof(argument))] string? parameterName = null)
        {
            if (argument == Guid.Empty)
            {
                throw new GuardException(
                    GuardMessages.ParameterCannotBeEmptyGuid(
                        parameterName ?? Unknown));
            }
        }

        /// <summary>
        /// Throws <see cref="GuardException"/> if the condition is false.
        /// </summary>
        public static void False(bool condition, string message)
        {
            if (!condition)
            {
                throw new GuardException(message);
            }
        }

        /// <summary>
        /// Throws <see cref="GuardException"/> if the condition is true.
        /// </summary>
        public static void True(bool condition, string message)
        {
            if (condition)
            {
                throw new GuardException(message);
            }
        }

        /// <summary>
        /// Throws <see cref="GuardException"/> if the string exceeds the maximum length.
        /// </summary>
        public static void StringTooLong(
            string argument,
            int maxLength,
            [CallerArgumentExpression(nameof(argument))] string? parameterName = null)
        {
            if (argument.Length > maxLength)
            {
                throw new GuardException(
                    GuardMessages.ParameterExceedsMaxLength(
                        parameterName ?? Unknown,
                        maxLength,
                        argument.Length));
            }
        }

        /// <summary>
        /// Throws <see cref="GuardException"/> if the string is shorter than the minimum length.
        /// </summary>
        public static void StringTooShort(
            string argument,
            int minLength,
            [CallerArgumentExpression(nameof(argument))] string? parameterName = null)
        {
            if (argument.Length < minLength)
            {
                throw new GuardException(
                    GuardMessages.ParameterBelowMinLength(
                        parameterName ?? Unknown,
                        minLength,
                        argument.Length));
            }
        }
    }
}
