using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using ChatNotifyService.ABS.Exceptions;

namespace ChatNotifyService.BLL.Helpers;

/// <summary>
/// Provides guard clauses for method parameter validation.
/// </summary>
public static class Guard
{
    /// <summary>
    /// Ensures the Guid is not empty.
    /// </summary>
    /// <exception cref="BadRequestException">Thrown when the Guid is empty.</exception>
    public static Guid AgainstEmptyGuid(
        Guid value,
        [CallerArgumentExpression(nameof(value))] string? parameterName = null)
    {
        if (value == Guid.Empty)
        {
            throw new BadRequestException($"{parameterName} cannot be empty.");
        }
        
        return value;
    }

    /// <summary>
    /// Ensures the string is not null or whitespace.
    /// </summary>
    /// <exception cref="BadRequestException">Thrown when the string is null or whitespace.</exception>
    public static string AgainstNullOrWhiteSpace(
        [NotNull] string? value,
        [CallerArgumentExpression(nameof(value))] string? parameterName = null)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new BadRequestException($"{parameterName} cannot be null or whitespace.");
        }
        
        return value;
    }

    /// <summary>
    /// Ensures the object is not null.
    /// </summary>
    /// <exception cref="BadRequestException">Thrown when the object is null.</exception>
    public static T AgainstNull<T>(
        [NotNull] T? value,
        [CallerArgumentExpression(nameof(value))] string? parameterName = null) where T : class
    {
        if (value == null)
        {
            throw new BadRequestException($"{parameterName} cannot be null.");
        }
        
        return value;
    }

    /// <summary>
    /// Ensures the nullable value type has a value.
    /// </summary>
    /// <exception cref="BadRequestException">Thrown when the nullable value is null.</exception>
    public static T AgainstNull<T>(
        [NotNull] T? value,
        [CallerArgumentExpression(nameof(value))] string? parameterName = null) where T : struct
    {
        if (!value.HasValue)
        {
            throw new BadRequestException($"{parameterName} cannot be null.");
        }
        
        return value.Value;
    }
    
    /// <summary>
    /// Ensures the string is not null or empty.
    /// </summary>
    /// <exception cref="BadRequestException">Thrown when the string is null or empty.</exception>
    public static string AgainstNullOrEmpty(
        [NotNull] string? value,
        [CallerArgumentExpression(nameof(value))] string? parameterName = null)
    {
        if (string.IsNullOrEmpty(value))
        {
            throw new BadRequestException($"{parameterName} cannot be null or empty.");
        }
        
        return value;
    }

    /// <summary>
    /// Ensures the collection is not null or empty.
    /// </summary>
    /// <exception cref="BadRequestException">Thrown when the collection is null or empty.</exception>
    public static IEnumerable<T> AgainstNullOrEmpty<T>(
        [NotNull] IEnumerable<T>? collection,
        [CallerArgumentExpression(nameof(collection))] string? parameterName = null)
    {
        if (collection == null || !collection.Any())
        {
            throw new BadRequestException($"{parameterName} cannot be null or empty.");
        }
        
        return collection;
    }

    /// <summary>
    /// Ensures the collection is not null or empty.
    /// </summary>
    /// <exception cref="BadRequestException">Thrown when the collection is null or empty.</exception>
    public static ICollection<T> AgainstNullOrEmpty<T>(
        [NotNull] ICollection<T>? collection,
        [CallerArgumentExpression(nameof(collection))] string? parameterName = null)
    {
        if (collection == null || collection.Count == 0)
        {
            throw new BadRequestException($"{parameterName} cannot be null or empty.");
        }
        
        return collection;
    }

    /// <summary>
    /// Ensures the array is not null or empty.
    /// </summary>
    /// <exception cref="BadRequestException">Thrown when the array is null or empty.</exception>
    public static T[] AgainstNullOrEmpty<T>(
        [NotNull] T[]? array,
        [CallerArgumentExpression(nameof(array))] string? parameterName = null)
    {
        if (array == null || array.Length == 0)
        {
            throw new BadRequestException($"{parameterName} cannot be null or empty.");
        }
        
        return array;
    }

    /// <summary>
    /// Ensures the condition is false (throws if true).
    /// </summary>
    /// <exception cref="BadRequestException">Thrown when the condition is true.</exception>
    public static void Against(
        bool condition,
        string message)
    {
        if (condition)
        {
            throw new BadRequestException(message);
        }
    }

    /// <summary>
    /// Ensures the value is within the specified range.
    /// </summary>
    /// <exception cref="BadRequestException">Thrown when the value is out of range.</exception>
    public static T AgainstOutOfRange<T>(
        T value,
        T min,
        T max,
        [CallerArgumentExpression(nameof(value))] string? parameterName = null) where T : IComparable<T>
    {
        if (value.CompareTo(min) < 0 || value.CompareTo(max) > 0)
        {
            throw new BadRequestException($"{parameterName} must be between {min} and {max}.");
        }
        
        return value;
    }

    /// <summary>
    /// Ensures the value is positive.
    /// </summary>
    /// <exception cref="BadRequestException">Thrown when the value is not positive.</exception>
    public static int AgainstNegativeOrZero(
        int value,
        [CallerArgumentExpression(nameof(value))] string? parameterName = null)
    {
        if (value <= 0)
        {
            throw new BadRequestException($"{parameterName} must be positive.");
        }
        
        return value;
    }

    /// <summary>
    /// Ensures the value is not negative.
    /// </summary>
    /// <exception cref="BadRequestException">Thrown when the value is negative.</exception>
    public static int AgainstNegative(
        int value,
        [CallerArgumentExpression(nameof(value))] string? parameterName = null)
    {
        if (value < 0)
        {
            throw new BadRequestException($"{parameterName} cannot be negative.");
        }
        
        return value;
    }

    /// <summary>
    /// Ensures the string matches the specified pattern.
    /// </summary>
    /// <exception cref="BadRequestException">Thrown when the string doesn't match the pattern.</exception>
    public static string AgainstInvalidFormat(
        string value,
        string pattern,
        string formatDescription,
        [CallerArgumentExpression(nameof(value))] string? parameterName = null)
    {
        if (!System.Text.RegularExpressions.Regex.IsMatch(value, pattern))
        {
            throw new BadRequestException($"{parameterName} has invalid format. Expected: {formatDescription}");
        }
        
        return value;
    }
}
