
# Work in progress

This library is work in progress, currently only some parts are finished, see issue #19 to track the progress.
This README may contain some information that aren't done yet.
I've tried to make these parts crossed out. I've included them so it's
clear what this library is for.

# NosSmooth
A multi-platform library with an interface for NosTale packets, data, game state and such.
Can be used for local (injected) ~~as well as remote (clientless)~~ bots.
The library is splitted into multiple assemblies to allow for
using only specific features. Bots that can be both locally
and remotely can be implemented that way using the same code base.

For local-specific libraries see [NosSmooth.Local](https://github.com/Rutherther/NosSmooth.Local).

## How to use
All of the packages are on NuGet. Search for NosSmooth.
There is not a full documentation, but there are some samples and the
library methods and objects are documented.
You can find the samples in `Samples` folder in this repository,
[NosSmooth.Local](https://github.com/Rutherther/NosSmooth.Local) repository ~~and
[NosSmooth.Remote](https://github.com/Rutherther/NosSmooth.Remote) repository.~~

## Projects that use NosSmooth
- [Rutherther/NosSmooth.Local](https://github.com/Rutherther/NosSmooth.Local) A set of libraries used for injecting into a running NosTale instance, hooking some functions and reading memory
- [Rutherther/NosSmooth.Comms](https://github.com/Rutherther/NosSmooth.Comms) A set of libraries used for communicating with a NosTale project using named pipes or tcp. Uses NosSmooth.Local
- [Rutherther/NosTale-PacketLogger](https://github.com/Rutherther/NosTale-PacketLogger) A standalone packet logger that may open and filter packet files or connect to a running NosTale instance using named pipes
- [Rutherther/NosTale-Anonymizer](https://github.com/Rutherther/NosTale-Anonymizer) A CLI used for changing names and ids inside of logged packets to make the contents anonymous, untraceable, but keep them consistent


## Features

### Data
NosTale data can be retrieved either using .NOS files or a sqlite3 database.
The database can be created by starting `NosSmooth.Data.CLI`
with arguments `migrate {NostaleDataDirectoryPath}`.
Currently only translations and data about skills, items and monsters are stored.
The migration will migrate all languages NosTale supports.

See the sample `DataBrowser` for more details about the usage.

### Packets
NosTale packets are located inside of `NosSmooth.Packets`.
Serializers and deserializers are generated using source generators.
The source generator is located in `NosSmooth.PacketSerializersGenerator`.

Serializing the packets may be done using `NosSmooth.PacketSerializer`.

### Core (Low level)
The core contains abstractions for the NosTale client, packets and commands.
You can register your packet responders (IPacketResponder)
that will be called by the client when there is a new packet of the given type.

If there is an unknown packet, `UnresolvedPacket` will be created
and can be handled by the user. If there is other parsing error,
`ParsingFailedPacket` will be created and can be handled.

### Game state (High level)
The game state is built using the core, it stores useful information about the state
such as the current map, entities, information about the current character, mates, skills, family, group.
The game project also has custom events that contain more information
than packets would. In some cases, there would be no gain in information,
for these packets there is no event and a packet responder should be registered instead.
It uses NosTale data for a few features, so setting up the data provider
is required. If no provider is found, an exception will be thrown
as `ILanguageService` and `IInfoService` will not be found.

## Extensions

### Combat
Attacks entities, players using techniques that execute operations.
See `SimpleAttackTechnique` for example on how to make a technique.
Simple technique just attacks one target by walking to it and using skills
from a skill selector.

### Pathfinding
Finds paths on NosTale maps using A* algorithm.
May take (walk) the given path, using `WalkCommand` in the process.

Will support walking through more maps in the future.

## Commands
The library uses commands for features that may be implemented
differently on local and remote bots. For examples,
walking on local client is done calling the walk function on the client,
whereas on the remote client just packets have to be sent directly.

- TakeControlCommand
  - Makes sure there is just one command in the given group running at the same time
  - Useful for operations that do not support two simultaneous states such as walking.
  - For local client this can also disable the users controls or be cancelled upon user making any action
  - Everything stated here can be configured and should be clear from the documentation of the constructor members
- WalkCommand
  - Uses PlayerWalkCommand and PetWalkCommand
  - PetSelectors specify the indices of the pets the player has and should be moved to the specified position
  - This is a TakeControlCommand
  - Used for walking in straight lines
  - IMPORTANT: doesn't support obstacles, pathfinding mechanism must be used
- AttackCommand
  - This is a TakeControlCommand
  - Usually a long-running command that controls the client, walks, kills entities
  - `NosSmooth.Combat.Extensions` uses `AttackCommand`
