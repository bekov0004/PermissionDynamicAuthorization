using Infrastructure.Data;
using Microsoft.AspNetCore.Identity; 
using Infrastructure.Constants;

namespace Infrastructure.Seed;

public class SeedData
{
    public static void Seed(DataContext context, UserManager<IdentityUser<Guid>> userManager)
    {
        if (context.Roles.Any()) return;
        var roles = new List<IdentityRole<Guid>>()
        {
            new IdentityRole < Guid >()
            { Id = new Guid("9aadec6e-d0f7-4748-b70c-fe9dfc1100bf"), NormalizedName = Roles.Admin.ToUpper(),Name = Roles.Admin},
            new IdentityRole < Guid >()
            { Id = new Guid("0d34aa53-9ae9-4402-964a-40d818a70ca1"), NormalizedName = Roles.SuperAdmin.ToUpper(),Name = Roles.SuperAdmin}, 
            new IdentityRole < Guid >()
                { Id = new Guid("e97f4ccb-3fe3-43c9-ab8f-99664d92d3ce"), NormalizedName = Roles.User.ToUpper(),Name = Roles.User},
        };
        context.Roles.AddRangeAsync(roles);
        context.SaveChanges();

        if (context.Users.Any()) return;
        var user = new IdentityUser<Guid>()
        {
            Id = new Guid("b86f4ccb-3fe3-43c9-ab8f-99664d92d56c"), 
            PhoneNumber = "988181158",   
            UserName = "anushervon", 
            Email = "bekovanushervon@gmail.com", 
        };

        userManager.CreateAsync(user, "A12345"); 
        context.SaveChanges(); 

        var user1 = new IdentityUser<Guid>()
        {
            Id = new Guid("a86f4ccb-3fe3-43c9-ab8f-99664d92d56c"), 
            PhoneNumber = "988181158",   
            UserName = "abdullah", 
            Email = "abullohsheralizoda@gmail.com", 
        };

        userManager.CreateAsync(user1, "A12345"); 
        context.SaveChanges(); 

        var user2 = new IdentityUser<Guid>()
        {
            Id = new Guid("866e2d7b-f5ea-413e-a378-3177535a97bc"), 
            PhoneNumber = "988181158",   
            UserName = "muhammad", 
            Email = "muhammadsheralizoda@gmail.com", 
        };

        userManager.CreateAsync(user2, "A12345"); 
        context.SaveChanges(); 

        var user3 = new IdentityUser<Guid>()
        {
            Id = new Guid("ef039dc7-5ff3-4c84-b1b4-48d183754ff7"), 
            PhoneNumber = "988181158",   
            UserName = "surush", 
            Email = "surushsheralizoda@gmail.com", 
        };

        userManager.CreateAsync(user3, "A12345"); 
        context.SaveChanges(); 


        if (context.UserRoles.Any()) return;
        var userRole = new List<IdentityUserRole<Guid>>()
        {
            new IdentityUserRole<Guid>()
            {
                RoleId = new Guid("0d34aa53-9ae9-4402-964a-40d818a70ca1"),
                UserId = new Guid("b86f4ccb-3fe3-43c9-ab8f-99664d92d56c"),
            },
            new IdentityUserRole<Guid>()
            {
                RoleId = new Guid("e97f4ccb-3fe3-43c9-ab8f-99664d92d3ce"),
                UserId = new Guid("a86f4ccb-3fe3-43c9-ab8f-99664d92d56c"),
            }
        };
        context.UserRoles.AddRange(userRole);
        context.SaveChanges();





    }
}