using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DAB.Web.Data
{
    public class WebDbContext : IdentityDbContext
    {
        public WebDbContext(DbContextOptions<WebDbContext> options)
            : base(options)
        {
        }
    }
}