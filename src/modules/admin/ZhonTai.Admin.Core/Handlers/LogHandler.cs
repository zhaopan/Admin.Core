﻿using System.Diagnostics;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using ZhonTai.Admin.Core.Exceptions;
using ZhonTai.Common.Helpers;
using ZhonTai.Admin.Core.Dto;
using ZhonTai.Admin.Core.Helpers;
using ZhonTai.Admin.Core.GrpcServices;
using ZhonTai.Admin.Core.GrpcServices.Dtos;

namespace ZhonTai.Admin.Core.Handlers;

/// <summary>
/// 操作日志处理
/// </summary>
public class LogHandler : ILogHandler
{
    private readonly ILogger _logger;
    private readonly ApiHelper _apiHelper;
    private readonly IOprationLogGrpcService _oprationLogGrpcService;

    public LogHandler(
        ILogger<LogHandler> logger,
        ApiHelper apiHelper,
        IOprationLogGrpcService oprationLogGrpcService
    )
    {
        _logger = logger;
        _apiHelper = apiHelper;
        _oprationLogGrpcService = oprationLogGrpcService;
    }

    public async Task LogAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var stopwatch = Stopwatch.StartNew();
        var actionExecutedContext = await next();
        stopwatch.Stop();

        try
        {
            var input = new OperationLogAddGrpcInput
            {
                Status = true,
                ApiMethod = context.HttpContext.Request.Method.ToLower(),
                ApiPath = context.ActionDescriptor.AttributeRouteInfo.Template.ToLower(),
                ElapsedMilliseconds = stopwatch.ElapsedMilliseconds,
                StatusCode = context.HttpContext.Response.StatusCode
            };

            var api = (await _apiHelper.GetApiListAsync()).FirstOrDefault(a => a.Path == input.ApiPath);
            //操作日志启用
            if ((api != null && api.EnabledLog))
            {
                var excepton = actionExecutedContext.Exception;

                //操作参数
                if ((api != null && api.EnabledParams && context.ActionArguments.Count > 0) || excepton != null)
                {
                    input.Params = JsonHelper.Serialize(context.ActionArguments);
                }

                //操作结果
                if (api != null && api.EnabledResult && actionExecutedContext.Result != null && actionExecutedContext.Result is JsonResult result)
                {
                    input.Result = JsonHelper.Serialize(result.Value);
                }

                if (excepton != null)
                {
                    input.Status = false;

                    var code = "";
                    if (excepton is AppException appException)
                    {
                        input.StatusCode = appException.StatusCode;
                        code = appException.AppCode;
                    }
                    else
                    {
                        input.StatusCode = (int)HttpStatusCode.InternalServerError;
                    }

                    input.Result = JsonHelper.Serialize(new ResultOutput<string>()
                    {
                        Code = code
                    }.NotOk(excepton.Message));
                }

                input.ApiLabel = api?.Label;

                await _oprationLogGrpcService.AddAsync(input);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("操作日志插入异常：{@ex}", ex);
        }
    }
}