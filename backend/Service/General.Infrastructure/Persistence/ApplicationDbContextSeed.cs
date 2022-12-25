using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using General.Domain.Common;
using General.Domain.Enums;
using General.Domain.Entities;
using General.Domain.Enumerations;
using System.IO;
using Common.Shared.Enums;

namespace General.Infrastructure.Persistence
{
    public static class ApplicationDbContextSeed
    {
        public static async Task SeedDefaultUserAsync(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            // admin @grex solutions
            var accountAdminGrex = new ApplicationUser
            {
                UserName = "admin",
                Email = "admin@grex-solutions.com"
            };
            if (userManager.Users.All(u => u.UserName != accountAdminGrex.UserName))
            {
                accountAdminGrex.LockoutEnabled = false;
                accountAdminGrex.LockoutEnd = DateTime.Now;
                await userManager.CreateAsync(accountAdminGrex, "Aa@123456789");
                accountAdminGrex = await userManager.FindByNameAsync(accountAdminGrex.UserName);
            }

            var adminGrex = await userManager.FindByNameAsync("admin");
            if (adminGrex != null)
            {
                await userManager.AddToRoleAsync(adminGrex, Roles.SystemAdministrator);
                if (adminGrex.EmailConfirmed == false)
                {
                    adminGrex.EmailConfirmed = true;
                    adminGrex.PhoneNumberConfirmed = true;
                    await userManager.UpdateAsync(adminGrex);
                }

                var profileAdmin = context.ProfileInformation.Where(x => x.UserId.ToString() == adminGrex.Id).FirstOrDefault();
                if(profileAdmin == null)
                {
                    var profile = new ProfileInformationEntity()
                    {
                        UserId = Guid.Parse(adminGrex.Id),
                        IsDeleted = DeletedStatus.False,
                        LastName = "Admin",
                        FirstName = "Admin"
                    };

                    context.ProfileInformation.AddRange(profile);
                } 
                else
                {
                    profileAdmin.UserId = Guid.Parse(adminGrex.Id);
                    profileAdmin.IsDeleted = DeletedStatus.False;
                    profileAdmin.LastName = "Admin";
                    profileAdmin.FirstName = "Admin";
                }
            }

            // add admin vietnamhome account
            var accountAdmin = new ApplicationUser
            {
                UserName = "administrator",
                Email = "admin@vietnamhomes.ca"
            };
            if (userManager.Users.All(u => u.UserName != accountAdmin.UserName))
            {
                accountAdmin.LockoutEnabled = false;
                accountAdmin.LockoutEnd = DateTime.Now;
                await userManager.CreateAsync(accountAdmin, "Aa@123456789");
                accountAdmin = await userManager.FindByNameAsync(accountAdmin.UserName);
            }

            var admin = await userManager.FindByNameAsync("administrator");
            if (admin != null)
            {
                await userManager.AddToRoleAsync(admin, Roles.SystemAdministrator);
                if (admin.EmailConfirmed == false)
                {
                    admin.EmailConfirmed = true;
                    admin.PhoneNumberConfirmed = true;
                    await userManager.UpdateAsync(admin);
                }

                var profileAdmin = context.ProfileInformation.Where(x => x.UserId.ToString() == admin.Id).FirstOrDefault();
                if (profileAdmin == null)
                {
                    var profile = new ProfileInformationEntity()
                    {
                        UserId = Guid.Parse(admin.Id),
                        IsDeleted = DeletedStatus.False,
                        LastName = "Administrator",
                        FirstName = ""
                    };

                    context.ProfileInformation.AddRange(profile);
                }
                else
                {
                    profileAdmin.UserId = Guid.Parse(admin.Id);
                    profileAdmin.IsDeleted = DeletedStatus.False;
                    profileAdmin.LastName = "administrator";
                    profileAdmin.FirstName = "";
                }
            }

            // add local service account
            ApplicationUser localAccount = userManager.Users.Where(t => t.UserName == "localservice").FirstOrDefault();
            if (localAccount == null)
            {
                localAccount = new ApplicationUser
                {
                    UserName = "localservice",
                    Email = "local@grex-solutions.com",
                    LockoutEnabled = false,
                    LockoutEnd = DateTime.Now
                };
                await userManager.CreateAsync(localAccount, "Localservice@123");
                localAccount = await userManager.FindByNameAsync("localservice");
                await userManager.AddToRoleAsync(localAccount, Roles.LocalService);
                if (localAccount.EmailConfirmed == false)
                {
                    localAccount.EmailConfirmed = true;
                    await userManager.UpdateAsync(localAccount);
                }
            }

            var local = await userManager.FindByNameAsync("localservice");
            if (local != null)
            {
                var profileLocal = context.ProfileInformation.Where(x => x.UserId.ToString() == local.Id).FirstOrDefault();
                if (profileLocal == null)
                {
                    var profile = new ProfileInformationEntity()
                    {
                        UserId = Guid.Parse(local.Id),
                        IsDeleted = DeletedStatus.False,
                        LastName = "Service",
                        FirstName = "Local"
                    };

                    context.ProfileInformation.AddRange(profile);
                }
                else
                {
                    profileLocal.UserId = Guid.Parse(local.Id);
                    profileLocal.IsDeleted = DeletedStatus.False;
                    profileLocal.LastName = "Service";
                    profileLocal.FirstName = "Local";
                }
            }

            await context.SaveChangesAsync();
        }

        public static async Task SeedAttachmentTypesAsync(ApplicationDbContext context)
        {
            // Seed, if necessary
            if (!context.AttachmentType.Any())
            {
                var attachmentTypes = new List<AttachmentTypeEntity>
                {
                    new AttachmentTypeEntity
                    {
                        Name = AttachmentTypes.photo,
                    },
                    new AttachmentTypeEntity
                    {
                       Name = AttachmentTypes.video,
                    },
                    new AttachmentTypeEntity
                    {
                       Name = AttachmentTypes.excel,
                    },
                     new AttachmentTypeEntity
                    {
                       Name = AttachmentTypes.audio,
                    }
                };

                context.AttachmentType.AddRange(attachmentTypes);
                await context.SaveChangesAsync();
            }
        }

        public static async Task SeedImageCategoryAsync(ApplicationDbContext context)
        {
            // Seed, if necessary
            if (!context.ImageCategory.Any())
            {
                var imageCategory = new List<ImageCategoryEntity>
                {
                    new ImageCategoryEntity
                    {
                        Name = ImageCategories.property,
                    },
                    new ImageCategoryEntity
                    {
                       Name = ImageCategories.news,
                    },
                    new ImageCategoryEntity
                    {
                       Name = ImageCategories.user,
                    },
                    new ImageCategoryEntity
                    {
                       Name = ImageCategories.banner,
                    },
                    new ImageCategoryEntity
                    {
                       Name = ImageCategories.site,
                    }
                };

                context.ImageCategory.AddRange(imageCategory);
                await context.SaveChangesAsync();
            } else
            {
                var imageCategory = context.ImageCategory;
                if(imageCategory.Where(x => x.Name == "Property").Count() == 0)
                {
                    var item = new ImageCategoryEntity()
                    {
                        Name = ImageCategories.property,
                    };

                    context.ImageCategory.Add(item);
                    await context.SaveChangesAsync();
                }
                if (imageCategory.Where(x => x.Name == "News").Count() == 0)
                {
                    var item = new ImageCategoryEntity()
                    {
                        Name = ImageCategories.news,
                    };

                    context.ImageCategory.Add(item);
                    await context.SaveChangesAsync();
                }
                if (imageCategory.Where(x => x.Name == "User").Count() == 0)
                {
                    var item = new ImageCategoryEntity()
                    {
                        Name = ImageCategories.user,
                    };

                    context.ImageCategory.Add(item);
                    await context.SaveChangesAsync();
                }
                if (imageCategory.Where(x => x.Name == "Banner").Count() == 0)
                {
                    var item = new ImageCategoryEntity()
                    {
                        Name = ImageCategories.banner,
                    };

                    context.ImageCategory.Add(item);
                    await context.SaveChangesAsync();
                }
                if (imageCategory.Where(x => x.Name == "Site").Count() == 0)
                {
                    var item = new ImageCategoryEntity()
                    {
                        Name = ImageCategories.site,
                    };

                    context.ImageCategory.Add(item);
                    await context.SaveChangesAsync();
                } 
                if (imageCategory.Where(x => x.Name == "Project").Count() == 0)
                {
                    var item = new ImageCategoryEntity()
                    {
                        Name = ImageCategories.project,
                    };

                    context.ImageCategory.Add(item);
                    await context.SaveChangesAsync();
                }
            }
        }

        public static async Task SeedDefaultRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            // SystemAdministrator
            bool exist = await roleManager.RoleExistsAsync(Roles.SystemAdministrator);
            if (exist == false)
            {
                IdentityRole role = new IdentityRole
                {
                    Name = Roles.SystemAdministrator
                };
                await roleManager.CreateAsync(role);
            }

            // InternalUser
            exist = await roleManager.RoleExistsAsync(Roles.InternalUser);
            if (exist == false)
            {
                IdentityRole role = new IdentityRole
                {
                    Name = Roles.InternalUser
                };
                await roleManager.CreateAsync(role);
            }

            // LocalService
            exist = await roleManager.RoleExistsAsync(Roles.LocalService);
            if (exist == false)
            {
                IdentityRole role = new IdentityRole
                {
                    Name = Roles.LocalService
                };
                await roleManager.CreateAsync(role);
            }

            // Seller
            exist = await roleManager.RoleExistsAsync(Roles.Seller);
            if (exist == false)
            {
                IdentityRole role = new IdentityRole
                {
                    Name = Roles.Seller
                };
                await roleManager.CreateAsync(role);
            }
            
            // Guest
            exist = await roleManager.RoleExistsAsync(Roles.Guest);
            if (exist == false)
            {
                IdentityRole role = new IdentityRole
                {
                    Name = Roles.Guest
                };
                await roleManager.CreateAsync(role);
            }
            
            // Supplier
            //exist = await roleManager.RoleExistsAsync(Roles.Supplier);
            //if (exist == false)
            //{
            //    IdentityRole role = new IdentityRole
            //    {
            //        Name = Roles.Supplier
            //    };
            //    await roleManager.CreateAsync(role);
            //}
        }

        public static async Task SeedFolder(ApplicationDbContext context)
        {
            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\\photos");
            if (!Directory.Exists(pathToSave))
            {
                Directory.CreateDirectory(pathToSave);
            }

            pathToSave = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\\photos\\property");
            if (!Directory.Exists(pathToSave))
            {
                Directory.CreateDirectory(pathToSave);
            }

            pathToSave = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\\photos\\news");
            if (!Directory.Exists(pathToSave))
            {
                Directory.CreateDirectory(pathToSave);
            }

            pathToSave = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\\photos\\user");
            if (!Directory.Exists(pathToSave))
            {
                Directory.CreateDirectory(pathToSave);
            }
                        
            pathToSave = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\\photos\\site");
            if (!Directory.Exists(pathToSave))
            {
                Directory.CreateDirectory(pathToSave);
            }

            pathToSave = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\\photos\\banner");
            if (!Directory.Exists(pathToSave))
            {
                Directory.CreateDirectory(pathToSave);
            } 
            
            pathToSave = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\\photos\\project");
            if (!Directory.Exists(pathToSave))
            {
                Directory.CreateDirectory(pathToSave);
            }

            pathToSave = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\\audios");
            if (!Directory.Exists(pathToSave))
            {
                Directory.CreateDirectory(pathToSave);
            }

            pathToSave = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\\videos");
            if (!Directory.Exists(pathToSave))
            {
                Directory.CreateDirectory(pathToSave);
            }
        }

        public static async Task SeedUserTestingAsync(UserManager<ApplicationUser> userManager)
        {
            
        }
    }
}