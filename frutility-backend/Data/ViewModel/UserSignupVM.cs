using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace frutility_backend.Data.ViewModel
{
    public class UserSignupVM
    {
        public string fname { get; set; }
        public string lname { get; set; }
        public string email { get; set; }
        public string sAddress { get; set; }
        public string sState { get; set; }
        public string sCity{ get; set; }
        public string bAddress { get; set; }
        public string bState { get; set; }
        public string bCity { get; set; }
        public string phone { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string cpassword { get; set; }
    }
}
