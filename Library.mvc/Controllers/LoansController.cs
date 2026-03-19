using Library.Domain.Entities;
using Library.mvc.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Library.mvc.Controllers;

public class LoansController : Controller
{
    private readonly LibraryDbContext _libraryDbContext;

    public LoansController(LibraryDbContext libraryDbContext)
    {
        _libraryDbContext = libraryDbContext;
    }

    //lista todos os empréstimos
    public async Task<IActionResult> Index()
    {
        // Include carrega os dados relacionados de Book e Member junto com o Loan
        var loansList = await _libraryDbContext.Loans
            .Include(loan => loan.Book)
            .Include(loan => loan.Member)
            .ToListAsync();

        return View(loansList);
    }

    //formulário de criação
    public async Task<IActionResult> Create()
    {
        // Carrega apenas os livros disponíveis
        ViewData["AvailableBooks"] = await _libraryDbContext.Books
            .Where(book => book.IsAvailable == true)
            .ToListAsync();

        // Carrega todos os membros para o dropdown
        ViewData["MembersList"] = await _libraryDbContext.Members.ToListAsync();

        return View();
    }

    // Recebe e salva os dados do formulário
    [HttpPost]
    public async Task<IActionResult> Create(Loan newLoan)
    {
        // Busca o livro selecionado no banco
        var selectedBook = await _libraryDbContext.Books.FindAsync(newLoan.BookId);

        // Validação, impede emprestar livro indisponível
        if (selectedBook == null || selectedBook.IsAvailable == false)
        {
            ModelState.AddModelError("BookId", "This book is not available for loan.");
        }
        
        ModelState.Remove("Book");
        ModelState.Remove("Member");

        if (ModelState.IsValid)
        {
            // Marca o livro como indisponível
            selectedBook!.IsAvailable = false;

            // Define a data do empréstimo como hoje
            newLoan.LoanDate = DateTime.Today;

            //data de devolução
            newLoan.DueDate = DateTime.Today.AddDays(14);

            _libraryDbContext.Loans.Add(newLoan);
            await _libraryDbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Se houver erro recarrega os dropdowns
        ViewData["AvailableBooks"] = await _libraryDbContext.Books
            .Where(book => book.IsAvailable == true)
            .ToListAsync();

        ViewData["MembersList"] = await _libraryDbContext.Members.ToListAsync();

        return View(newLoan);
    }

    // exibe a confirmação de devolução
    public async Task<IActionResult> Return(int loanId)
    {
        var loanToReturn = await _libraryDbContext.Loans
            .Include(loan => loan.Book)
            .Include(loan => loan.Member)
            .FirstOrDefaultAsync(loan => loan.Id == loanId);

        if (loanToReturn == null)
        {
            return NotFound();
        }

        return View(loanToReturn);
    }

    //Executa a devolução
    [HttpPost, ActionName("Return")]
    public async Task<IActionResult> ReturnConfirmed(int loanId)
    {
        var loanToReturn = await _libraryDbContext.Loans
            .Include(loan => loan.Book)
            .FirstOrDefaultAsync(loan => loan.Id == loanId);

        if (loanToReturn != null)
        {
            // Marca a data de devolução como hoje
            loanToReturn.ReturnedDate = DateTime.Today;

            // Marca o livro como disponível novamente
            loanToReturn.Book.IsAvailable = true;

            await _libraryDbContext.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }
}