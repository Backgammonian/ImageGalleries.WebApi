using ImageGalleries.WebApi.Models;

namespace ImageGalleries.WebApi.Repositories.Tags
{
    public interface ITagRepository
    {
        Task<ICollection<Tag>> GetTags();
        Task<bool> DoesTagExist(string tagName);
        Task<Tag?> GetTag(string tagName);
        Task<ICollection<Picture>> GetPicturesByTag(string tagName);
        Task<bool> CreateTag(string name, string description = "");
        Task<bool> AddTagToPicture(Tag tag, Picture picture);
        Task<bool> RemoveTagFromPicture(Tag tag, Picture picture);
        Task<bool> UpdateTag(Tag tag, string newName, string newDescription);
        Task<bool> RemoveTag(Tag tag);
        Task<bool> Save();
    }
}
