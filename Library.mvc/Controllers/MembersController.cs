using Library.Domain.Entities;
using Library.mvc.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Library.mvc.Controllers;

public class MembersController : Controller
{
    // Conexão com o banco.
    private readonly LibraryDbContext _libraryDbContext;

    public MembersController(LibraryDbContext libraryDbContext)
    {
        _libraryDbContext = libraryDbContext;
    }

    //INDEX lista todos os membros
    public async Task<IActionResult> Index()
    {
        var membersList = await _libraryDbContext.Members.ToListAsync();
        return View(membersList);
    }

    // CREATE GET exibe o formulário de criação
    public IActionResult Create()
    {
        return View();
    }

    //CREATE POST recebe e salva os dados do formulário
    [HttpPost]
    public async Task<IActionResult> Create(Member newMember)
    {
        if (ModelState.IsValid)
        {
            _libraryDbContext.Members.Add(newMember);
            await _libraryDbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return View(newMember);
    }

    //EDIT GET exibe o formulário de edição
    public async Task<IActionResult> Edit(int memberId)
    {
        var memberToEdit = await _libraryDbContext.Members.FindAsync(memberId);

        if (memberToEdit == null)
        {
            return NotFound();
        }

        return View(memberToEdit);
    }

    //EDIT POST recebe e salva as alterações
    [HttpPost]
    public async Task<IActionResult> Edit(int memberId, Member updatedMember)
    {
        if (memberId != updatedMember.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            _libraryDbContext.Members.Update(updatedMember);
            await _libraryDbContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return View(updatedMember);
    }

    //DELETE GET exibe a confirmação de deleção
    public async Task<IActionResult> Delete(int memberId)
    {
        var memberToDelete = await _libraryDbContext.Members.FindAsync(memberId);

        if (memberToDelete == null)
        {
            return NotFound();
        }

        return View(memberToDelete);
    }

    //DELETE POST: Executa a deleção
    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int memberId)
    {
        var memberToDelete = await _libraryDbContext.Members.FindAsync(memberId);

        if (memberToDelete != null)
        {
            _libraryDbContext.Members.Remove(memberToDelete);
            await _libraryDbContext.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }
}