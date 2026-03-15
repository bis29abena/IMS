using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using IMS.WebApp.Data;

namespace IMS.WebApp.Data
{
    public class IMSIdentityContext(DbContextOptions<IMSIdentityContext> options) : IdentityDbContext<IMSWebAppUser>(options)
    {
    }
}
