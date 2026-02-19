using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace Authi.Server.Extensions
{
    internal static class ErrorHandlingExtensions
    {
        public static T OnException<T>(this T builder, Action<Exception> handler) where T : IApplicationBuilder
        {
            builder.UseMiddleware<ExceptionMiddleware>(handler);
            return builder;
        }

        public static DbContextOptionsBuilder OnError(this DbContextOptionsBuilder optionsBuilder, Action<Exception> handler)
        {
            optionsBuilder.AddInterceptors(new DbErrorInterceptor(handler));
            return optionsBuilder;
        }

        private class ExceptionMiddleware(RequestDelegate next, Action<Exception> handler)
        {
            public async Task InvokeAsync(HttpContext context)
            {
                try
                {
                    await next(context);
                }
                catch (Exception exception)
                {
                    handler.Invoke(exception);
                    throw;
                }
            }
        }

        private class DbErrorInterceptor(Action<Exception> handler) : DbCommandInterceptor
        {
            public override void CommandFailed(DbCommand command, CommandErrorEventData eventData)
            {
                handler(new InterceptedDbError(command.CommandText, eventData.Exception));
            }
        }
    }

    internal class InterceptedDbError(string commandText, Exception innerException) 
        : Exception($"An error occurred while executing the database command: {commandText}", innerException)
    {
        public string CommandText { get; } = commandText;
    }
}
