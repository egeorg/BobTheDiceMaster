# School

This repository contains source code for a dice game called "School". It's not a widespread game, first time I've heard about it in my wife's family circle.
In its current implementation it allows to play for a single human player with an AI. The AI is called Bob The Dice Master and possibly I'll add other dice game\[s\] to it.

It's a [SPA](https://en.wikipedia.org/wiki/Single-page_application) written in C# ([Blazor webassembly](https://learn.microsoft.com/en-us/aspnet/core/blazor/?view=aspnetcore-7.0#blazor-webassembly)), and is also has AWS Lambda backend version.
The idea is to make it entirely in browser, with possibility to play online with other users using cloud-based backend. An online variant is not implemented yet.

More details:

[Rules of the game](school-rules.md)

[Algorithm for Bob](school-bob-algorithm.md)

[The game itself](https://egeorg.github.io/BobTheDiceMaster)
