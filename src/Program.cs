using System;
using System.IO;
using System.Text;

using Discord.Commands;

// Discord.Net

// using System.Net
                   // WebRequest
using System.Net.Http;

using Newtonsoft.Json.Linq;
using Discord;
using Discord.WebSocket;
using System.Diagnostics;

using Handlers;

public static class MyExtensions {
    public static Discord.SlashCommandBuilder CreateCommandWithName(this Discord.SlashCommandBuilder builder, string name){
        return builder.WithName(name);
    }

    
    public static Discord.SlashCommandBuilder CreateCommandWithDescription(this Discord.SlashCommandBuilder builder, string desc){
        return builder.WithDescription(desc);
    }
}

// See https://aka.ms/new-console-template for more information
public class Program
{

    // https://discordnet.dev/api/Discord.WebSocket.DiscordSocketClient.html
	// _client publishes an event like Ready
    private Discord.WebSocket.DiscordSocketClient _client;
    
    // The Task class represents a single operation that does not return a value and that usually executes asynchronously.
    // https://learn.microsoft.com/en-us/dotnet/api/system.threading.tasks.task?view=net-7.0
    
    // https://discordnet.dev/api/Discord.LogMessage.html
    private Task Log(Discord.LogMessage msg)
    {
	    Console.WriteLine(msg.ToString());
	    return Task.CompletedTask;
    }

    private Task LogBotReady()
    {
        Console.WriteLine("Bot ready");
        return Task.CompletedTask;
    }

    public async Task ClientReady() {
        // This is only one guild, how do you get guild id on invite
        
        // Read guild id from file
        string guildIDFile = "secure/guildID.txt";
        ulong guildID = ulong.Parse(File.ReadAllText(guildIDFile));
        Console.WriteLine($"guildID={guildID}");
        var guild = _client.GetGuild(guildID);
        //

        // You can have 100 global + 100 guild slash commands

        // TIP: Use guild commands for Testing

        // GUILD COMMANDS
        // slash-command (guild): guildcommand1
        var guildCommand = new Discord.SlashCommandBuilder();
        // Note: Names have to be all lowercase and match the regular expression ^[\w-]{3,32}$
        // https://regex101.com/
        guildCommand.WithName("guildcommand1");
        // Descriptions can have max length of 100
        guildCommand.WithDescription("This is first guild slash command");

        // create extension method to make this more readable
        guildCommand.WithName("guildgetpikachu");
        guildCommand.CreateCommandWithDescription("get pikachu from pokeapi");

        // GET Pokemon from pokeapi
        guildCommand.WithName("getpokemon");
        guildCommand.WithDescription("Get a pokemon from the pokeapi");
        // see: addoption  - https://discordnet.dev/api/Discord.SlashCommandBuilder.html
        // see: optiontype - https://discordnet.dev/api/Discord.ApplicationCommandOptionType.html
        // default: you'll just get pikachu
        guildCommand.AddOption("pokemon", ApplicationCommandOptionType.String, "the pokemon you want to get(default=pikachu)", isRequired: false);
        
        
        // GLOBAL COMMANDS
        // slash-command (global): globalcommand1
        var globalCommand = new Discord.SlashCommandBuilder();
        globalCommand.WithName("globalcommand1");
        globalCommand.WithDescription("This is first global slash command");

        // getpikachu command
        globalCommand.WithName("globalgetpikachu");
        globalCommand.WithDescription("Get info on pokemon: pikachu");

        try {
            await guild.CreateApplicationCommandAsync(guildCommand.Build());
            // With global commands we don't need the guild
            await _client.CreateGlobalApplicationCommandAsync(globalCommand.Build());
            // Using the ready event is simple and suitable for testing
            // For production bot, recommend to only run CreateGlobalApplicationCommandAsync() once for each command
        } catch( Discord.Net.ApplicationCommandException exception ) {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(exception.Errors, Newtonsoft.Json.Formatting.Indented);
            Console.WriteLine(json);
        }
    }


    // Main / Entry Point
	public async Task MainAsync()
	{
        
        PType.printTestType();
        
        Handlers.Handlers handlers = new Handlers.Handlers();
        
        // There are many events you can listen for
        // see: https://discordnet.dev/api/Discord.WebSocket.DiscordSocketClient.html
        _client = new Discord.WebSocket.DiscordSocketClient();
        // https://discordnet.dev/api/Discord.Rest.BaseDiscordClient.html#Discord_Rest_BaseDiscordClient_Log
        // Log is an event, you attach a Task to event ( event += Task)
        _client.Log += Log;
        // Listen for when client is ready
        _client.Ready += ClientReady;
        _client.Ready += LogBotReady;
        // Listen for a event where slash command is executed
        _client.SlashCommandExecuted += handlers.SlashCommandHandler;


        // read token from file, don't keep in variable for security reasons
        string tokenFile = "secure/bot_token.txt";
        string token = File.ReadAllText(tokenFile);
        Console.WriteLine(token);

        await _client.LoginAsync(Discord.TokenType.Bot, token);
        await _client.StartAsync();

        // Block this task until the program is closed
        await Task.Delay(-1);
	}

    public static Task Main(string[] args) => new Program().MainAsync();
}