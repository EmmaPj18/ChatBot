namespace ChatBot.Bot.Models;

public record StockData(string Symbol,
    string Date,
    string Time,
    string Open,
    string High,
    string Low,
    string Close,
    string Volume);
