using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models;
using Models.DTO;
using Services;
using Newtonsoft.Json;
using GoodFriendsRazor.SeidoHelpers;

namespace GoodFriendsRazor.Pages.Shared.Uppgifter
{
    public class FriendInputModel : PageModel
    {
        IFriendsService _service = null;
        ILogger<FriendInputModel> _logger = null;

        [BindProperty]
        public csFriendIM FriendIM { get; set; }

        public IFriend Friends { get; set; }

        public string ErrorMessage { get; set; } = null;
        //For Server Side Validation set by IsValid()
        public bool HasValidationErrors { get; set; }
        public IEnumerable<string> ValidationErrorMsgs { get; set; }
        public IEnumerable<KeyValuePair<string, ModelStateEntry>> InvalidKeys { get; set; }

        public ModelValidationResult ValidationResult { get; set; } = new ModelValidationResult(false, null, null);


        public async Task<IActionResult> OnGet()
        {

            if (Guid.TryParse(Request.Query["id"], out Guid _id))
            {

                FriendIM = new csFriendIM(await _service.ReadFriendAsync( _id, false));
            }
            else
            {

                FriendIM = new csFriendIM();
                FriendIM.StatusIM = enStatusIM.Inserted;

            }
            return Page();
        }

        public async Task<IActionResult> OnPostSave()
        {
            if (!IsValid())
            {
                //The page is not valid
                return Page();
            }
            if (FriendIM.StatusIM == enStatusIM.Inserted)
            {
                try
                {

                    var updatedModel = FriendIM.UpdateModel(new csFriend()
                    {
                        FriendId = Guid.Empty,
                        FirstName = FriendIM.FirstName,
                        LastName = FriendIM.LastName,
                        Email = FriendIM.Email,
                        Birthday = FriendIM.Birthday,
                        Address = new Address()
                        {
                            AddressId = Guid.Empty,
                            StreetAddress = FriendIM.StreetAddress,
                            ZipCode = FriendIM.ZipCode,
                            City = FriendIM.City,
                            Country = FriendIM.Country
                        },
                    });


                    var f =  await _service.ReadFriendAsync(FriendIM.FriendId, false);
                       

                       
                        var UpdateModel = FriendIM.UpdateModel(f);
                        var updateAdress = await _service.CreateAddressAsync(new AddressCUdto(UpdateModel.Address));
                       

               
                        var updatedFriend = await _service.UpdateFriendAsync(new FriendCUdto(UpdateModel));

                    return Redirect($"/Shared/Uppgifter/FriendView?id={updatedFriend.FriendId}");

                }
                catch (Exception)
                {
                    return StatusCode(500);
                }
            }
            else
            {
                try
                {


                        var f =  await _service.ReadFriendAsync(FriendIM.FriendId, false);

                       
                        var UpdateModel = FriendIM.UpdateModel(f);
                          

                        var updatedFriend = await _service.UpdateFriendAsync(new FriendCUdto(UpdateModel));

                        var updateAdress = await _service.UpdateAddressAsync(new AddressCUdto(UpdateModel.Address));

                    return Redirect($"/Shared/Uppgifter/FriendView?id={updatedFriend.FriendId}");
                }
                catch (Exception)
                {
                    return StatusCode(500);
                }
            }
        }

        public FriendInputModel(IFriendsService service, ILogger<FriendInputModel> logger)
        {
            _logger = logger;
            _service = service;
        }

         private bool IsValid(string[] validateOnlyKeys = null)
        {
            InvalidKeys = ModelState
               .Where(s => s.Value.ValidationState == ModelValidationState.Invalid);

            if (validateOnlyKeys != null)
            {
                InvalidKeys = InvalidKeys.Where(s => validateOnlyKeys.Any(vk => vk == s.Key));
            }

            ValidationErrorMsgs = InvalidKeys.SelectMany(e => e.Value.Errors).Select(e => e.ErrorMessage);
            HasValidationErrors = InvalidKeys.Any();

            return !HasValidationErrors;
        }

    }


        public enum enStatusIM { Unknown, Unchanged, Inserted, Modified, Deleted }
        public class csFriendIM
        {
            public enStatusIM StatusIM { get; set; }

            public Guid FriendId{ get; init; } = Guid.NewGuid(); 

            [Required(ErrorMessage ="First name is required")]
            public string FirstName { get; set; }

            [Required(ErrorMessage ="Last name is required")]
            public string LastName { get; set; }

            public string Email { get; set; }
            public DateTime? Birthday { get; set; }

            public Guid AddressId { get; set; }

            [Required(ErrorMessage ="Street address is required")]
            public string StreetAddress { get; set; }

            [Required(ErrorMessage ="ZipCode is required")]
            public int ZipCode { get; set; }

            [Required(ErrorMessage ="City is required")]
            public string City { get; set; }

            [Required(ErrorMessage ="Country is required")]
            public string Country { get; set; }

            public List<Guid> Pets { get; set; }
            public List<Guid> Quotes { get; set; }

            public csFriendIM() { StatusIM = enStatusIM.Unchanged; }

            public csFriendIM(csFriendIM original)
            {
                StatusIM = original.StatusIM;

                FriendId = original.FriendId;
                FirstName = original.FirstName;
                LastName = original.LastName;

                Email = original.Email;
                Birthday = original.Birthday;

                StreetAddress = original.StreetAddress;
                ZipCode = original.ZipCode;
                City = original.City;
                Country = original.Country;

                Pets = original.Pets;
                Quotes = original.Quotes;
            }

            public csFriendIM(IFriend model)
            {

                FriendId = model.FriendId;
                FirstName = model.FirstName;
                LastName = model.LastName;

                Email = model.Email;
                Birthday = model.Birthday;

                AddressId = model.Address.AddressId;
                StreetAddress = model.Address.StreetAddress;
                ZipCode = model.Address.ZipCode;
                City = model.Address.City;
                Country = model.Address.Country;

                Pets = model.Pets.Select(x => x.PetId).ToList();
                Quotes = model.Quotes.Select(x => x.QuoteId).ToList();
            }

            public IFriend UpdateModel(IFriend model)
            {
                model.FirstName = FirstName;
                model.LastName = LastName;

                model.Email = Email;
                model.Birthday = Birthday;

                model.Address.StreetAddress = StreetAddress;
                model.Address.ZipCode = ZipCode;
                model.Address.City = City;
                model.Address.Country = Country;

                return model;
            }
        }



    }

