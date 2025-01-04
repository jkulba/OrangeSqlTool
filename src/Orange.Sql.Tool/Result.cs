namespace Orange.Sql.Tool;

/// <summary>
/// Represents the result of an operation, which can be either a success or an error.
/// </summary>
/// <typeparam name="TValue">The type of the value in case of success.</typeparam>
/// <typeparam name="TError">The type of the error in case of failure.</typeparam>
public readonly struct Result<TValue, TError> : IEquatable<Result<TValue, TError>>
{
    /// <summary>
    /// Gets a value indicating whether the result is an error.
    /// </summary>
    public bool IsError { get; }

    /// <summary>
    /// Gets a value indicating whether the result is a success.
    /// </summary>
    public bool IsSuccess => !IsError;

    private readonly TValue? _value;
    private readonly TError? _error;

    /// <summary>
    /// Initializes a new instance of the <see cref="Result{TValue, TError}"/> struct with a success value.
    /// </summary>
    /// <param name="value">The success value.</param>
    public Result(TValue value)
    {
        IsError = false;
        _value = value;
        _error = default;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Result{TValue, TError}"/> struct with an error value.
    /// </summary>
    /// <param name="error">The error value.</param>
    public Result(TError error)
    {
        IsError = true;
        _error = error;
        _value = default;
    }

    public static implicit operator Result<TValue, TError>(TValue value) => new(value);

    public static implicit operator Result<TValue, TError>(TError error) => new(error);

    /// <summary>
    /// Matches the result and executes the appropriate function based on whether it is a success or an error.
    /// </summary>
    /// <typeparam name="TResult">The type of the result of the match function.</typeparam>
    /// <param name="success">The function to execute if the result is a success.</param>
    /// <param name="failure">The function to execute if the result is an error.</param>
    /// <returns>The result of the executed function.</returns>
    public TResult Match<TResult>(Func<TValue, TResult> success, Func<TError, TResult> failure) =>
        !IsError ? success(_value!) : failure(_error!);

    /// <summary>
    /// Gets the value if the result is a success, or the specified default value if it is an error.
    /// </summary>
    /// <param name="defaultValue">The default value to return if the result is an error.</param>
    /// <returns>The value if the result is a success, or the default value if it is an error.</returns>
    public TValue GetValueOrDefault(TValue defaultValue = default!) => IsError ? defaultValue : _value!;

    /// <summary>
    /// Deconstructs the result into its components.
    /// </summary>
    /// <param name="isError">Indicates whether the result is an error.</param>
    /// <param name="value">The success value.</param>
    /// <param name="error">The error value.</param>
    public void Deconstruct(out bool isError, out TValue? value, out TError? error)
    {
        isError = IsError;
        value = _value;
        error = _error;
    }

    public override string ToString() => IsError ? $"Error: {_error}" : $"Success: {_value}";

    public override bool Equals(object? obj) => obj is Result<TValue, TError> other && Equals(other);

    public bool Equals(Result<TValue, TError> other) =>
        IsError == other.IsError &&
        EqualityComparer<TValue?>.Default.Equals(_value, other._value) &&
        EqualityComparer<TError?>.Default.Equals(_error, other._error);

    public override int GetHashCode() =>
        HashCode.Combine(IsError, _value, _error);
}
