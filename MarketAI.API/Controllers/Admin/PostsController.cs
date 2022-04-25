﻿using MarketAI.API.Core;
using MarketAI.API.Models;
using MarketAI.API.Models.Statuses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketAI.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PostsController : ControllerBase
    {
        private readonly ILogger<PostsController> _logger;
        private readonly APIDBContext db;
        public PostsController(ILogger<PostsController> logger, APIDBContext _db)
        {
            _logger = logger;
            db = _db;
        }
        [HttpGet]
        public PostModel GetPost(int id)
        {
            return db.Posts.FirstOrDefault(o => o.Id == id);
        }
        [HttpGet]
        public IEnumerable<PostModel> GetPosts()
        {
            return db.Posts.ToList();
        }
        [HttpDelete]
        public async Task<RequestStatus> RemovePost(int id)
        {
            var post = db.Posts.First(o => o.Id == id);
            if (post == null)
                return new RequestStatus("Поста с таким id не существует", 404);
            db.Posts.Remove(post);
            await db.SaveChangesAsync();
            return new RequestStatus("Пост успешно удален");
        }
        [HttpPost]
        public async Task<RequestStatus> CreatePost(PostModel post)
        {
            try
            {
                db.Posts.Add(post);
                await db.SaveChangesAsync();
                return new RequestStatus("Пост успешно добавлен");
            }
            catch (Exception ex)
            {
                return new RequestStatus(ex.Message + ex.StackTrace,500);
            }
        }
        [HttpPut]
        public async Task<RequestStatus> UpdatePost(PostModel updatedPost)
        {
            try
            {
                db.Posts.Update(updatedPost);
                await db.SaveChangesAsync();
                return new RequestStatus("Пост успешно обновлен");
            }
            catch (Exception ex)
            {
                return new RequestStatus(ex.Message + ex.StackTrace, 500);
            }
        }

        [HttpGet]
        public IEnumerable<string> GetAllTags()
        {
            var tags = new SortedSet<string>();
            using (APIDBContext db = new APIDBContext())
            {
                foreach (var post in db.Posts)
                {
                    var postTags = post.Tags.Split(',');
                    foreach (var tag in postTags)
                    {
                        tags.Add(tag);
                    }
                }
            }
            return tags;
        }
    }
}
