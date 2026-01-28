namespace UiS.Dat240.Lab1.Domain.Entities;

public class Member
{
    // Constants for business rules
    private const int MaxActiveLoans = 5;
    private const decimal MaxFinesBeforeSuspension = 10.00m;

    public string MemberId { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public MemberStatus Status { get; private set; }
    public decimal OutstandingFines { get; private set; }

    private readonly List<Loan> _activeLoans = new();
    public IReadOnlyList<Loan> ActiveLoans => _activeLoans.AsReadOnly();

    /// <summary>
    /// Creates a new library member.
    /// </summary>
    /// <param name="memberId">Unique member identifier (must not be null or empty)</param>
    /// <param name="name">Member's name (must not be null or empty)</param>
    /// <param name="email">Member's email (must not be null or empty and must contain @)</param>
    /// <exception cref="ArgumentException">Thrown when parameters are invalid</exception>
    public Member(string memberId, string name, string email)
    {
        // TODO: Validate parameters
        // - memberId, name, email must not be null or whitespace
        // - email must contain '@' character
        // Initial status should be Active
        // Initial outstanding fines should be 0
        
        if (string.IsNullOrEmpty(memberId))
            throw new ArgumentException("MemberId must not be null or empty");
        if (string.IsNullOrEmpty(name))
            throw new ArgumentException("Name must not be null or empty");
        if (string.IsNullOrEmpty(email))
            throw new ArgumentException("Email must not be null or empty");
        if (!email.Contains("@"))
            throw new ArgumentException("Email must contain '@' character");

        MemberId = memberId;
        Name = name;
        Email = email;
        Status = MemberStatus.Active;
        OutstandingFines = 0;
    }

    /// <summary>
    /// Checks if the member is allowed to borrow books.
    /// </summary>
    /// <returns>True if the member can borrow, false otherwise</returns>
    public bool CanBorrow()
    {
        // TODO: Implement
        // Member can borrow if:
        // - Status is Active
        // - Has fewer than MaxActiveLoans (5) active loans
        // - Outstanding fines are less than MaxFinesBeforeSuspension ($10)
        if (Status != MemberStatus.Active)
            return false;
        if (_activeLoans.Count >= MaxActiveLoans)
            return false;
        if (OutstandingFines >= MaxFinesBeforeSuspension)
            return false;
        return true;
    }

    /// <summary>
    /// Borrows a book for this member.
    /// </summary>
    /// <param name="book">The book to borrow</param>
    /// <exception cref="InvalidOperationException">Thrown when member cannot borrow</exception>
    public void BorrowBook(Book book)
    {
        // TODO: Implement
        // 1. Check if member CanBorrow() - throw InvalidOperationException if not
        // 2. Call book.Borrow(this.MemberId)
        // 3. Create a new Loan with a unique loan ID (you can use Guid.NewGuid().ToString())
        // 4. Add the loan to _activeLoans
        if (!CanBorrow())
            throw new InvalidOperationException("Member cannot borrow at this time.");
        book.Borrow(this.MemberId);
        _activeLoans.Add(new Loan(Guid.NewGuid().ToString(), this.MemberId, book, DateTime.Now));
    }

    /// <summary>
    /// Returns a book and removes it from active loans.
    /// </summary>
    /// <param name="loan">The loan to return</param>
    public void ReturnBook(Loan loan)
    {
        // TODO: Implement
        // 1. Call loan.Return(DateTime.Now)
        // 2. Calculate any fines using loan.CalculateFine()
        // 3. If there are fines, add them using AddFine()
        // 4. Remove the loan from _activeLoans
        
        decimal fine = loan.CalculateFine();
        if (fine > 0)
            AddFine(fine);
        _activeLoans.Remove(loan);
        loan.Return(DateTime.Now);
    }

    /// <summary>
    /// Adds a fine to the member's account.
    /// </summary>
    /// <param name="amount">The fine amount to add</param>
    public void AddFine(decimal amount)
    {
        // TODO: Implement
        // 1. Add amount to OutstandingFines
        // 2. If OutstandingFines >= MaxFinesBeforeSuspension, automatically Suspend()
        OutstandingFines += amount;
        if (OutstandingFines >= MaxFinesBeforeSuspension)
            Suspend();
    }

    /// <summary>
    /// Pays off some or all of the outstanding fines.
    /// </summary>
    /// <param name="amount">The amount to pay</param>
    /// <exception cref="ArgumentException">Thrown when amount is negative or exceeds outstanding fines</exception>
    public void PayFine(decimal amount)
    {
        // TODO: Implement
        // 1. Validate amount is positive and doesn't exceed OutstandingFines
        // 2. Subtract amount from OutstandingFines
        // 3. If member was suspended and fines drop below $10, consider auto-activating (optional)
        if (amount < 0)
            throw new ArgumentException("Amount must be positive");
        if (amount > OutstandingFines)
            throw new ArgumentException("Amount exceeds outstanding fines");
        OutstandingFines -= amount;
        if (Status == MemberStatus.Suspended && OutstandingFines < MaxFinesBeforeSuspension)
            Activate();
    }

    /// <summary>
    /// Suspends the member's account.
    /// </summary>
    public void Suspend()
    {
        // TODO: Implement
        // Change Status to Suspended
        Status = MemberStatus.Suspended;
    }

    /// <summary>
    /// Activates the member's account.
    /// </summary>
    public void Activate()
    {
        // TODO: Implement
        // Change Status to Active
        Status = MemberStatus.Active;
    }

    /// <summary>
    /// Marks the member's account as expired.
    /// </summary>
    public void Expire()
    {
        // TODO: Implement
        // Change Status to Expired
        Status = MemberStatus.Expired;
    }
}

public enum MemberStatus
{
    Active,
    Suspended,
    Expired
}