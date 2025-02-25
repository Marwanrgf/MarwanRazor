using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models;
using Models.DTO;
using Services;

namespace GoodFriendsRazor.Pages.Shared.Uppgifter
{
    public class FriendViewModel : PageModel
    {
        readonly IFriendsService _service = null;
        ILogger<FriendViewModel> _logger = null;
        public Guid CurrentFriendId {  get; set; }
        public csFriend CurrentFriend {  get; set; }

        public List<IFriend> Friend { get; set; }

       [BindProperty]
        public csFriendIM FriendIM { get; set; }


        [BindProperty]
        public csQuoteIM QuoteIM { get; set; }        

        public List<IQuote> Quotes { get; set; }
        public int NrOfGroups { get; set; }
        [BindProperty]
        public bool UseSeeds { get; set; } = true;
        [BindProperty]
        public string SearchFilter { get; set; } = null;
        public int ThisPageNr { get; set; } = 0;
        public int PageSize { get; } = 10;

        

        public async Task<IActionResult> OnGet()
        {
            if (Guid.TryParse(Request.Query["id"], out Guid _id)) 
            {  
                CurrentFriendId = _id;
                var friend = await _service.ReadFriendAsync( CurrentFriendId, false);

                csFriend _friend = new csFriend()
                {
                    FriendId = friend.FriendId,
                    FirstName = friend.FirstName,
                    LastName = friend.LastName,
                    Email = friend.Email,
                    Birthday = friend.Birthday,
                    Address = friend.Address,
                    Quotes = friend.Quotes,
                    Pets = friend.Pets,
                };
                CurrentFriend = _friend;

 

                
            }



            return Page();
        }

        public async Task<IActionResult> OnPostDeletePet(Guid petId, Guid friendId)
        {
         await _service.DeletePetAsync(petId);
       

            var f =  await _service.ReadFriendAsync(FriendIM.FriendId, false);

            return Redirect($"/Shared/Uppgifter/FriendView?id={friendId}");
        }


        //Qoutes
        public async Task<IActionResult> OnPostDeleteQuote(Guid quoteId, Guid friendId)
        {
         await _service.DeleteQuoteAsync(quoteId);

        var q = await _service.ReadQuoteAsync(quoteId, false);
            

                          


            var f =  await _service.ReadFriendAsync(FriendIM.FriendId, false);

                       


            return Redirect($"/Shared/Uppgifter/FriendView?id={friendId}");
        }


        public FriendViewModel(IFriendsService service, ILogger<FriendViewModel> logger)
        {
            _logger = logger;
            _service = service;
        }
    }


       

           public class csQuoteIM
        {
            public enStatusIM StatusIM { get; set; }

            public Guid QuoteId { get; set; }
            public string Quote { get; set; }
            public string Author { get; set; }
            public Guid FriendId { get; set; }

            public csQuoteIM() { StatusIM = enStatusIM.Unchanged; }

            public csQuoteIM(csQuoteIM original)
            {
                QuoteId = original.QuoteId;
                Quote = original.Quote;
                Author = original.Author;
                FriendId = original.FriendId;
            }

            public csQuoteIM(IQuote model)
            {
                QuoteId = model.QuoteId;
                Quote = model.QuoteText;
                Author = model.Author;
                FriendId = model.Friends.Select(f => f.FriendId).FirstOrDefault();
            }

            public IQuote UpdateModel(IQuote model)
            {
                Quote = model.QuoteText;
                Author = model.Author;

                return model;
            }
        }

}
