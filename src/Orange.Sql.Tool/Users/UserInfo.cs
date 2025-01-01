using System.ComponentModel.DataAnnotations;

namespace Orange.Sql.Tool.Users;

public record UserInfo
{
    public Guid UserId { get; init; }
    
    [Required]
    public required string EmployeeNum { get; init; } = default!;
    public string FirstName { get; init; } = default!;
    public string LastName { get; init; } = default!;
    public string Email { get; init; } = default!;
    public DateTime UtcCreatedAt { get; init; }
    public string? CreatedBy { get; init; }
    public DateTime UtcUpdatedAt { get; init; }
    public string? UpdatedBy { get; init; }
    public bool IsEnabled { get; init; }
}

