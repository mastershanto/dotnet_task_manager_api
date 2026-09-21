using BuildingBlocks.CQRS;

namespace Tasks.Application.Features.Tasks.Commands.DeleteTask;

/// <summary>
/// টাস্ক মুছে ফেলার CQRS Command:
/// </summary>
public record DeleteTaskCommand(Guid Id) : ICommand;
