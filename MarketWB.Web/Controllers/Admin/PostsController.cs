using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketWB.Web.Controllers.Admin
{
    public class PostsController : Controller
    {
        private readonly ILogger<PostsController> _logger;
        public PostsController(ILogger<PostsController> logger)
        {
            _logger = logger;
        }

        [Route("Admin/Posts")]
        public IActionResult Posts()
        {
            return View("Views/Admin/Posts/Posts.cshtml");
        }
        [Route("Admin/CreatePost")]
        public IActionResult CreatePost()
        {
            return View("Views/Admin/Posts/CreatePost.cshtml");
        }
        [Route("Admin/UpdatePost")]
        public IActionResult UpdatePost()
        {
            return View("Views/Admin/Posts/UpdatePost.cshtml");
        }
    }
}
