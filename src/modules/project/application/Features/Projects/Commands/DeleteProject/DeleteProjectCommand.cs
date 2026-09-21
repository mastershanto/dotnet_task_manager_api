using BuildingBlocks.CQRS;

namespace Projects.Application.Features.Projects.Commands.DeleteProject;

/// <summary>
/// প্রজেক্ট ডিলিট করার CQRS Command:
/// </summary>
public record DeleteProjectCommand(Guid Id) : ICommand;
