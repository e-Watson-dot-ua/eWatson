using eWatson.Guards.Exceptions;
using eWatson.Guards.Resources;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace eWatson.Guards;

public static partial class Guard
{
    /// <summary>
    /// Provides guard clause validation methods.
    /// </summary>
    public static class Against
    {
        /// <summary>
        /// Throws <see cref="GuardException"/> if the argument is null.
        /// </summary>
        /// <typeparam name="T">The type of the argument.</typeparam>
        /// <param name="argument">The argument to check.</param>
        /// <param name="parameterName">The name of the parameter being checked.</param>
        /// <exception cref="GuardException">Thrown when the argument is null.</exception>
        public static void Null<T>(
            [NotNull] T? argument,
            [CallerArgumentExpression(nameof(argument))] string? parameterName = null)
        {
            if (argument is null)
            {
                throw new GuardException(
                    GuardMessages.ParameterCannotBeNull(parameterName ?? "unknown"));
            }
        }

        /// <summary>
        /// Throws <see cref="GuardException"/> if the string is null or empty.
        /// </summary>
        /// <param name="argument">The string to check.</param>
        /// <param name="parameterName">The name of the parameter being checked.</param>
        /// <exception cref="GuardException">
        /// Thrown when the string is null or empty.
        /// </exception>
        public static void NullOrEmpty(
            [NotNull] string? argument,
            [CallerArgumentExpression(nameof(argument))] string? parameterName = null)
        {
            if (string.IsNullOrEmpty(argument))
            {
                throw new GuardException(
                    GuardMessages.ParameterCannotBeNullOrEmpty(
                        parameterName ?? "unknown"));
            }
        }

        /// <summary>
        /// Throws <see cref="GuardException"/> if the string is null, empty, or
        /// consists only of white-space characters.
        /// </summary>
        /// <param name="argument">The string to check.</param>
        /// <param name="parameterName">The name of the parameter being checked.</param>
        /// <exception cref="GuardException">
        /// Thrown when the string is null, empty, or whitespace.
        /// </exception>
        public static void NullOrWhiteSpace(
            [NotNull] string? argument,
            [CallerArgumentExpression(nameof(argument))] string? parameterName = null)
        {
            if (string.IsNullOrWhiteSpace(argument))
            {
                throw new GuardException(
                    GuardMessages.ParameterCannotBeNullOrWhiteSpace(
                        parameterName ?? "unknown"));
            }
        }

        /// <summary>
        /// Throws <see cref="GuardException"/> if the collection is null or empty.
        /// </summary>
        /// <typeparam name="T">The type of elements in the collection.</typeparam>
        /// <param name="argument">The collection to check.</param>
        /// <param name="parameterName">The name of the parameter being checked.</param>
        /// <exception cref="GuardException">
        /// Thrown when the collection is null or empty.
        /// </exception>
        public static void NullOrEmpty<T>(
            [NotNull] IEnumerable<T>? argument,
            [CallerArgumentExpression(nameof(argument))] string? parameterName = null)
        {
            if (argument is null || !argument.Any())
            {
                throw new GuardException(
                    GuardMessages.ParameterCannotBeNullOrEmpty(
                        parameterName ?? "unknown"));
            }
        }

        /// <summary>
        /// Throws <see cref="GuardException"/> if the number is negative.
        /// </summary>
        /// <param name="argument">The number to check.</param>
        /// <param name="parameterName">The name of the parameter being checked.</param>
        /// <exception cref="GuardException">Thrown when the number is negative.</exception>
        public static void Negative(
            int argument,
            [CallerArgumentExpression(nameof(argument))] string? parameterName = null)
        {
            if (argument < 0)
            {
                throw new GuardException(
                    GuardMessages.ParameterCannotBeNegative(
                        parameterName ?? "unknown",
                        argument));
            }
        }

        /// <summary>
        /// Throws <see cref="GuardException"/> if the number is negative.
        /// </summary>
        /// <param name="argument">The number to check.</param>
        /// <param name="parameterName">The name of the parameter being checked.</param>
        /// <exception cref="GuardException">Thrown when the number is negative.</exception>
        public static void Negative(
            long argument,
            [CallerArgumentExpression(nameof(argument))] string? parameterName = null)
        {
            if (argument < 0)
            {
                throw new GuardException(
                    GuardMessages.ParameterCannotBeNegative(
                        parameterName ?? "unknown",
                        argument));
            }
        }

        /// <summary>
        /// Throws <see cref="GuardException"/> if the number is negative.
        /// </summary>
        /// <param name="argument">The number to check.</param>
        /// <param name="parameterName">The name of the parameter being checked.</param>
        /// <exception cref="GuardException">Thrown when the number is negative.</exception>
        public static void Negative(
            decimal argument,
            [CallerArgumentExpression(nameof(argument))] string? parameterName = null)
        {
            if (argument < 0)
            {
                throw new GuardException(
                    GuardMessages.ParameterCannotBeNegative(
                        parameterName ?? "unknown",
                        argument));
            }
        }

        /// <summary>
        /// Throws <see cref="GuardException"/> if the number is negative or zero.
        /// </summary>
        /// <param name="argument">The number to check.</param>
        /// <param name="parameterName">The name of the parameter being checked.</param>
        /// <exception cref="GuardException">
        /// Thrown when the number is negative or zero.
        /// </exception>
        public static void NegativeOrZero(
            int argument,
            [CallerArgumentExpression(nameof(argument))] string? parameterName = null)
        {
            if (argument <= 0)
            {
                throw new GuardException(
                    GuardMessages.ParameterMustBeGreaterThanZero(
                        parameterName ?? "unknown",
                        argument));
            }
        }

        /// <summary>
        /// Throws <see cref="GuardException"/> if the number is negative or zero.
        /// </summary>
        /// <param name="argument">The number to check.</param>
        /// <param name="parameterName">The name of the parameter being checked.</param>
        /// <exception cref="GuardException">
        /// Thrown when the number is negative or zero.
        /// </exception>
        public static void NegativeOrZero(
            long argument,
            [CallerArgumentExpression(nameof(argument))] string? parameterName = null)
        {
            if (argument <= 0)
            {
                throw new GuardException(
                    GuardMessages.ParameterMustBeGreaterThanZero(
                        parameterName ?? "unknown",
                        argument));
            }
        }

        /// <summary>
        /// Throws <see cref="GuardException"/> if the number is negative or zero.
        /// </summary>
        /// <param name="argument">The number to check.</param>
        /// <param name="parameterName">The name of the parameter being checked.</param>
        /// <exception cref="GuardException">
        /// Thrown when the number is negative or zero.
        /// </exception>
        public static void NegativeOrZero(
            decimal argument,
            [CallerArgumentExpression(nameof(argument))] string? parameterName = null)
        {
            if (argument <= 0)
            {
                throw new GuardException(
                    GuardMessages.ParameterMustBeGreaterThanZero(
                        parameterName ?? "unknown",
                        argument));
            }
        }

        /// <summary>
        /// Throws <see cref="GuardException"/> if the number is out of the
        /// specified range.
        /// </summary>
        /// <param name="argument">The number to check.</param>
        /// <param name="min">The minimum allowed value (inclusive).</param>
        /// <param name="max">The maximum allowed value (inclusive).</param>
        /// <param name="parameterName">The name of the parameter being checked.</param>
        /// <exception cref="GuardException">Thrown when the number is out of range.</exception>
        public static void OutOfRange(
            int argument,
            int min,
            int max,
            [CallerArgumentExpression(nameof(argument))] string? parameterName = null)
        {
            if (argument < min || argument > max)
            {
                throw new GuardException(
                    GuardMessages.ParameterMustBeBetween(
                        parameterName ?? "unknown",
                        min,
                        max,
                        argument));
            }
        }

        /// <summary>
        /// Throws <see cref="GuardException"/> if the number is out of the
        /// specified range.
        /// </summary>
        /// <param name="argument">The number to check.</param>
        /// <param name="min">The minimum allowed value (inclusive).</param>
        /// <param name="max">The maximum allowed value (inclusive).</param>
        /// <param name="parameterName">The name of the parameter being checked.</param>
        /// <exception cref="GuardException">Thrown when the number is out of range.</exception>
        public static void OutOfRange(
            long argument,
            long min,
            long max,
            [CallerArgumentExpression(nameof(argument))] string? parameterName = null)
        {
            if (argument < min || argument > max)
            {
                throw new GuardException(
                    GuardMessages.ParameterMustBeBetween(
                        parameterName ?? "unknown",
                        min,
                        max,
                        argument));
            }
        }

        /// <summary>
        /// Throws <see cref="GuardException"/> if the number is out of the
        /// specified range.
        /// </summary>
        /// <param name="argument">The number to check.</param>
        /// <param name="min">The minimum allowed value (inclusive).</param>
        /// <param name="max">The maximum allowed value (inclusive).</param>
        /// <param name="parameterName">The name of the parameter being checked.</param>
        /// <exception cref="GuardException">Thrown when the number is out of range.</exception>
        public static void OutOfRange(
            decimal argument,
            decimal min,
            decimal max,
            [CallerArgumentExpression(nameof(argument))] string? parameterName = null)
        {
            if (argument < min || argument > max)
            {
                throw new GuardException(
                    GuardMessages.ParameterMustBeBetween(
                        parameterName ?? "unknown",
                        min,
                        max,
                        argument));
            }
        }

        /// <summary>
        /// Throws <see cref="GuardException"/> if the value is equal to the
        /// invalid value.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="argument">The value to check.</param>
        /// <param name="invalidValue">The invalid value to compare against.</param>
        /// <param name="parameterName">The name of the parameter being checked.</param>
        /// <exception cref="GuardException">
        /// Thrown when the value equals the invalid value.
        /// </exception>
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
                        parameterName ?? "unknown",
                        invalidValue));
            }
        }

        /// <summary>
        /// Throws <see cref="GuardException"/> if the Guid is empty.
        /// </summary>
        /// <param name="argument">The Guid to check.</param>
        /// <param name="parameterName">The name of the parameter being checked.</param>
        /// <exception cref="GuardException">Thrown when the Guid is empty.</exception>
        public static void EmptyGuid(
            Guid argument,
            [CallerArgumentExpression(nameof(argument))] string? parameterName = null)
        {
            if (argument == Guid.Empty)
            {
                throw new GuardException(
                    GuardMessages.ParameterCannotBeEmptyGuid(
                        parameterName ?? "unknown"));
            }
        }

        /// <summary>
        /// Throws <see cref="GuardException"/> if the condition is false.
        /// </summary>
        /// <param name="condition">The condition to check.</param>
        /// <param name="message">The error message if the condition is false.</param>
        /// <exception cref="GuardException">Thrown when the condition is false.</exception>
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
        /// <param name="condition">The condition to check.</param>
        /// <param name="message">The error message if the condition is true.</param>
        /// <exception cref="GuardException">Thrown when the condition is true.</exception>
        public static void True(bool condition, string message)
        {
            if (condition)
            {
                throw new GuardException(message);
            }
        }
    }
}
