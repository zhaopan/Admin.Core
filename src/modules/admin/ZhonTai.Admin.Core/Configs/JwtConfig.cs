﻿namespace ZhonTai.Admin.Core.Configs;

/// <summary>
/// Jwt配置
/// </summary>
public class JwtConfig
{
    /// <summary>
    /// 发行者
    /// </summary>
    public string Issuer { get; set; } = "admin.core";

    /// <summary>
    /// 订阅者
    /// </summary>
    public string Audience { get; set; } = "admin.core";

    /// <summary>
    /// 密钥
    /// </summary>
    public string SecurityKey { get; set; }

    /// <summary>
    /// 有效期(分钟)
    /// </summary>
    public int Expires { get; set; } = 120;

    /// <summary>
    /// 刷新有效期(分钟)
    /// </summary>
    public int RefreshExpires { get; set; } = 480;
}