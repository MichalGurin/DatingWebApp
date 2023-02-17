using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Helpers;
using API.Interfaces;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
      public class LikesRepository : ILikesRepository
      {
            private readonly DataContext _context;

            public LikesRepository(DataContext context)
            {
                _context = context;
            }

            public async Task<UserLike> GetUserLike(int sourceUserId, int targetUserId)
            {
                return await _context.Like.FindAsync(sourceUserId, targetUserId);
            }

            public async Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams)
            {
                var users = _context.Users.OrderBy(u => u.UserName).AsQueryable();
                var likes = _context.Like.AsQueryable();

                if(likesParams.Predicate == "liked")
                {
                    likes = likes.Where(like => like.SourceUserId == likesParams.UserId);
                    users = likes.Select(like => like.TargetUser);
                }
                else if(likesParams.Predicate == "likedBy")
                {
                    likes = likes.Where(like => like.TargetUserId == likesParams.UserId);
                    users = likes.Select(like => like.SourceUser);
                }

                var likedUsers = users.Select(user => new LikeDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Age = user.GetAge(),
                    KnownAs = user.KnownAs,
                    PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain).Url,
                    City = user.City
                });

                return await PagedList<LikeDto>.CreateAsync(likedUsers, likesParams.PageNumber, likesParams.PageSize);
            }

            public async Task<AppUser> GetUserWithLikes(int sourceUserId)
            {
                return await _context.Users.Include(u => u.LikedUsers).FirstOrDefaultAsync(u => u.Id == sourceUserId);
            }
      }
}