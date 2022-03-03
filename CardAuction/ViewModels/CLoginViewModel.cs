using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CardAuction.ViewModels
{
    public class CLoginViewModel
    {

        public string Account { get; set; }
        [Required]
        public string Password { get; set; }
    }
}