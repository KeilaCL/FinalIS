using System;
using System.Collections.Generic;

namespace FinalIS.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Usernombre { get; set; } = null!;

        public string Password { get; set; } = null!;
    }
}
