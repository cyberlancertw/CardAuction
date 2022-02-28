using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CardAuction.ViewModels
{
    public class CRegisterViewModel
    {
        public string Account { get; set; }
        public string Password { get; set; }
        public string PasswordCheck { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string AddressSelect { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public DateTime Birthday { get; set; }
        public bool Subscribe { get; set; }

    }
}