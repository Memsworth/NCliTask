using MassTransit;
using Message.Contract;
using Microsoft.AspNetCore.SignalR;

public class EmployeeConsumer(EmployeeRepo employeeRepo, IHubContext<MessageHub> _hubContext) : IConsumer<Employee>
{
    public async Task Consume(ConsumeContext<Employee> context)
    {
        var massageData = context.Message;

        if (massageData is not null)
        {
            await employeeRepo.AddAsync(massageData);

            await _hubContext.Clients.All.SendAsync("EmployeeData", new
            {
                context.MessageId,
                massageData.Id,
                massageData.Age,
                massageData.Department,
                massageData.Name
            });
        }
    }
}