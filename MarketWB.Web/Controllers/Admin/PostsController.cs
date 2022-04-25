using MarketAI.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Posts = MarketAI.API.Controllers.PostsModule;

namespace MarketWB.Web.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class PostsController : Controller
    {
        private readonly ILogger<PostsController> _logger;
        private readonly IWebHostEnvironment _appEnvironment;
        private readonly Posts _api;
        public PostsController(ILogger<PostsController> logger, Posts api, IWebHostEnvironment appEnvironment)
        {
            _logger = logger;
            _api = api;
            _appEnvironment = appEnvironment;
        }

        [Route("Admin/Posts")]
        public IActionResult Posts()
        {
            var posts = _api.GetPosts();
            return View("Views/Admin/Posts/Posts.cshtml", posts);
        }
        [Route("Admin/CreatePost")]
        public IActionResult CreatePost()
        {
            return View("Views/Admin/Posts/CreatePost.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> CreatePostPOST(PostModel post)
        {
            var img = Request.Form.Files.FirstOrDefault();
            var filePath = "/uploads/" + Guid.NewGuid().ToString();

            if (img != null)
            {
                post.ImgURL = filePath + img.FileName;
                using (var fileStream = new FileStream(_appEnvironment.WebRootPath + post.ImgURL, FileMode.Create))
                {
                    await img.CopyToAsync(fileStream);
                }
            }

            await _api.CreatePost(post);

            return Posts();
        }


        [Route("Admin/UpdatePost")]
        public async Task<IActionResult> UpdatePost(int id)
        {
            var model = _api.GetPost(id);
            return View("Views/Admin/Posts/UpdatePost.cshtml", model);
        }
        [Route("Admin/DeletePost")]
        public async Task<IActionResult> DeletePost(int id)
        {
            await _api.RemovePost(id);
            return Posts();
        }

        [HttpPost]
        public async Task<IActionResult> UpdatePostPost(int id,PostModel post)
        {
            var img = Request.Form.Files.FirstOrDefault();
            var filePath = "/uploads/" + Guid.NewGuid().ToString();

            if (img != null)
            {
                post.ImgURL = filePath + img.FileName;
                using (var fileStream = new FileStream(_appEnvironment.WebRootPath + post.ImgURL, FileMode.Create))
                {
                    await img.CopyToAsync(fileStream);
                }
            }
            await _api.UpdatePost(post);

            return Posts();
        }

    }
}
