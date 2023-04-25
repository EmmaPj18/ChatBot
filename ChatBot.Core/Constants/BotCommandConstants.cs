namespace ChatBot.Core.Constants;

public partial class BotCommandConstants
{
    public const string STOCK_COMMAND = "/stock";
}

public partial class BotCommandConstants
{
    public const string ERROR_COMMAND_DEFAULT = "There is an error on this command";
    public const string ERROR_COMMAND_NOT_EXISTS = "command not exists.";
    public const string ERROR_INVALID_FORMAT = @"Invalid format for Command. It needs to start with '\'.";
    public const string ERROR_NULL_PARAMETER = "Parameter can not be null.";
    public const string ERROR_PARAMETER_NOT_FOUND = "Indicator '=' is missing.";
}
