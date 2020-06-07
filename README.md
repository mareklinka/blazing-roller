# blazing-roller

This project was born out of a wish to improve the experience of my players while playing socially-distant (online) Dungeons & Dragons.

I couldn't find a sufficiently useful and visually pleasing dice throwing app for my players so I decided to write one. I also decide to make this a learning opportunity by combining technologies I had very little experience with, namely Unity3D, SignalR, and Blazor WebAssembly.

## Features

* 3D visualization of dice throws
* Supports d4, d6, d8, d10, d12, and d20 and any combination of thereof, up to a maximum of 15 dice in a single throw
* Multiplayer-ready - all people within a room see all throws
* Based on WebGL - supports any reasonably modern browser
* Customization of the dice look and feel

## How it all works

The application can generally be divided into three main components:
1. Dice throwing simulation and visualization (Unity)
2. The server hosting the Blazor client, the API, and the SignalR hub
3. The Blazor WebAssembly client containing the front-end

### Unity

The Unity project contains the 3D scene and other assets required to compute and visualize the physics of throwing the dice. This part is written using Unity 2019.3.15f1 and in C# 7.3. The logic is relatively straight-forward:

1. The DiceManager game object receives a message (via JS interop) describing the parameters of the throw to simulate
2. These parameters describe how many of which dice to use and the initial random seed
3. The dice are thrown according to random parameters derived from the random seed
4. When the system stabilizes, the final values and positions of all dice are sent out using JS interop

### Blazor

The Blazor client renders the application UI. There is very little complexity in here, the most complicated piece being the JS interop with the Unity WebGL layer. The client needs to be able to both invoke objects within the Unity scene AND to receive information from the scene. This is achieved via JS interop (Blazor -> JS -> Unity and Unity -> JS -> Blazor).

The client also connects to a SignalR hub to be able to send and receive throw information to/from other players in the same room.

### Server

The server is written in ASP.NET Core 3.1. It serves as the SignalR hub and hosts a few APIs required for the client (namely, room creation, login, and persistance). Access to rooms is password-protected. Rooms are persisted in an SQL Server database and their activity is tracked. Once a room is inactive for 12 hours, it can be taken over by a new player. This prevents the situation when a room cannot be used by one set of players because another set of players used the same room name six months ago

## The live version

The current version of the application is being hosted on Azure: https://blazing-roller.azurewebsites.net/