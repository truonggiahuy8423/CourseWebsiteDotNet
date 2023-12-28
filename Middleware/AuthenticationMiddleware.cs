


using Microsoft.AspNetCore.Mvc.Controllers;

namespace CourseWebsiteDotNet.Middleware
{

    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!IsRequestToUserController(context))
            {
                if (context.Session.GetString("user_id") == null)
                {
                    context.Response.Redirect("/login");
                    return;
                }
                else
                {
                    await _next(context);
                }
            }
            else
            {
                if (IsRequestToLogout(context) || IsRequestToActive(context))
                {
                    await _next(context);
                }
                else
                {
                    if (context.Session.GetString("user_id") == null)
                    {
                        await _next(context);
                    }
                    else
                    {
                        context.Response.Redirect("/courses");
                    }
                }
            }
        }
        private bool IsRequestToLogout(HttpContext context)
        {
            // Lấy thông tin về phương thức, controller và action của HttpRequest
            var actionDescriptor = context.GetEndpoint()?.Metadata.GetMetadata<ControllerActionDescriptor>();

            if (actionDescriptor != null)
            {
                var controllerName = actionDescriptor.ControllerName;
                var actionName = actionDescriptor.ActionName;

                // Kiểm tra xem controller có phải là "UserController" hay không
                return string.Equals(controllerName, "Authentication", StringComparison.OrdinalIgnoreCase) && string.Equals(actionName, "Logout", StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }
        private bool IsRequestToActive(HttpContext context)
        {
            // Lấy thông tin về phương thức, controller và action của HttpRequest
            var actionDescriptor = context.GetEndpoint()?.Metadata.GetMetadata<ControllerActionDescriptor>();

            if (actionDescriptor != null)
            {
                var controllerName = actionDescriptor.ControllerName;
                var actionName = actionDescriptor.ActionName;

                // Kiểm tra xem controller có phải là "UserController" hay không
                return string.Equals(controllerName, "Authentication", StringComparison.OrdinalIgnoreCase) && string.Equals(actionName, "Active", StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }

        private bool IsRequestToUserController(HttpContext context)
        {
            // Kiểm tra thông tin định tuyến để xem yêu cầu có được gửi đến UserController không
            var routeData = context.GetRouteData();
            var controller = routeData?.Values["controller"]?.ToString();

            // Kiểm tra xem controller có phải là "User" hay không
            return string.Equals(controller, "Authentication", StringComparison.OrdinalIgnoreCase);
        }
    }
    public static class SessionCheckMiddlewareExtensions
    {
        public static IApplicationBuilder UseSessionCheckMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthenticationMiddleware>();
        }
    }
}
