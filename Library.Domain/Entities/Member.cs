namespace Library.Domain.Entities;

public class Member
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;

    // Propriedade de navegação, um membro pode ter vários empréstimos
    public virtual ICollection<Loan> Loans { get; set; } = new List<Loan>();
}