wiki

[Discord.Net](https://discordnet.dev/) wiki and notes

## Discord interactions

Discord [Interaction](https://discord.com/developers/docs/interactions/receiving-and-responding) is the message that your application receives when a user uses an application command or a message component.

[Working with the interaction framework](https://discordnet.dev/guides/int_framework/intro.html)

Concept:
- [Discord.Interaction](https://discord.com/developers/docs/interactions/receiving-and-responding)
- [C#.attribute](https://learn.microsoft.com/en-us/dotnet/csharp/advanced-topics/reflection-and-attributes/)
- [programming.reflection](https://stackoverflow.com/questions/37628/what-is-reflection-and-why-is-it-useful)

The Interaction service provides [attribute](https://learn.microsoft.com/en-us/dotnet/csharp/advanced-topics/reflection-and-attributes/) based framework for creating [Discord Interaction](https://discord.com/developers/docs/interactions/receiving-and-responding) handlers
- In C# attributes are classes that inherit from the Attribute base class. Any class that inherits Attribute can be used as a sort of "tag" on other pieces of code.
- Use attributes to associate metadata or declarative information with code in C#
- An **attribute** can be queried at run-time by using [reflection](https://stackoverflow.com/questions/37628/what-is-reflection-and-why-is-it-useful)
- reflection is code that can introspect itself which allows you to manipulate it at runtime giving your code more power and flexibility at the cost of performance 
- reflection gives you the ability to write more generic code
- it allows you to create objects at  runtime and call their methods at runtime
- reflection allows you to retrieve metadata on types at runtime
- reflection allows you to dynamically create instance of types (create instances at run-time)



What is [reflection?](https://stackoverflow.com/questions/37628/what-is-reflection-and-why-is-it-useful)
- reflection is code that can introspect itself which allows you to manipulate it at runtime giving your code more power and flexibility at the cost of performance 
- reflection gives you the ability to write more generic code
- it allows you to create objects at  runtime and call their methods at runtime
- reflection allows you to retrieve metadata on types at runtime
- reflection allows you to dynamically create instance of types (create instances at run-time)

Examples

In C# reflection can be used to read Attribute metadata and use that information to further manipulate the run-time code [see](https://learn.microsoft.com/en-us/dotnet/csharp/advanced-topics/reflection-and-attributes/)
- In C# attributes are classes that inherit from the Attribute base class. Any class that inherits Attribute can be used as a sort of "tag" on other pieces of code.
- Use attributes to associate metadata or declarative information with code in C#
- An **attribute** can be queried at run-time by using [reflection](https://stackoverflow.com/questions/37628/what-is-reflection-and-why-is-it-useful)