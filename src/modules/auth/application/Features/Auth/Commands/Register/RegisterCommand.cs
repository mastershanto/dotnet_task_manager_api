using Auth.Domain;
using BuildingBlocks.CQRS;

namespace Auth.Application.Features.Auth.Commands.Register;

/// <summary>
/// সাইন-আপ / রেজিস্ট্রেশন কমান্ড (CQRS Command):
/// ইউজারের নাম, ইমেইল ও পাসওয়ার্ড গ্রহণ করে।
/// </summary>
public record RegisterCommand(
    string Name,
    string Email,
    string Password
) : ICommand<RegisterResponse>;
