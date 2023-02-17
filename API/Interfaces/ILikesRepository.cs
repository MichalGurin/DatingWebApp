using API.DTOs;
using API.Helpers;
using API.Models;

namespace API.Interfaces
{
    public interface ILikesRepository
    {
        Task<UserLike> GetUserLike(int sourceUserId, int targetUserId);
        Task<AppUser> GetUserWithLikes(int sourceUserId);
        Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams);
    }
}