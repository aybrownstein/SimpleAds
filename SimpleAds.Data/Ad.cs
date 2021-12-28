using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAds.Data
{
    public class Ad
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int PhoneNumber { get; set; }
        public string Description { get; set; }
        public string UserName { get; set; }
        public int UserId { get; set; }
    }
}
