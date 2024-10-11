﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMMS.DAL.Entities
{
    public class AppUser:IdentityUser
    {
        public string? ConnectionId { get; set; }
    }
}
