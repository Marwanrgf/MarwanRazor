using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models.DTO;
using Services;

namespace GoodFriendsRazor.Pages.Shared.Uppgifter
{
    public class FriendsByCityModel : PageModel
    {

        IFriendsService _service = null;
        ILogger<FriendsByCityModel> _logger = null;

        public GstUsrInfoAllDto infoAllDtos { get; set; }
        public List<GstUsrInfoFriendsDto> distinctCities { get; set; }
        public string currentCountry { get; set; }

        public async Task<IActionResult> OnGet()
        {
            string id = Request.Query["id"];

            infoAllDtos = await _service.InfoAsync;
            var cntrylist = infoAllDtos.Friends.Where(x => x.Country == id).ToList();

            distinctCities = cntrylist.Where(x => x.City != null).Distinct().ToList();
            currentCountry = id;

            return Page();
        }
        public FriendsByCityModel(IFriendsService service, ILogger<FriendsByCityModel> logger)
        {
            _service = service;
            _logger = logger;
        }
    }
}
