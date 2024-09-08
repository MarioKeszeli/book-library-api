using System;
using System.Threading;
using System.Threading.Tasks;
using BookLibrary.Domain.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace BookLibrary.FunctionApps;

public class ReminderFunction
{
    private readonly IBorrowingService _borrowingsService;
    private readonly IUserService _userService;
    private readonly IBookService _bookService;

    public ReminderFunction(IBorrowingService borrowingsService, IUserService userService, IBookService bookService)
    {
        _borrowingsService = borrowingsService;
        _userService = userService;
        _bookService = bookService;
    }

    /// <summary>
    /// The reminder function timer trigger configuration name.
    /// </summary>
    public const string ScheduleReminderFunction = "ScheduleReminderFunction";

    /// <summary>
    /// The reminder function sender email address configuration name.
    /// </summary>
    public const string SenderEmailAddressReminderFunction = "SenderEmailAddressReminderFunction";

    [FunctionName("ReminderFunction")]
    public async Task Run([TimerTrigger("%" + ReminderFunction.ScheduleReminderFunction + "%")] TimerInfo myTimer, ILogger log, CancellationToken cancellationToken)
    {
        log.LogInformation($"ReminderFunction executed at: {DateTime.Now}");

        var borrowings = await _borrowingsService.GetBorrowings();

        foreach (var borrowing in borrowings)
        {
            var user = await _userService.GetUserAsync(borrowing.UserId, cancellationToken);
            var book = await _bookService.GetBookAsync(borrowing.BookId, cancellationToken);

            var currentDate = DateTime.UtcNow;
            var returnDate = borrowing.ReturnDate;

            if (currentDate < returnDate && returnDate < currentDate.AddDays(1))
            {
                await SendEmail(
                    Environment.GetEnvironmentVariable(ReminderFunction.SenderEmailAddressReminderFunction),
                    user.Email,
                    "Reminder: Book return due tomorrow",
                    GetEmailBody(user.Name, returnDate, book.Title),
                    cancellationToken);
            }
        }
    }

    private static async Task SendEmail(string from, string to, string subject, string body, CancellationToken cancellationToken)
    {
        // TODO: email service
        return;
    }

    private static string GetEmailBody(string userName, DateTimeOffset? returnDate, string bookTitle)
    {
        return
            $"""
                Dear {userName},

                This is a reminder that your borrowed book {bookTitle} is due back tomorrow, {returnDate}.

                Please return it by the due date to avoid late fees. If you need an extension, visit your account or contact us.

                Thank you,
                BookLibrary
            """;
    }
}
