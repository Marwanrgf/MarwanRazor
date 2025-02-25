using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Models;
using Newtonsoft.Json;
using Services;
using Models.DTO;

namespace Razor.Pages.Shared.Uppgifter
{
    public class ListOfFriendsModel : PageModel
    {

        IFriendsService _service = null;

        ILogger<ListOfFriendsModel> _logger = null;

        public List<IFriend> Friends { get; set; }

        public string currentCity { get; set; }



        public async Task<IActionResult> OnGet()
        { 

            string id = Request.Query["id"];
            currentCity = id;


            var friends = await _service.ReadFriendsAsync(true, false, "", 0, 1000);
            var createdFriends = await _service.ReadFriendsAsync(false, false, "", 0, 1000);
        
            var allFriends = friends.PageItems.Concat(createdFriends.PageItems).ToList();

            var allFriendsWithAddress = allFriends.Where(x => x.Address != null && x.Address.City == currentCity).ToList();

             Friends = allFriendsWithAddress;


            return Page();


        }
           public ListOfFriendsModel(IFriendsService service, ILogger<ListOfFriendsModel> logger)
        {
            _service = service;
            _logger = logger;
          
        }
    }
}
