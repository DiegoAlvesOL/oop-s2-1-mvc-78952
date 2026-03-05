namespace Library.Domain.Entities;

public class Loan
{
    public int Id { get; set; }

    // Chaves Estrangeiras (Nomes claros e diretos)
    public int BookId { get; set; }
    public int MemberId { get; set; }

    public DateTime LoanDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnedDate { get; set; } // Nullable: nulo enquanto não for devolvido

    // Propriedades de navegação (Essenciais para o Entity Framework)
    public virtual Book Book { get; set; } = null!;
    public virtual Member Member { get; set; } = null!;
}