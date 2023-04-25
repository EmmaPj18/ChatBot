using ChatBot.Core.Constants;
using ChatBot.Core.Interface;
using ChatBot.Core.Models;

namespace ChatBot.Core.Services;

public sealed partial class BotCommandService : IBotCommandService
{
    public readonly IReadOnlyList<string> _availableCommands =
        new List<string> {
            BotCommandConstants.STOCK_COMMAND
        };

    public CommandInformation GetCommandInformation(string text)
    {
        var error = ValidateCommand(text);

        if (!string.IsNullOrWhiteSpace(error))
        {
            var errorMessage = new CommandInformation(string.Empty, string.Empty, error);

            return errorMessage;
        }

        var commandInfo = ConvertToCommandInformation(text);

        commandInfo ??= new CommandInformation(string.Empty, string.Empty, BotCommandConstants.ERROR_COMMAND_DEFAULT);

        return commandInfo;
    }

    public string? ValidateCommand(string text)
    {
        var commandInfo = ConvertToCommandInformation(text);

        if (commandInfo == null)
        {
            if (!text.StartsWith("/"))
                return BotCommandConstants.ERROR_INVALID_FORMAT;
            

            if (!text.Contains('='))
                return BotCommandConstants.ERROR_PARAMETER_NOT_FOUND;

            return BotCommandConstants.ERROR_COMMAND_DEFAULT;
        }

        if (!_availableCommands.Contains(commandInfo.Command))
            return $"'{commandInfo.Command}' {BotCommandConstants.ERROR_COMMAND_NOT_EXISTS}";

        return string.IsNullOrWhiteSpace(commandInfo.Parameter)
            ? BotCommandConstants.ERROR_NULL_PARAMETER
            : null;
    }

    public bool IsCommand(string text) => text.StartsWith("/", StringComparison.InvariantCultureIgnoreCase);

    private CommandInformation? ConvertToCommandInformation(string text)
    {
        if (!text.StartsWith("/"))
            return null;

        if (!text.Contains('='))
            return null;

        var splitter = text.Split("=");
        var (command, parameter) = (splitter[0].Trim(), splitter[1].Trim());

        var commandInfo = new CommandInformation(command, parameter);

        return commandInfo;
    }
}
