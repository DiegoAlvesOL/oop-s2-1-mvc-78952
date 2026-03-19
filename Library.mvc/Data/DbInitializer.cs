using Bogus;
using Library.Domain.Entities;
using Library.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Library.mvc.Data;

public static class DBInitializer
{
    public static void InsertInitialData(LibraryDbContext contextLibrary)
    {
        // Verifica se tem algum dado no banco se tiver não faz nada.
        if (contextLibrary.Books.Any())
        {
            return;
        }
        // Acessando o enum BookCategory para gerar os livros baseado na categoria deste enum.
        var bookCategories = Enum.GetValues<BookCategory>();
        
        // Criando fabricante falso usando classe Fake do Bogus
        var bookFaker =  new Faker<Book>()
            .RuleFor(book => book.Title, faker => faker.Lorem.Sentence(3))
            .RuleFor(book => book.Author, faker => faker.Name.FullName())
            .RuleFor(book => book.Isbn,        faker => faker.Commerce.Ean13())
            .RuleFor(book => book.Category,    faker => faker.PickRandom(bookCategories))
            .RuleFor(book => book.IsAvailable, faker => true);

        // Lista com os 20 livros usando a regra acima bookFake.
        var generatedBooks = bookFaker.Generate(20);
        contextLibrary.Books.AddRange(generatedBooks);

        // Criando lista de membros falso usando a Bogus
        var memberFaker = new Faker<Member>()
            .RuleFor(member => member.FullName, faker => faker.Name.FullName())
            .RuleFor(member => member.Email, faker => faker.Internet.Email())
            .RuleFor(member => member.PhoneNumber, faker => faker.Phone.PhoneNumber());

        // Lista com os 10 membros usando a regra acima memberFaker
        var generatedMembers = memberFaker.Generate(10);
        contextLibrary.Members.AddRange(generatedMembers);

        // Salvando as lista de livros e de membros antes de criar os emprestimos pq vamos usar os IDs
        contextLibrary.SaveChanges();
        
        
        // Criando 5 locações de livros que já foram devolvidos
        var generatedLoans = new List<Loan>();
        var today = DateTime.Today;

        for (int loanIndex = 0; loanIndex < 5; loanIndex++)
        {
            var loanDate = today.AddDays(-30);
            
            generatedLoans.Add(new Loan
            {
                BookId = generatedBooks[loanIndex].Id,
                MemberId = generatedMembers[loanIndex].Id,
                LoanDate = loanDate,
                DueDate = loanDate.AddDays(14),
                ReturnedDate = loanDate.AddDays(10)
            });
            
            // Tornando os livros devolvidos disponíveis para locação
            generatedBooks[loanIndex].IsAvailable = true;
        }
        
        
        // Criando 5 locações, ativas e dentro do prazo 
        for (int loanIndex = 5; loanIndex < 10; loanIndex++)
        {
            var loanDate = today.AddDays(-3);
            
            generatedLoans.Add(new Loan
            {
                BookId = generatedBooks[loanIndex].Id,
                MemberId = generatedMembers[loanIndex].Id,
                LoanDate = loanDate,
                DueDate = loanDate.AddDays(14),
            });
            generatedBooks[loanIndex].IsAvailable = false;
        }

        // Criando 5 locações em curso com atraso para devolução
        for (int loanIndex = 10 ; loanIndex < 15; loanIndex++)
        {
            var loanDate = today.AddDays(-30);


            generatedLoans.Add(new Loan
            {
                BookId = generatedBooks[loanIndex].Id,
                MemberId = generatedMembers[0].Id,
                LoanDate = loanDate,
                DueDate = loanDate.AddDays(14),
            });
            generatedBooks[loanIndex].IsAvailable = false;
        }
        contextLibrary.Loans.AddRange(generatedLoans);
        contextLibrary.SaveChanges();
    }
    
    public static async Task InsertAdminUser(
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        var adminRoleExists = await roleManager.RoleExistsAsync("Admin");
        if (!adminRoleExists)
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        }
        
        var adminEmail = "admin@library.com";
        var adminUserExists = await userManager.FindByEmailAsync(adminEmail);
        if (adminUserExists == null)
        {
            var adminUser = new IdentityUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };
            
            var creationResult = await userManager.CreateAsync(adminUser, "Admin@123");
            
            if (creationResult.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
    
    
}