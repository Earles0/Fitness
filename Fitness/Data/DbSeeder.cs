using Fitness.Models;
using Microsoft.AspNetCore.Identity;

    namespace Fitness.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // ✅ 1. Roller oluştur (Admin ve Üye)
            string[] roles = { "Admin", "Uye" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // ✅ 2. Admin kullanıcı oluştur
            // ⚠️ ÖĞRENCİ NUMARANIZI BURAYA YAZIN!
            // İlk admin (mevcut)
            var adminEmail = "b221210383@sakarya.edu.tr";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var adminUser = new AppUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    AdSoyad = "Admin Kullanıcı",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, "sau");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            // ✅ İKİNCİ ADMİN EKLE (YENİ)
            var adminEmail2 = "admin@sakarya.edu.tr"; // İstediğiniz email
            if (await userManager.FindByEmailAsync(adminEmail2) == null)
            {
                var adminUser2 = new AppUser
                {
                    UserName = adminEmail2,
                    Email = adminEmail2,
                    AdSoyad = "İkinci Admin",
                    EmailConfirmed = true
                };

                var result2 = await userManager.CreateAsync(adminUser2, "admin"); // Şifre
                if (result2.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser2, "Admin");
                }
            }

            // ✅ 3. Örnek Salon Ekleme
            if (!context.Salonlar.Any())
            {
                var salon = new Salon
                {
                    Ad = "Merkez Fitness Salonu",
                    Adres = "Sakarya Üniversitesi Esentepe Kampüsü",
                    CalismaSaatleri = "08:00 - 22:00"
                };
                context.Salonlar.Add(salon);
                await context.SaveChangesAsync();

                // ✅ 4. Örnek Hizmetler
                var hizmetler = new List<Hizmet>
                {
                    new Hizmet 
                    { 
                        Ad = "Fitness & Kilo Verme", 
                        SureDakika = 60, 
                        Ucret = 200, 
                        SalonId = salon.Id 
                    },
                    new Hizmet 
                    { 
                        Ad = "Yoga", 
                        SureDakika = 45, 
                        Ucret = 150, 
                        SalonId = salon.Id 
                    },
                    new Hizmet 
                    { 
                        Ad = "Pilates", 
                        SureDakika = 50, 
                        Ucret = 180, 
                        SalonId = salon.Id 
                    },
                    new Hizmet 
                    { 
                        Ad = "Kas Geliştirme", 
                        SureDakika = 60, 
                        Ucret = 220, 
                        SalonId = salon.Id 
                    }
                };
                context.Hizmetler.AddRange(hizmetler);
                await context.SaveChangesAsync();

                // ✅ 5. Örnek Antrenörler
                var antrenor1 = new Antrenor
                {
                    AdSoyad = "Ahmet Yılmaz",
                    UzmanlikAlani = "Kas Geliştirme, Fitness",
                    SalonId = salon.Id
                };
                
                var antrenor2 = new Antrenor
                {
                    AdSoyad = "Ayşe Demir",
                    UzmanlikAlani = "Yoga, Pilates, Kilo Verme",
                    SalonId = salon.Id
                };

                context.Antrenorler.AddRange(antrenor1, antrenor2);
                await context.SaveChangesAsync();

                // ✅ 6. Antrenör-Hizmet İlişkileri (Many-to-Many)
                context.AntrenorHizmetler.AddRange(new List<AntrenorHizmet>
                {
                    // Ahmet - Fitness & Kas Geliştirme
                    new AntrenorHizmet { AntrenorId = antrenor1.Id, HizmetId = hizmetler[0].Id },
                    new AntrenorHizmet { AntrenorId = antrenor1.Id, HizmetId = hizmetler[3].Id },
                    
                    // Ayşe - Yoga, Pilates, Kilo Verme
                    new AntrenorHizmet { AntrenorId = antrenor2.Id, HizmetId = hizmetler[0].Id },
                    new AntrenorHizmet { AntrenorId = antrenor2.Id, HizmetId = hizmetler[1].Id },
                    new AntrenorHizmet { AntrenorId = antrenor2.Id, HizmetId = hizmetler[2].Id }
                });

                // ✅ 7. Antrenör Müsaitlik Saatleri
                context.AntrenorMusaitlikler.AddRange(new List<AntrenorMusaitlik>
                {
                    // Ahmet - Pazartesi, Çarşamba, Cuma
                    new AntrenorMusaitlik 
                    { 
                        AntrenorId = antrenor1.Id, 
                        Gun = DayOfWeek.Monday, 
                        BaslangicSaati = new TimeSpan(9, 0, 0), 
                        BitisSaati = new TimeSpan(17, 0, 0) 
                    },
                    new AntrenorMusaitlik 
                    { 
                        AntrenorId = antrenor1.Id, 
                        Gun = DayOfWeek.Wednesday, 
                        BaslangicSaati = new TimeSpan(9, 0, 0), 
                        BitisSaati = new TimeSpan(17, 0, 0) 
                    },
                    new AntrenorMusaitlik 
                    { 
                        AntrenorId = antrenor1.Id, 
                        Gun = DayOfWeek.Friday, 
                        BaslangicSaati = new TimeSpan(9, 0, 0), 
                        BitisSaati = new TimeSpan(17, 0, 0) 
                    },

                    // Ayşe - Salı, Perşembe, Cumartesi
                    new AntrenorMusaitlik 
                    { 
                        AntrenorId = antrenor2.Id, 
                        Gun = DayOfWeek.Tuesday, 
                        BaslangicSaati = new TimeSpan(10, 0, 0), 
                        BitisSaati = new TimeSpan(18, 0, 0) 
                    },
                    new AntrenorMusaitlik 
                    { 
                        AntrenorId = antrenor2.Id, 
                        Gun = DayOfWeek.Thursday, 
                        BaslangicSaati = new TimeSpan(10, 0, 0), 
                        BitisSaati = new TimeSpan(18, 0, 0) 
                    },
                    new AntrenorMusaitlik 
                    { 
                        AntrenorId = antrenor2.Id, 
                        Gun = DayOfWeek.Saturday, 
                        BaslangicSaati = new TimeSpan(10, 0, 0), 
                        BitisSaati = new TimeSpan(16, 0, 0) 
                    }
                });

                await context.SaveChangesAsync();
            }
        }
    }
}
