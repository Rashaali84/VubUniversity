using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using VubUniversity.Services;

namespace VubUniversity.Models
{
   
     public class EmailReputaionModel : ValidationAttribute
    {
        public string GetErrorMessage() => "Email address is rejected because of its reputation";
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string email = value.ToString();
            var service = (IEmailCheckRisky)validationContext.GetService(typeof(IEmailCheckRisky));
            if (service.IsUntrusted(email))
                return new ValidationResult(GetErrorMessage());

            return ValidationResult.Success;
        }
    }

    public class Details
    {
        public bool blacklisted { get; set; }
        public bool malicious_activity { get; set; }
        public bool malicious_activity_recent { get; set; }
        public bool spam { get; set; }
        public bool suspicious_tld { get; set; }
    }

    public class Reputation
    {
        public Details details { get; set; }
        public string email { get; set; }
        public string reputation { get; set; }
        public bool suspicious { get; set; }
    }
}
