using Message.Contract;

public class EmployeeRepo(AppDbContext context)
{
    public async Task AddAsync(Employee data)
    {
        await context.Employees.AddAsync(data);
        await context.SaveChangesAsync();
    }
}