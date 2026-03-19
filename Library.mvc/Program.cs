using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Library.mvc.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Pegar a string de conexão do MySQL que configuramos no appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// 2. CONFIGURAÇÃO DO MYSQL (Pomelo): Substituímos ApplicationDbContext por LibraryDbContext
builder.Services.AddDbContext<LibraryDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// 3. CONFIGURAÇÃO DO IDENTITY: Adicionamos .AddRoles para suportar Admin/Manager (Critério 5)
builder.Services.AddDefaultIdentity<IdentityUser>(options => 
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.Password.RequireDigit = false;
        options.Password.RequiredLength = 6;
    })
    .AddRoles<IdentityRole>() 
    .AddEntityFrameworkStores<LibraryDbContext>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// 4. POPULAR O BANCO COM DADOS INICIAIS
// Abrimos um escopo para acessar o banco antes da aplicação iniciar
using (var serviceScope = app.Services.CreateScope())
{
    var databaseContext = serviceScope.ServiceProvider.GetRequiredService<LibraryDbContext>();
    var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    
    databaseContext.Database.Migrate();
    DBInitializer.InsertInitialData(databaseContext);
    await DBInitializer.InsertAdminUser(userManager, roleManager);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
    .WithStaticAssets();

app.Run();