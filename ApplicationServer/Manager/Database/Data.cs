/// <summary>
/// 用户
/// </summary>
public class User
{
    public ulong Id { get; set; }
    //用户名
    public string? Username { get; set; }
    //密码
    public string? Password { get; set; }
    //邮箱
    public string? Email { get; set; }
    //用户权限
    public UserPermissions Permissions { get; set; }
}

/// <summary>
/// 登录结果（发给客户端）
/// </summary>
public class LoginResult
{
    public string? Token { get; set; }
}

/// <summary>
/// 注册结果
/// </summary>
public class RegisterResult
{
    public string? Token { get; set; }
}

/// <summary>
/// 令牌
/// </summary>
public class TokenResult
{
    public int ResultCode { get; set; }
    public string? Message { get; set; }
    public string? Token { get; set; }
}