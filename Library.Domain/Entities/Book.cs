using Library.Domain.Enums;

namespace Library.Domain.Entities;

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Isbn { get; set; } = string.Empty;
    public BookCategory Category { get; set; }
    public bool IsAvailable { get; set; } = true;

    // Propriedade de navegação: Um livro pode ter vários registros de empréstimo
    public virtual ICollection<Loan> Loans { get; set; } = new List<Loan>();
}