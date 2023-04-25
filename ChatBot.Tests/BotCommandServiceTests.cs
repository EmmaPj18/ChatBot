using ChatBot.Core.Constants;
using ChatBot.Core.Interface;
using ChatBot.Core.Services;
using Shouldly;

namespace ChatBot.Tests;

public class BotCommandServiceTests
{
    private readonly IBotCommandService _suspect;

    public BotCommandServiceTests()
    {
        _suspect = new BotCommandService();
    }

    [Theory]
    [InlineData("NOT COMMAND", false)]
    [InlineData("/should-be-valid-command", true)]
    public void Should_ReturnFalse_IfIsNotACommand(string command, bool expectedResult)
    {
        var isCommand = _suspect.IsCommand(command);

        isCommand.ShouldBe(expectedResult);
    }

    [Theory]
    [InlineData("command", BotCommandConstants.ERROR_INVALID_FORMAT)]
    [InlineData("command=", BotCommandConstants.ERROR_INVALID_FORMAT)]
    [InlineData("/command", BotCommandConstants.ERROR_PARAMETER_NOT_FOUND)]
    [InlineData("/command=", $"'/command' {BotCommandConstants.ERROR_COMMAND_NOT_EXISTS}")]
    [InlineData($"{BotCommandConstants.STOCK_COMMAND}=", BotCommandConstants.ERROR_NULL_PARAMETER)]
    public void Should_ReturnErrorMessage_WhenValidatingInvalidCommand(string command, string expectedMessage)
    {
        var errorMessage = _suspect.ValidateCommand(command);

        errorMessage.ShouldNotBeNull();
        errorMessage.ShouldBe(expectedMessage);
    }

    [Fact]
    public void Should_ReturnNull_WhenCommandIsValid()
    {
        string command = "/stock=doge.c";

        var errorMessage = _suspect.ValidateCommand(command);

        errorMessage.ShouldBeNull();
    }

    [Fact]
    public void Should_ReturnValidCommandInformation_WhenCommandIsValid()
    {
        var parameter = "doge.c";
        var command = $"{BotCommandConstants.STOCK_COMMAND}={parameter}";

        var commandInfo = _suspect.GetCommandInformation(command);

        commandInfo.ShouldNotBeNull();
        commandInfo.Command.ShouldBe(BotCommandConstants.STOCK_COMMAND);
        commandInfo.Parameter.ShouldBe(parameter);
    }

    [Fact]
    public void Should_ReturnNull_WhenCommandIsInvalid()
    {
        var command = $"invalid-command";

        var commandInfo = _suspect.GetCommandInformation(command);

        commandInfo.ShouldNotBeNull();
        commandInfo.Error.ShouldNotBeNullOrWhiteSpace();
    }
}