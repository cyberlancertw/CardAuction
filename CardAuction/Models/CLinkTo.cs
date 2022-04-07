using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CardAuction.Models
{
    public class CLinkTo
    {
        public string ToController { get; set; }
        public string ToAction { get; set; }
        public string ToId { get; set; }
        public CLinkTo(string controller, string action)
        {
            this.ToController = controller;
            this.ToAction = action;
            this.ToId = string.Empty;
        }
        public CLinkTo(string controller, string action, string id)
        {
            this.ToController = controller;
            this.ToAction = action;
            this.ToId = id;
        }

    }
}