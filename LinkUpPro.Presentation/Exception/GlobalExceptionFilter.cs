using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class GlobalExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        // Aquí podrías loguear el error real (context.Exception) en un archivo o sistema de logs
        
        context.Result = new BadRequestObjectResult(new 
        { 
            message = "Ocurrió un error al procesar la solicitud. Inténtelo nuevamente." 
        });
        
        context.ExceptionHandled = true;
    }
}