using ImageGalleries.WebApi.Models;

namespace ImageGalleries.WebApi.Repositories.Tags
{
    public interface ITagRepository
    {
        Task<ICollection<Tag>> GetTags();
        Task<bool> DoesTagExist(string tagId);
        Task<Tag?> GetTag(string tagId);
        Task<ICollection<Picture>> GetPicturesByTag(string tagId);
        Task<bool> CreateTag(string name, string description = "");
        Task<bool> AddTagToPicture(string tagId, string pictureId);
        Task<bool> RemoveTagFromPicture(string tagId, string pictureId);
        Task<bool> UpdateTagNameAndDescription(string tagId, string newName, string newDescription);
        Task<bool> RemoveTag(string tagId);
        Task<bool> Save();
    }
}
