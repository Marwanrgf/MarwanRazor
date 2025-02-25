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


namespace GoodFriendsRazor.Pages.Shared.Uppgifter
{
    public class FriendsbyCountryModel : PageModel
    {
        //Just like for WebApi
        IFriendsService _service = null;
        ILogger<FriendsbyCountryModel> _logger = null;

        public GstUsrInfoAllDto infoAllDtos {get; set;}

        public List<string> country { set; get; } 


     
        public async Task<IActionResult> OnGet()
        {
            var info = await _service.InfoAsync;
            infoAllDtos = info; 


            var list = info.Friends.Select( s => s.Country).Where(s => s != null).Distinct().ToList();

           country = list;


            return Page();
        }

           public FriendsbyCountryModel(IFriendsService service, ILogger<FriendsbyCountryModel> logger)
        {
            _service = service;
            _logger = logger;
          
        }

       

    }

}