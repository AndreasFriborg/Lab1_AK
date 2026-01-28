using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace UiS.Dat240.Lab1.Domain.Entities;

public class Loan
{
    // Constants for business rules
    private const int LoanPeriodDays = 14;
    private const decimal Tier1DailyRate = 5.00m;  // Days 1-7
    private const decimal Tier2DailyRate = 10.00m;  // Days 8-14
    private const decimal Tier3DailyRate = 20.00m;  // Day 15+

    public string LoanId { get; private set; }
    public string MemberId { get; private set; }
    public Book Book { get; private set; }
    public DateTime BorrowDate { get; private set; }
    public DateTime DueDate { get; private set; }
    public DateTime? ReturnDate { get; private set; }

    public bool IsReturned => ReturnDate.HasValue;
    public bool IsOverdue => !IsReturned && DateTime.Now.Date > DueDate.Date;

    /// <summary>
    /// Creates a new loan.
    /// </summary>
    /// <param name="loanId">Unique loan identifier</param>
    /// <param name="memberId">The member borrowing the book</param>
    /// <param name="book">The book being borrowed</param>
    /// <param name="borrowDate">The date the book was borrowed</param>
    public Loan(string loanId, string memberId, Book book, DateTime borrowDate)
    {
        // TODO: Validate and initialize
        // DueDate should be BorrowDate + LoanPeriodDays (14 days)
        // ReturnDate should be null initially
        if (string.IsNullOrEmpty(loanId))
            throw new ArgumentException("LoanId must not be null or empty.");
        if (string.IsNullOrEmpty(memberId))
            throw new ArgumentException("MemberId must not be null or empty.");
        if (book == null)
            throw new ArgumentException("Book must not be null.");
        

        ReturnDate = null;
        
        DueDate = borrowDate.AddDays(LoanPeriodDays);

        LoanId = loanId;
        MemberId = memberId;
        Book = book;
        BorrowDate = borrowDate;

    }

    /// <summary>
    /// Calculates the fine for this loan based on how overdue it is.
    /// </summary>
    /// <returns>The fine amount in kroner</returns>
    public decimal CalculateFine()
    {
        // TODO: Implement fine calculation
        // Return 0 if:
        // - Loan is already returned (IsReturned is true)
        // - Loan is not overdue (DateTime.Now <= DueDate)
        //
        // Otherwise calculate based on days overdue:
        // - Days 1-7 overdue: Kr 5.00 per day
        // - Days 8-14 overdue: Kr 10.00 per day (in addition to first 7 days)
        // - Day 15+ overdue: Kr 20.00 per day (in addition to first 14 days)
        //
        // Example: 10 days overdue = (7 × Kr 5.00) + (3 × Kr 10.00) = Kr 65.00
        // Example: 20 days overdue = (7 × Kr 5.00) + (7 × Kr 10.00) + (6 × Kr 20.00) = Kr 225.00

        if (DateTime.Now.Date <= DueDate.Date || IsReturned == true)
            return 0;

        if (GetDaysOverdue() <= 7)
            return GetDaysOverdue() * Tier1DailyRate;
        if (GetDaysOverdue() > 7 && GetDaysOverdue() <= 14)
            return 7 * Tier1DailyRate + (GetDaysOverdue() - 7) * Tier2DailyRate;
        if (GetDaysOverdue() > 14)
            return 7 * Tier1DailyRate + 7 * Tier2DailyRate + (GetDaysOverdue() - 14) * Tier3DailyRate;
            
        throw new InvalidOperationException("Invalid inputs.");

    }

    /// <summary>
    /// Marks the loan as returned.
    /// </summary>
    /// <param name="returnDate">The date the book was returned</param>
    /// <exception cref="InvalidOperationException">Thrown if loan is already returned</exception>
    public void Return(DateTime returnDate)
    {
        // TODO: Implement
        // 1. Check if already returned - throw InvalidOperationException if so
        // 2. Set ReturnDate to the provided date
        // 3. Validate returnDate is not before BorrowDate

        if (IsReturned == true)
            throw new InvalidOperationException("Book has already been returned.");

        if (returnDate < BorrowDate)
                throw new ArgumentException("Date returned must be after date borrowed.");

        ReturnDate = returnDate;
        Book.Return();
        
    }


    /// <summary>
    /// Gets the number of days this loan is overdue.
    /// </summary>
    /// <returns>Number of days overdue, or 0 if not overdue</returns>
    public int GetDaysOverdue()
    {
        // TODO: Implement
        // Return 0 if not overdue or already returned
        // Otherwise return (DateTime.Now - DueDate).Days

        if (IsReturned == true)
            return 0;
        if (DateTime.Now.Date <= DueDate.Date)

            return 0;

        return (DateTime.Now - DueDate).Days;

    }
}