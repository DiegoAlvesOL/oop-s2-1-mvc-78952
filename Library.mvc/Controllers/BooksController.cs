using Library.Domain.Entities;
using Library.Domain.Enums;
using Library.mvc.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Library.mvc.Controllers;

public class BooksController : Controller
{
    // Conexão com o banco de dados injetada pelo ASP.NET
    private readonly LibraryDbContext _libraryDbContext;

    public BooksController(LibraryDbContext libraryDbContext)
    {
        _libraryDbContext = libraryDbContext;
    }

    //INDEX: Lista todos os livros com busca e filtros
    public async Task<IActionResult> Index(string searchText, string selectedCategory, string selectedAvailability)
    {
        // Começa com todos os livros
        IQueryable<Book> booksQuery = _libraryDbContext.Books;

        // Filtro por título ou autor
        if (!string.IsNullOrEmpty(searchText))
        {
            booksQuery = booksQuery.Where(book => book.Title.Contains(searchText) || book.Author.Contains(searchText));
        }

        // Filtro por categoria, só adiciona à query se o utilizador selecionou uma categoria
        if (!string.IsNullOrEmpty(selectedCategory))
        {
            var bookCategory = Enum.Parse<BookCategory>(selectedCategory);
            booksQuery = booksQuery.Where(book => book.Category == bookCategory);
        }

        // Filtro por disponibilidade
        if (selectedAvailability == "Available")
        {
            booksQuery = booksQuery.Where(book => book.IsAvailable == true);
        }
        else if (selectedAvailability == "OnLoan")
        {
            booksQuery = booksQuery.Where(book => book.IsAvailable == false);
        }

        // Execução da query
        var booksList = await booksQuery.ToListAsync();

        // Envia os valores dos filtros para a View manter o estado dos campos
        
        ViewData["SearchText"] = searchText;
        ViewData["SelectedCategory"] = selectedCategory;
        ViewData["SelectedAvailability"] =selectedAvailability;
        ViewData["CategoryList"] = Enum.GetValues<BookCategory>();

        return View(booksList);
    }

    // CREATE GET: Exibe o formulário de criação
    public IActionResult Create()
    {
        ViewData["CategoryList"] = Enum.GetValues<BookCategory>();
        return View();
    }

    // CREATE POST: Recebe e salva os dados do formulário
    [HttpPost]
    public async Task<IActionResult> Create(Book newBook)
    {
        if (ModelState.IsValid)
        {
            newBook.IsAvailable = true;
            _libraryDbContext.Books.Add(newBook);
            await _libraryDbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewData["CategoryList"] = Enum.GetValues<BookCategory>();
        return View(newBook);
    }

    // EDIT GET: Exibe o formulário de edição
    public async Task<IActionResult> Edit(int bookId)
    {
        var bookToEdit = await _libraryDbContext.Books.FindAsync(bookId);

        if (bookToEdit == null)
        {
            return NotFound();
        }

        ViewData["CategoryList"] = Enum.GetValues<BookCategory>();
        return View(bookToEdit);
    }

    //EDIT POST: Recebe e salva as alterações
    [HttpPost]
    public async Task<IActionResult> Edit(int bookId, Book updatedBook)
    {
        if (bookId != updatedBook.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            _libraryDbContext.Books.Update(updatedBook);
            await _libraryDbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        ViewData["CategoryList"] = Enum.GetValues<BookCategory>();
        return View(updatedBook);
    }

    // DELETE GET: Exibe a confirmação de deleção
    public async Task<IActionResult> Delete(int bookId)
    {
        var bookToDelete = await _libraryDbContext.Books.FindAsync(bookId);

        if (bookToDelete == null)
        {
            return NotFound();
        }

        return View(bookToDelete);
    }

    // DELETE POST: Executa a deleção
    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int bookId)
    {
        var bookToDelete = await _libraryDbContext.Books.FindAsync(bookId);

        if (bookToDelete != null)
        {
            _libraryDbContext.Books.Remove(bookToDelete);
            await _libraryDbContext.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }
}