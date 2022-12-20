using Microsoft.AspNetCore.Components;
using Radzen;
using System.Net;
using Toolbelt.Blazor;

namespace FamilyBudget.Client.Services
{
    public class HttpInterceptorService
    {
        private readonly HttpClientInterceptor _interceptor;
        private readonly NavigationManager _navManager;
        private readonly NotificationService _notificationService;

        public HttpInterceptorService(HttpClientInterceptor interceptor, NavigationManager navManager,
            NotificationService notificationService)
        {
            _interceptor = interceptor;
            _navManager = navManager;
            _notificationService = notificationService;
        }

        public void MonitorEvent() => _interceptor.AfterSend += InterceptResponse;

        private void InterceptResponse(object sender, HttpClientInterceptorEventArgs e)
        {
            string message = string.Empty;

            if (!e.Response.IsSuccessStatusCode)
            {
                var responseCode = e.Response.StatusCode;

                switch (responseCode)
                {
                    case HttpStatusCode.NotFound:
                        _navManager.NavigateTo("/404");
                        message = "The requested resorce was not found.";
                        break;
                    case HttpStatusCode.BadRequest:
                        var x = _notificationService.Messages;
                        _notificationService.Messages.Clear();
                        _notificationService.Notify(NotificationSeverity.Error, "Bad request occured", closeOnClick: true);
                        return;
                    case HttpStatusCode.Unauthorized:
                    case HttpStatusCode.Forbidden:
                        _navManager.NavigateTo("/unauthorized");
                        message = "You are not authorized to access this resource. ";
                        break;
                    default:
                        _navManager.NavigateTo("/500");
                        message = "Something went wrong, please contact Administrator";
                        break;
                }
                throw new HttpRequestException(message);
            }
        }
        public void DisposeEvent() => _interceptor.AfterSend -= InterceptResponse;
    }
}
