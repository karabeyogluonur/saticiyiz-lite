using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace SL.Web.Controllers;

public class ErrorController : Controller
{
    private readonly ILogger<ErrorController> _logger;

    public ErrorController(ILogger<ErrorController> logger)
    {
        _logger = logger;
    }

    [Route("/Error/{statusCode}")]
    public IActionResult HttpStatusCodeHandler(int statusCode)
    {
        string viewName = statusCode switch
        {
            404 => "NotFound",
            500 => "InternalServerError",
            403 => "Forbidden",
            401 => "Unauthorized",
            _ => "InternalServerError" // Diğer tüm hatalar için 500 sayfası
        };

        return View(viewName, new ErrorViewModel
        {
            StatusCode = statusCode,
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
            Message = GetErrorMessage(statusCode)
        });
    }

    [Route("/Error")]
    public IActionResult Error()
    {
        return View("InternalServerError", new ErrorViewModel
        {
            StatusCode = 500,
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
            Message = "Beklenmeyen bir hata oluştu"
        });
    }

    private static string GetErrorMessage(int statusCode)
    {
        return statusCode switch
        {
            404 => "Aradığınız sayfa bulunamadı",
            500 => "Sunucu hatası oluştu",
            403 => "Bu sayfaya erişim yetkiniz bulunmamaktadır",
            401 => "Bu sayfaya erişmek için giriş yapmanız gerekmektedir",
            _ => "Bir hata oluştu"
        };
    }
}

public class ErrorViewModel
{
    public int StatusCode { get; set; }
    public string RequestId { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Details { get; set; }
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
