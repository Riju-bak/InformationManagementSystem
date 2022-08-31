using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using InformationManagementSystem.Data;
using InformationManagementSystem.Models;
using InformationManagementSystem.Models.SchoolViewModels;
using Microsoft.EntityFrameworkCore;

namespace InformationManagementSystem.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly SchoolContext _context;

    // ASP .Net Core dependency injection takes care of passing an instance of SchoolContext into controller.
    // We configured this by registering SchoolContext in Program.cs
    public HomeController(ILogger<HomeController> logger, SchoolContext context)
    {
        _logger = logger;
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public async Task<IActionResult> About()
    {
        var data = from s in _context.Students
            group s by s.EnrollmentDate
            into dataGroup
            select new EnrollmentDateGroup()
            {
                EnrollmentDate = dataGroup.Key,
                StudentCount = dataGroup.Count()
            };

        return View(await data.AsNoTracking().ToListAsync());
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}