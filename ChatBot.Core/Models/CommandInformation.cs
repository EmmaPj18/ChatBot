namespace ChatBot.Core.Models;

public record CommandInformation(string Command, string Parameter, string? Error = null);

