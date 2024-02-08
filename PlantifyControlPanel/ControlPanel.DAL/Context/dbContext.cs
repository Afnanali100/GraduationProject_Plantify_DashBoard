using ControlPanel.DAL.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlPanel.DAL.Context
{
    public class dbContext:IdentityDbContext<ApplicationUser>
    {
        public dbContext(DbContextOptions<dbContext> options):base(options) 
        {

        }


    }
}
