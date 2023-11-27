using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Net;

namespace ToDoMVC.Middlewares
{
    public class ErrorHandlingMiddleware(RequestDelegate next, ICompositeViewEngine viewEngine, ITempDataProvider tempDataProvider)
    {
        private readonly RequestDelegate _next = next;
        private readonly ICompositeViewEngine _viewEngine = viewEngine;
        private readonly ITempDataProvider _tempDataProvider = tempDataProvider;

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "text/html";

            var viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
            {
                { "Message", ex.Message },
                { "Exception", ex.GetType().Name },
                { "StackTrace", ex.StackTrace }
            };

            var tempData = new TempDataDictionary(context, _tempDataProvider);

            using var writer = new StringWriter();
            var actionContext = new ActionContext(context, new RouteData(), new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor());
            var viewResult = _viewEngine.FindView(actionContext, "Error", false);
            var viewContext = new ViewContext(actionContext, viewResult.View, viewData, tempData, writer, new HtmlHelperOptions());
            await viewResult.View.RenderAsync(viewContext);
            var html = writer.ToString();
            await context.Response.WriteAsync(html);
        }
    }
}
