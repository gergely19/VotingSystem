using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VotingSystem.DataAccess.Models
{
    public class UserRole : IdentityRole
    {
        public UserRole() { }
        public UserRole(string role) : base(role) { }
    }
}
