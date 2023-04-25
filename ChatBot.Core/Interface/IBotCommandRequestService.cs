using ChatBot.Core.Models;

namespace ChatBot.Core.Interface;

public interface IBotCommandRequestService
{
    Task ExecuteCommand(CommandInformation command, CancellationToken ct = default);
}
