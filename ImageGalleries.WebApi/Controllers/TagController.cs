using AutoMapper;
using ImageGalleries.WebApi.Repositories.Tags;
using ImageGalleries.WebApi.Repositories.Users;
using Microsoft.AspNetCore.Mvc;

namespace ImageGalleries.WebApi.Controllers
{
    [ApiController]
    [Route("api/tag")]
    public class TagController : Controller
    {
        private readonly ITagRepository _tagRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public TagController(ITagRepository tagRepository,
            IUserRepository userRepository,
            IMapper mapper)
        {
            _tagRepository = tagRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }
    }
}
