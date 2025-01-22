
using Mango.Services.ShoppingCartAPI.Exceptions;

namespace Mango.Services.ShoppingCartAPI.Middlewares;

public class ErrorHandlingMiddleware: IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (NotFoundException notFound)
        {
            context.Response.StatusCode = 404;
            await context.Response.WriteAsync(notFound.Message);
        }
        catch (Exception exception)
        {
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync("Oh no, something went wrong. Please try again later!" + exception.Message);
            // await context.Response.WriteAsync("Oh no, something went wrong. Please try again later!");

        }
    }
}

