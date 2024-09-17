using System.Diagnostics;
using GymWebApp.Models;
using GymWebApp.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;


namespace GymWebApp.Controllers
{

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private UnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _unitOfWork = new UnitOfWork(configuration.GetConnectionString("DefaultConnection"));
        }

        [Route("Index")]
        public IActionResult Index()
        {
            return Redirect("/registration/index.html");
        }
    }
}
