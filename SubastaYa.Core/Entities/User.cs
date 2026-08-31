using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Text;

namespace SubastaYa.Core.Entities
{
    internal class User
    {
        public int Id {  get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }

        public DateTime Created { get; set; }
    }
}
