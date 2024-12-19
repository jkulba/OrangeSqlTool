namespace Orange.Sql.Tool.Users;

public class UserInfo
{
    public Guid UserId { get; set; }
    public required string EmployeeNum { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? Email { get; set; }
    public DateTime UtcCreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime UtcUpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsEnabled { get; set; }
}

