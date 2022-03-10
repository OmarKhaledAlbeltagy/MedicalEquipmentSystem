using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AMEKSA.Models
{
    public class EmailModel
    {
        public static string SmtpServer { get; } = "mail.ameksaapii.xyz";

        public static int port { get; } = 8889;  //465 //8889

        public static string EmailAddress { get; } = "ame@ameksaapii.xyz";

        public static string Password { get; } = "Adminpass@1";
    }
}
