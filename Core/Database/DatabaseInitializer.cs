using Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

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
        new(){ ChainName = "Opti View"},
        new(){ ChainName = "FocalPoint"},
        new(){ ChainName = "Focus Optics"},
        new(){ ChainName = "ClearSight"}
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
        if (EnsureCleanState)
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
                    if (!_context.RetailChains.Any(x => x.ChainName == chain.ChainName))
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
                    new RetailStore { StoreName = "Opti view Randers", Number = 1, StoreOwner = "Lars Jensen", PostalCode = "8900", Region = "Midtjylland", City = "Randers", Address = "Emmavej 22", Country = "Danmark", Email = "", Phone = "" },
                    new RetailStore { StoreName = "Opti view Aarhus", Number = 2, StoreOwner = "Hans Nielsen", PostalCode = "8000", Region = "Midtjylland", City = "Aarhus", Address = "Åboulevarden 22", Country = "Danmark", Email = "", Phone = "" },
                    new RetailStore { StoreName = "Opti view København", Number = 3, StoreOwner = "Lars Andersen", PostalCode = "2100", Region = "Hovedstaden", City = "København", Address = "Nørrebrogade 22", Country = "Danmark", Email = "", Phone = "" }
                ];

                RetailChains[1].Stores = [
                    new RetailStore { StoreName = "FocalPoint Odense", Number = 4, StoreOwner = "Hans Olesen", PostalCode = "5000", Region = "Syddanmark", City = "Odense", Address = "Vestergade 22", Country = "Danmark", Email = "", Phone = "" },
                    new RetailStore { StoreName = "FocalPoint Svendborg", Number = 5, StoreOwner = "Emil Jensen", PostalCode = "5700", Region = "Syddanmark", City = "Svendborg", Address = "Havnegade 22", Country = "Danmark", Email = "", Phone = "" }
                ];

                RetailChains[2].Stores = [
                    new RetailStore { StoreName = "Focus Optics Vejle", Number = 6, StoreOwner = "Hans Nielsen", PostalCode = "7100", Region = "Syddanmark", City = "Vejle", Address = "Nørregade 22", Country = "Danmark", Email = "", Phone = "" },
                ];

                RetailChains[3].Stores = [
                ];

                await _context.SaveChangesAsync();

                //single store without a chain
                await _context.RetailStores.AddAsync(new RetailStore { StoreName = "Optia Vejle", Number = 7, StoreOwner = "Mikkel Nielsen", PostalCode = "7100", Region = "Syddanmark", City = "Vejle", Address = "Nørregade 22", Country = "Danmark", Email = "", Phone = "" });
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
