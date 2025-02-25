using Seido.Utilities.SeedGenerator;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models;
using Models.DTO;
using Services;
using Newtonsoft.Json;


namespace GoodFriendsRazor.Pages.Shared.Uppgifter
{
    public class QuoteInputModel : PageModel
    {
        IFriendsService _service = null;
        ILogger<QuoteInputModel> _logger = null;
        public List<IQuote> Quotes { get; set; }
        public int NrOfGroups { get; set; }

        [BindProperty]
        public csQuoteIM QuoteIM { get; set; }
        public string Author { get; set; }

        [BindProperty]
        public bool UseSeeds { get; set; } = true;
        [BindProperty]
        public string SearchFilter { get; set; } = null;
        public int ThisPageNr { get; set; } = 0;
        public int PageSize { get; } = 10;

        public string ErrorMessage { get; set; } = null;
     
        public bool HasValidationErrors { get; set; }
        public IEnumerable<string> ValidationErrorMsgs { get; set; }
        public IEnumerable<KeyValuePair<string, ModelStateEntry>> InvalidKeys { get; set; }



        public async Task<IActionResult> OnGet()
        {
            if (Guid.TryParse(Request.Query["id"], out Guid id))
            {
                QuoteIM = new csQuoteIM(await _service.ReadQuoteAsync( id, false));
 
            }
            else
            {
          
                 QuoteIM = new csQuoteIM();

                 QuoteIM.StatusIM = enStatusIM.Inserted;
            }

            return Page();
        }






        public async Task<IActionResult> OnPostDeleteGroup(Guid id)
        {
         await _service.DeleteQuoteAsync(id);

            var resp = await _service.ReadQuotesAsync(UseSeeds, false, SearchFilter, ThisPageNr, PageSize);
            Quotes = resp.PageItems;
            NrOfGroups = resp.DbItemsCount;



            return Page();
        }


        public async Task<IActionResult> OnPostSubmit()
        {
            if (QuoteIM.StatusIM == enStatusIM.Inserted)
            {
                try
                {
                    
                }
                catch (Exception)
                {
                    return StatusCode(500);
                }


                var updateModel = QuoteIM.UpdateModel(new Quote()
                {
                    Author = QuoteIM.Author,
                    QuoteText = QuoteIM.Quote,

                });
                var updatedDTO = await _service.CreateQuoteAsync( new Models.DTO.QuoteCUdto()
                {
                    QuoteId = Guid.Empty,
                    Quote = updateModel.QuoteText,
                    Author = updateModel.Author,


                });
                var friend = await _service.ReadFriendAsync( QuoteIM.FriendId, false);


                var updateFriend = await _service.UpdateFriendAsync( new Models.DTO.FriendCUdto()
                {

                    PetsId = friend.Pets.Select(x => x.PetId).ToList(),

                });

                return Redirect($"/Shared/Uppgifter/FriendView?id={QuoteIM.FriendId}");
            }
            else
            {
                var updateModel = QuoteIM.UpdateModel(new Quote()
                {
                    Author = QuoteIM.Author,
                    QuoteText = QuoteIM.Quote,
                });

                var updatedDTO = _service.UpdateQuoteAsync( new Models.DTO.QuoteCUdto()
                {
                    QuoteId = QuoteIM.QuoteId,
                    Author = updateModel.Author,
                    Quote = updateModel.QuoteText

                });

                return Redirect($"/Shared/Uppgifter/FriendView?id={QuoteIM.FriendId}");
            }
        }


        public QuoteInputModel(IFriendsService service, ILogger<QuoteInputModel> logger)
        {
            _logger = logger;
            _service = service;
        }
        public enum enStatusIM { Unknown, Unchanged, Inserted, Modified, Deleted }

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

            public Quote UpdateModel(Quote model)
            {
                Quote = model.QuoteText;
                Author = model.Author;

                return model;
            }
        }
    }
}
