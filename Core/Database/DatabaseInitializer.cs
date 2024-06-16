using Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Database;
public class DatabaseInitializer
{
    private readonly ILogger _logger;
    private ApplicationDbContext _context;
    private UserManager<ApplicationUser> _userManager;
    private RoleManager<ApplicationRole> _roleManager;

    public DatabaseInitializer(ApplicationDbContext context, ILogger<DatabaseInitializer> logger, RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
    }

    public List<RetailChain> RetailChains { get; set; } =
    [
        new(){ Name = "Opti View"},
        new(){ Name = "FocalPoint"},
        new(){ Name = "Focus Optics"},
        new(){ Name = "ClearSight"}
    ];

    /// <summary>
    ///  Run the init database setup and create the init admin user
    ///  Different parameters allowed for different configurations between debug and prod env
    /// </summary>
    /// <param name="EnsureCleanState">Ensure if database need to be clean state. (this also delete tables and create them again)</param>
    /// <param name="CheckExistedChains">If you dont want the dummy chains data to be in the database. False means it will be created, true means it will skip and not be created</param>
    /// <param name="CheckExistsStores">If you dont want the dummy store data to be in the database. False means it will be created, true means it will skip and not be created</param>
    /// <param name="CreateAdmin">If the default admin user should be created</param>
    /// <returns>A flag if it runs smooth or false if it failed</returns>
    public async Task<bool> Run(bool EnsureCleanState = true, bool CheckExistedChains = false, bool CheckExistsStores = false, bool CreateAdmin = true)
    {
        //Ensure that database is always in clean state. However it can be disabled.
        if(EnsureCleanState)
        {
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
        }

        //Check if retailchains should be checked if they exist and if not create them
        if (!CheckExistedChains)
        {
            try
            {
                foreach (RetailChain chain in RetailChains)
                {
                    if (!_context.RetailChains.Any(x => x.Name == chain.Name))
                    {
                        _context.RetailChains.Add(chain);
                    }
                }
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

            }
            
        }

        if (!CheckExistsStores)
        {
            try
            {
                //stores that belong to chains
                RetailChains[0].Stores = [
                    new RetailStore { Name = "Opti view Randers", Number = 1 },
                    new RetailStore { Name = "Opti view Viborg", Number = 2 },
                ];

                RetailChains[1].Stores = [
                    new RetailStore { Name = "Focalpoint Aalborg", Number = 3 },
                ];

                await _context.SaveChangesAsync();

                //single store without a chain
                await _context.RetailStores.AddAsync(new RetailStore { Name = "Jens butik", Number = 4 });
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

            }
            
        }

        if (CreateAdmin)
        {
            try
            {
                //Create roles
                await CreateRole(UserRoles.Support);
                await CreateRole(UserRoles.Admin);

                //Create the admin user
                ApplicationUser? adminUser = await CreateAdminUser("8ang5npremp2@opayq.com", "Password1");
                if (adminUser == null)
                {
                    //returned because we cannot created admin user for whatever reasons
                    return false;
                }

                //await SetPasswordForUser(adminUser, "Password1");
                if (!await _userManager.IsInRoleAsync(adminUser, UserRoles.Admin))
                {
                    await _userManager.AddToRoleAsync(adminUser, UserRoles.Admin);
                }
            }
            catch (Exception)
            {

            }
        }

        return true;
    }

    private async Task CreateRole(string role)
    {
        _logger.LogInformation($"Create the role `{role}` for application");
        IdentityResult result = await _roleManager.CreateAsync(new ApplicationRole { Name = role });
        if (result.Succeeded)
        {
            _logger.LogDebug($"Created the role `{role}` successfully");
        }
        else
        {
            ApplicationException exception = new ApplicationException($"Default role `{role}` cannot be created");
            _logger.LogError(exception.Message);
            throw exception;
        }
    }

    private async Task<ApplicationUser?> CreateAdminUser(string email, string password)
    {
        _logger.LogInformation($"Create default user with email `{email}` for application");

        var adminUser = new ApplicationUser
        {
            UserName = "admin",
            Email = email,
            EmailConfirmed = true,
            PhoneNumber = "",
            PhoneNumberConfirmed = true,
            FirstName = "Admin",
            LastName = "Admin",
            CreatedOn = DateTime.UtcNow,
            ModifiedOn = DateTime.UtcNow
        };

        IdentityResult result = await _userManager.CreateAsync(adminUser, password);
        if (result.Succeeded)
        {
            _logger.LogDebug($"Created default user `{email}` successfully");
        }
        else
        {
            ApplicationException exception = new ApplicationException($"Default user `{email}` cannot be created");
            _logger.LogError(exception.Message);
            return null;
        }

        ApplicationUser? createdUser = await _userManager.FindByEmailAsync(email);
        return createdUser;
    }
}
