namespace TicketTracker.Models;

public enum UserRole
{
    Operator = 1,
    Specialist = 2,
    Executor = 3,
    Administrator = 4
}

public enum TicketStatus
{
    New = 1,
    InProgress = 2,
    Completed = 3,
    Postponed = 4,
    Closed = 5
}

public enum TicketPriority
{
    Low = 1,
    Medium = 2,
    High = 3,
    Critical = 4
}
