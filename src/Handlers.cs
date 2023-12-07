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

namespace Handlers {

public class Handlers {
    // HTTP functionality
    // see: https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient?view=net-7.0
    static readonly System.Net.Http.HttpClient httpClient = new HttpClient();
    static Random rng = new Random();
    public async Task SlashCommandHandler(Discord.WebSocket.SocketSlashCommand command){
        // ERR! Can't do this twice it seems
        // A SlashCommandExecuted handler has thrown an unhandled exception.:
        // System.InvalidOperationException: Cannot respond twice to the same interaction
        // await command.RespondAsync($"You executed {command.Data.Name}");
        
        // https://discordnet.dev/api/Discord.IApplicationCommandInteractionData.html
        switch(command.Data.Name){
            case "getpokemon":
                await GetPokemonCommandHandler(command);
                break;
            case "globalcommand1":
                await GlobalCommand1Handler(command);
                break;
            case "guildgetpikachu":
                command.RespondAsync("SRY deprecated");
                break;
            case "globalcommand2":
                break;
            default:
                command.RespondAsync("Unrecognized command");
                break;
        }
    }



    // Pikachu Handler
    public async Task GetPokemonCommandHandler(Discord.WebSocket.SocketSlashCommand command) {
        
        // Get pokemon option from command data
        // Options = IReadOnlyCollection<TOption> - Returns a filtered collection of elements that contains the descendant elements of every element and document in the source collection.
        var commandOptions = command.Data.Options;
        var responseString = "";
        string chosenPokemon = "pikachu";
        string pokemonName = "";
        string spriteUrl = "";
        var embedBuilder = new EmbedBuilder();
        Pokemon pokemon = new Pokemon();

        int pokeID = rng.Next(1,641);
        chosenPokemon = pokeID.ToString();
        
        // Get command parameter information
        // iteration: https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/iterators
        foreach (SocketSlashCommandDataOption option in commandOptions) {
            // Command Options: 
            Console.WriteLine($"option_Name = {option.Name}");
            Console.WriteLine($"option_Type = {option.Type}");
            Console.WriteLine($"option_Value = {option.Value}, Type={option.Value.GetType()} ");
            // Set pokemon if given choose a random one if not
            if (option.Name == "pokemon" && option.Value != ""){
                chosenPokemon = (string)option.Value;
                chosenPokemon.ToLower();
            } // choose RNG
            else {

            }
        }
        // fetch pokeapi url and get data, i.e. send GET request to URL
        string pokemonUrl = $"https://pokeapi.co/api/v2/pokemon/{chosenPokemon}";
        string pokemonSpeciesUrl = $"https://pokeapi.co/api/v2/pokemon-species/{chosenPokemon}";
        string cacheDir = "cache/";
        string pokemonCacheFile = cacheDir + $"{chosenPokemon}.json";
        string pokemonSpeciesCacheFile = cacheDir + $"{chosenPokemon}-species.json";
        //


        
        try {
            // Fetch if file does not exist
            if (File.Exists(pokemonCacheFile) == false )
            {
                
                //await command.RespondAsync($"{cacheFile} not found. Sending fetch request");
                using HttpResponseMessage response = await httpClient.GetAsync(pokemonUrl);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                // Above three lines can be replaced with new helper method below
                // string responseBody = await client.GetStringAsync(uri);
                

                Console.WriteLine($"Response from {pokemonUrl}");
                Console.WriteLine(responseBody);
                // Figure out how to cache an http response
                // Just write to a file for now
                
                // Create directory
                if (System.IO.Directory.Exists(cacheDir) == false){
                    System.IO.Directory.CreateDirectory(cacheDir);
                }

                // Create pokemon file
                using (System.IO.FileStream fs = File.Create(pokemonCacheFile)){
                    Console.WriteLine($"File created: {pokemonCacheFile}");
                }

                

                // the using statement ensures that a disposable instance is disposed even if an exception occurs within the block of the using
                // its used with objects with IDisposable
                // using basically ensures your program memory stays clean
                using (System.IO.StreamWriter streamWriter = new System.IO.StreamWriter(pokemonCacheFile)){
                    Console.WriteLine($"Writing to file {pokemonCacheFile}");
                    await streamWriter.WriteAsync(responseBody);
                    // deserialize json -> obj?
                    // see: https://stackoverflow.com/questions/6620165/how-can-i-deserialize-json-with-c
                    // see: https://learn.microsoft.com/en-us/dotnet/csharp/advanced-topics/interop/using-type-dynamic
                    // https://pokeapi.co/docs/v2#pokemon
             
                }
                if (responseBody != null){
                    dynamic pkmnObject = Newtonsoft.Json.JsonConvert.DeserializeObject(responseBody);
                    Console.WriteLine($"pokemon_name={pkmnObject.name}");
                    pokemonName = pkmnObject.name;
                    responseString = $@"
Caching Pokemon for faster responses.
OK Pokemon cached success.
                    ";
                        responseString += $"\n{pkmnObject.sprites["front_default"]}";
                    }       

                // await command.RespondAsync(responseString);
            }

            if (File.Exists(pokemonSpeciesCacheFile) == false)
            {
                // Fetch pokeapi
                string speciesResponseBody = await httpClient.GetStringAsync(pokemonSpeciesUrl);
                // Create pokemon species file (we need this for pokedex entry)
                using (System.IO.FileStream fs = File.Create(pokemonSpeciesCacheFile)){
                    Console.WriteLine($"File created: {pokemonSpeciesCacheFile}");
                }
                // Write to file
                using (System.IO.StreamWriter streamWriter = new System.IO.StreamWriter(pokemonSpeciesCacheFile)) {
                    await streamWriter.WriteAsync(speciesResponseBody);
                }
            }

            if (File.Exists(pokemonCacheFile) == true && File.Exists(pokemonSpeciesCacheFile) == true) 
            {
                
                Console.WriteLine($"cacheFile={pokemonCacheFile}");
                // Read from file and deserialize
                var jsonString = File.ReadAllText(pokemonCacheFile);
                // Newtonsoft.Json.Linq.JObject potentialJSON = JObject.Parse(jsonString);
                if (Json.IsValidJson(jsonString)) {
                    dynamic pkmnObject = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonString);
                    Console.WriteLine($"pokemon_name={pkmnObject.name}");
                    spriteUrl = pkmnObject.sprites["front_default"];
                    responseString = $@"
Nice! Response has already been cached.
pokemon_name={pkmnObject.name}
pokemon_spriteURL={spriteUrl}
== STATS ==
hp: {pkmnObject.stats[0].base_stat}
atk: {pkmnObject.stats[1].base_stat}
def: {pkmnObject.stats[2].base_stat}
sp-atk: {pkmnObject.stats[3].base_stat}
sp-def: {pkmnObject.stats[4].base_stat}
spd: {pkmnObject.stats[5].base_stat}
                    ";
                    
                    // Analyze types
                    List<string> typeList = new List<string>();
                    if (pkmnObject.types.Count == 1) {
                        string type1 = pkmnObject.types[0].type.name;
                        responseString += "\nTYPE: " + pkmnObject.types[0].type.name;
                        typeList.Add(type1);
                        
                    }
                    if (pkmnObject.types.Count == 2) {
                        string type1 = pkmnObject.types[0].type.name;
                        string type2 = pkmnObject.types[1].type.name;
                        responseString += "\nTYPE: " + pkmnObject.types[0].type.name;
                        responseString += " / " + pkmnObject.types[1].type.name;
                        typeList.Add(type1);
                        typeList.Add(type2);
                        
                    }
                    pokemon.SetTypes(typeList);

                    int count = 1;
                    foreach( var type in typeList) {
                        embedBuilder.AddField($"type{count}", type);
                        count++;
                    }
                    // End Types

                    // Calculate Weaknesses
                    double damageX = 1;
                    uint numWeak = 0;
                    uint numSuperWeak = 0;
                    List<string> weakList = new List<string>();
                    List<string> superWeakList = new List<string>();
                    // see: https://stackoverflow.com/questions/141088/how-to-iterate-over-a-dictionary
                    // Calculate  weaknesses for each attacking type
                    foreach (var type in PType.pTypes.Keys) {
                        damageX = PType.calculateDamageX(type, pokemon);
                        if (damageX == 2) {
                            numWeak += 1;
                            weakList.Add(type);
                            
                        }
                        if (damageX == 4) {
                            numWeak += 1;
                            superWeakList.Add(type);
                            
                        }
                    }
                    var weakString = "";
                    var superWeakString = "";

                    if (weakList.Count > 0){
                        responseString += $"\nWeakness (2x): ";
                        foreach (var weak in weakList) {
                            responseString += $" {weak},";
                            weakString += $" {weak},";
                        }
                        embedBuilder.AddField("Weakness (2x):", weakString);
                    }
                    if (superWeakList.Count > 0){
                        responseString += $"\nWARN! Super Weak (4x): ";
                        foreach (var superWeak in superWeakList) {
                            responseString += $" {superWeak},";
                            superWeakString += $" {superWeak}";
                        }
                        embedBuilder.AddField("Super Weakness (4x):", superWeakString);
                    }
                    if ( (numWeak + numSuperWeak) == 0 ){
                        responseString += "\nWOW! This pokemon is weak to nothing!";
                    }

                    // Build embed
                    // Read pokemon species cache file
                    var speciesJson = File.ReadAllText(pokemonSpeciesCacheFile);
                    dynamic pkmnSpeciesObject = Newtonsoft.Json.JsonConvert.DeserializeObject(speciesJson);
                    string pokemonDesc = pkmnSpeciesObject["flavor_text_entries"][0]["flavor_text"];
                    Console.WriteLine($"pokemonName={pokemonName}");
                    pokemonName = (string)pkmnObject.name;
                    pokemonName = char.ToUpper(pokemonName[0]) + pokemonName.Substring(1);
                    embedBuilder
                        .WithTitle(pokemonName)
                        .WithDescription(pokemonDesc)
                        .WithImageUrl(spriteUrl)
                        .WithColor(PType.pTypeDiscordColors[typeList[0]]);
                    ;
                    // 

                    await command.RespondAsync(embed: embedBuilder.Build());
                    //await command.RespondAsync(responseString);
                } else {
                    await command.RespondAsync($"ERR! Invalid JSON");
                }


            }
        } catch (HttpRequestException error){
            Console.WriteLine($"ERR! {error.Message}");
        }
    }

    public async Task GlobalCommand1Handler(Discord.WebSocket.SocketSlashCommand command) {
        await command.RespondAsync($"You executed {command.Data.Name}");
    }




    public async Task GlobalSlashCommandHandler(Discord.WebSocket.SocketSlashCommand command){
        await command.RespondAsync($"Global command executed.");
        // Global
        switch(command.Data.Name) {
            case "globalcommmand1":
                break;
        }
    }

    public async Task ListRoleCommandHandler(Discord.WebSocket.SocketSlashCommand command) {
        await command.RespondAsync($"You executed command with snowflake_id={command.Data.Id}");
    }
}

}

public static class Json {
    public static bool IsValidJson(string strInput)
    {
        if (string.IsNullOrWhiteSpace(strInput)) { return false;}
        strInput = strInput.Trim();
        if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
            (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
        {
            try
            {
                var obj = JToken.Parse(strInput);
                return true;
            }
            catch (Newtonsoft.Json.JsonReaderException jex)
            {
                //Exception in parsing json
                Console.WriteLine(jex.Message);
                return false;
            }
            catch (Exception ex) //some other exception
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }
        else
        {
            return false;
        }
    }
}