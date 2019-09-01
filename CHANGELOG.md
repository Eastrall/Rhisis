# CHANGELOG

This is the changelog of the Rhisis project. All notable changes to this project will be documented in this file.

## [Unreleased]

## [0.4.0](https://github.com/Eastrall/Rhisis/releases/tag/v0.4) - 2019-09-30

### Fixes

- ![login] Optimize authentication SQL request ([#259](https://github.com/Eastrall/Rhisis/pull/259))

### BREAKING CHANGES

- ![login] Refactoring of the `LoginServer` with a [`HostBuilder`](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.hosting.hostbuilder?view=aspnetcore-2.2) and add `HandlerInvoker` system. ([#258](https://github.com/Eastrall/Rhisis/pull/258))
- ![cluster] Refactoring of the `ClusterServer` with a [`HostBuilder`](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.hosting.hostbuilder?view=aspnetcore-2.2) and add `HandlerInvoker` system. ([#259](https://github.com/Eastrall/Rhisis/pull/259))
- ![world] Refactoring of the `WorldServer` with a [`HostBuilder`](https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.hosting.hostbuilder?view=aspnetcore-2.2) and add `HandlerInvoker` system. ([#263](https://github.com/Eastrall/Rhisis/pull/263))
	- Refactoring systems
	- Refactoring packet factories
	- Refactoring resource loaders

## [Released]

## [0.3.0](https://github.com/Eastrall/Rhisis/releases/tag/v0.3) - 2019-06-30

### Fixes

- ![world] Fix equipement bug for female gender ([#247](https://github.com/Eastrall/Rhisis/pull/247))
- ![world] Fix item drop and moving in inventory ([#248](https://github.com/Eastrall/Rhisis/pull/248))
- ![world] Fix player death bug ([#249](https://github.com/Eastrall/Rhisis/pull/249))
- ![tools] Fix CLI `setup` and `database update` commands ([#250](https://github.com/Eastrall/Rhisis/pull/250))

### Features

- ![world] Blinkwing item usage ([#251](https://github.com/Eastrall/Rhisis/pull/251), [#253](https://github.com/Eastrall/Rhisis/pull/253))
- ![world] Delayer system: Delay actions with a given amount of time. ([#253](https://github.com/Eastrall/Rhisis/pull/253))
- ![world] Wrapzone system ([#254](https://github.com/Eastrall/Rhisis/pull/254))

## [0.2.1](https://github.com/Eastrall/Rhisis/releases/tag/v0.2.1) - 2019-06-18

### Fixes

- ![world] Fix `NullReferenceException` when a player attacks a monster without a weapon ([#231](https://github.com/Eastrall/Rhisis/pull/231))
- ![cluster] Fix ClusterServer weapon display ([#239](https://github.com/Eastrall/Rhisis/pull/239))
- ![world] Fix Respawn system ([#242](https://github.com/Eastrall/Rhisis/pull/242))
- ![world] Fix dropped items bonuses ([#243](https://github.com/Eastrall/Rhisis/pull/243)

### Features

- ![world] Attribute system ([#237](https://github.com/Eastrall/Rhisis/pull/237)
- ![world] Food, potions and refreshers items usage ([#241](https://github.com/Eastrall/Rhisis/pull/241))

### Resources

- Add Flarine dialogs ([#233](https://github.com/Eastrall/Rhisis/pull/233))

### Changes

- Remove SQL Server support ([#232](https://github.com/Eastrall/Rhisis/pull/232))


## [0.2.0](https://github.com/Eastrall/Rhisis/releases/tag/v0.2) - 2019-05-29

### World Server

#### Fixes

- Fix real-time position calculation ([#200](https://github.com/Eastrall/Rhisis/pull/200))
- Prevent dialogs from having duplicate links id. ([#213](https://github.com/Eastrall/Rhisis/pull/213))
- Fix NPC loading ([#220](https://github.com/Eastrall/Rhisis/pull/220))

#### Features

- Pick-up drop items ([#203](https://github.com/Eastrall/Rhisis/pull/203))
- Drop items from inventory ([#203](https://github.com/Eastrall/Rhisis/pull/203))
- WSAD movements and Jump behavior ([#204](https://github.com/Eastrall/Rhisis/pull/204))
- Add multiple dialog texts on DialogLinks. Allow next button on game ([#207](https://github.com/Eastrall/Rhisis/pull/207))
- Add multiple dialog texts on introduction text. ([#215](https://github.com/Eastrall/Rhisis/pull/215))
- Death System and resurection ([#206](https://github.com/Eastrall/Rhisis/pull/206))
- Experience and Level up ([#214](https://github.com/Eastrall/Rhisis/pull/214), [#218](https://github.com/Eastrall/Rhisis/pull/218))
- Experience loss on death ([#216](https://github.com/Eastrall/Rhisis/pull/216))
- Teleport system ([#222](https://github.com/Eastrall/Rhisis/pull/222))

#### Resources

- Add Flarine dialogs ([#197](https://github.com/Eastrall/Rhisis/pull/197), [#199](https://github.com/Eastrall/Rhisis/pull/199), [#202](https://github.com/Eastrall/Rhisis/pull/202), [#209](https://github.com/Eastrall/Rhisis/pull/209), [#225](https://github.com/Eastrall/Rhisis/pull/225))

#### Changes

- `MailShippingCost` configuration is now inside the `MailConfiguration` structure.
- Review of database assembly ([#208](https://github.com/Eastrall/Rhisis/pull/208))
- CLI code refactoring ([#217](https://github.com/Eastrall/Rhisis/pull/217))

## [0.1.0](https://github.com/Eastrall/Rhisis/releases/tag/v0.1) - 2019-03-23

### Added

[Core]
- Configuration structures
- Rijndael cryptography algorithm
- Custom exceptions
- Logger
- FlyFF Network packet handler
- ISC structures and packet headers

[Database]
- Multi-DB support (MySQL and MsSQL)

[Login]
- ISC authentication process
- Client authentication process
- Send server list to connected client

[Cluster]
- Inter-Server authentication (ISC)
- Character list
- Create character
- Delete character
- 2nd password verification
- Pre join

[World]
- Inter-Server authentication (ISC)
- Entity Component System architecture
- Connect to the world
- Load resources
   - Defines & texts
   - Movers
   - Maps
   - NPC Data/Shops/Dialogs
- Spawn monsters and NPC
- Visibility System
- Mobility System
- Chat System
	- Chat commands:
		- Create item : `/ci` or `/createitem`
		- Get gold : `/getgold`
		- Teleport : `/teleport`
- Inventory System
	- Move items
	- Equip/Unequip items
	- Save inventory
- Shop System
	- Buy items
	- Sell items
- Trade System
- NPC Dialog System
- MailBox System
- Drop System
- Battle System
	- Melee Attack
		- Player VS Monster
	- Monster death
	- Monster item/gold drop
- Character customization system

[world]: https://img.shields.io/badge/-world-brightgreen.svg "world-component"
[cluster]: https://img.shields.io/badge/-cluster-brightgreen.svg "cluster-component"
[login]: https://img.shields.io/badge/-login-brightgreen.svg "login-component"
[tools]: https://img.shields.io/badge/-tools-brightgreen.svg "tools-component"
