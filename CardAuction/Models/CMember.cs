using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CardAuction.Models
{
    /*create table tMember(
        fUserId int identity(1,1) primary key,
        fAccount varchar(20),
        fPassword varchar(50),
        fName nvarchar(20),
        fEmail varchar(50),
        fAddress nvarchar(50),
        fPhone varchar(20),
        fBirthday date,
        fSubscribe bit,
        fManager bit,
        fActive bit
    );*/

    public class CMember
    {
        public int UserId { get; set; }
        public string Account { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public DateTime Birthday { get; set; }
        public bool Subscribe { get; set; }
        public bool Manager { get; set; }
        public bool Active { get; set; }
    }
}