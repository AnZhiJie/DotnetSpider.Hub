﻿using DotnetSpider.Enterprise.Core;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;

namespace DotnetSpider.Enterprise
{
	public class HttpGlobalExceptionFilter : IExceptionFilter
	{
		private readonly ILogger<HttpGlobalExceptionFilter> _logger;

		public HttpGlobalExceptionFilter(ILogger<HttpGlobalExceptionFilter> logger)
		{
			_logger = logger;
		}

		public void OnException(ExceptionContext context)
		{
			_logger.LogError(context.Exception.ToString());
			context.HttpContext.Response.StatusCode = 206;
			string info;

			if (context.Exception is DotnetSpiderException)
			{
				info = JsonConvert.SerializeObject(new StandardResult { Code = 101, Message = context.Exception.Message, Status = Status.Error });

				_logger.LogError(context.Exception.ToString());
			}
			else
			{
				_logger.LogError(context.Exception.ToString());
				info = JsonConvert.SerializeObject(new StandardResult { Code = 102, Message = "Internal error.", Status = Status.Error });
			}

			var bytes = Encoding.UTF8.GetBytes(info);
			context.ExceptionHandled = true;
			context.HttpContext.Response.ContentType = "application/json; charset=utf-8";
			context.HttpContext.Response.Body.Write(bytes, 0, bytes.Length);
		}
	}
}

