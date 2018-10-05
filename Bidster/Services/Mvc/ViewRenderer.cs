using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Bidster.Services.Mvc
{
    public interface IViewRenderer
    {
        Task<string> RenderViewToStringAsync(string viewName, object model);
    }
    
    public class ViewRenderer : IViewRenderer
    {
        private readonly IRazorViewEngine _razorViewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly HttpContext _httpContext;
        private readonly ActionContext _actionContext;

        public ViewRenderer(IRazorViewEngine razorViewEngine,
            ITempDataProvider tempDataProvider,
            IHttpContextAccessor httpContextAccessor,
            IActionContextAccessor actionContextAccessor)
        {
            _razorViewEngine = razorViewEngine;
            _tempDataProvider = tempDataProvider;
            _httpContext = httpContextAccessor.HttpContext;
            _actionContext = actionContextAccessor.ActionContext;
        }

        public async Task<string> RenderViewToStringAsync(string viewName, object model)
        {
            using (var sw = new StringWriter())
            {
                var viewResult = _razorViewEngine.FindView(_actionContext, viewName, false);

                if (viewResult.View == null)
                    throw new ArgumentNullException($"{viewName} does not match any available view");

                var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                {
                    Model = model
                };

                var viewContext = new ViewContext(_actionContext, viewResult.View, viewDictionary,
                    new TempDataDictionary(_actionContext.HttpContext, _tempDataProvider),
                    sw, new HtmlHelperOptions());

                await viewResult.View.RenderAsync(viewContext);
                return sw.ToString(); ;
            }
        }
    }
}
