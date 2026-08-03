using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;
using ZhonTai.Admin.Core.Resources;
using ZhonTai.Common.Extensions;
using ZhonTai.Common.Helpers;
using StatusCodes = ZhonTai.Admin.Core.Enums.StatusCodes;

namespace ZhonTai.Admin.Core.Handlers;

/// <summary>
/// 响应认证处理器
/// </summary>
public class ResponseAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
#if NET8_0_OR_GREATER
    public ResponseAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder
    ) : base(options, logger, encoder) 
#else
    public ResponseAuthenticationHandler(
       IOptionsMonitor<AuthenticationSchemeOptions> options,
       ILoggerFactory logger,
       UrlEncoder encoder,
       ISystemClock systemClock
   ) : base(options, logger, encoder, systemClock)
#endif
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        throw new NotImplementedException();
    }

    protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        Response.ContentType = "application/json";
        Response.StatusCode = Microsoft.AspNetCore.Http.StatusCodes.Status401Unauthorized;
        var adminCoreLocalizer = AppInfo.GetRequiredService<AdminCoreLocalizer>();
        var msg = adminCoreLocalizer?[StatusCodes.Status401Unauthorized.ToDescription()]?.Value;

        await Response.WriteAsync(JsonHelper.Serialize(
            new ResponseStatusData
            {
                Code = StatusCodes.Status401Unauthorized,
                Msg = msg,
                Success = false
            }
        ));
    }

    protected override async Task HandleForbiddenAsync(AuthenticationProperties properties)
    {
        Response.ContentType = "application/json";
        Response.StatusCode = Microsoft.AspNetCore.Http.StatusCodes.Status403Forbidden;
        var adminCoreLocalizer = AppInfo.GetRequiredService<AdminCoreLocalizer>();
        var msg = adminCoreLocalizer?[StatusCodes.Status403Forbidden.ToDescription()]?.Value;

        await Response.WriteAsync(JsonHelper.Serialize(
            new ResponseStatusData
            {
                Code = StatusCodes.Status403Forbidden,
                Msg = msg,
                Success = false
            }
        ));
    }
}

public class ResponseStatusData
{
    public StatusCodes Code { get; set; } = StatusCodes.Status1Ok;
    public string Msg { get; set; }
    public bool Success { get; set; }

}