/// <summary>
/// 用户权限
/// </summary>
public enum UserPermissions
{
    SuperAdministrator,     // 超级管理员
    Administrator,          // 管理员
    OrdinaryUsers,          // 普通用户
}
/// <summary>
/// 数据库连接字符串
/// </summary>
public enum ConnectionMode
{
    Tcp,
    Udp,
    WebSocket,
    Kcp,
    Http,
    Https
}