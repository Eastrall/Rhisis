<h1 align="center">
  Rhisis - Fly For Fun V15 Emulator
  <br>
</h1>

<p align="center">
  <a href="http://forthebadge.com"><img src="http://forthebadge.com/images/badges/made-with-c-sharp.svg" alt="Made With C#"></a>
  <a href="http://forthebadge.com"><img src="http://forthebadge.com/images/badges/built-with-love.svg"></a><br>
</p>

<h4>Built with C# 9 and the <a href="https://dotnet.microsoft.com/download/dotnet-core" target="_blank">.NET 5</a>.</h4>

<p>This project has been created for learning purposes about the network and game logic problematics on the server-side.<br>
We choose to use the <a href="https://github.com/Eastrall/Sylver.Network">Sylver.Network</a> to manage our server connecitions because it provides a clients management system and also a robust packet management system entirely customisable.</p>

<h4 align="center">:warning: This project is not affiliated with Gala Lab. :warning:</h4><br>

<p align="center">
  <a href="https://www.codacy.com/app/Eastrall/Rhisis?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=Eastrall/Rhisis&amp;utm_campaign=Badge_Grade"><img src="https://api.codacy.com/project/badge/Grade/500148ec8bdd4f2e954f11c682c39f3c"></a>
  <a href="https://codecov.io/gh/Eastrall/Rhisis"><img src="https://codecov.io/gh/Eastrall/Rhisis/branch/develop/graph/badge.svg" /></a>
  <a href="https://github.com/Eastrall/Rhisis/commits/develop"><img src="https://img.shields.io/github/last-commit/Eastrall/Rhisis.svg?style=flat-square&logo=github&logoColor=white" alt="GitHub last commit"></a>
  <a href="https://discord.gg/z8K22p8"><img src="https://discordapp.com/api/guilds/294405146300121088/widget.png"></a>
</p>
	    
<p align="center">
  <a href="#technical-information">Technical information</a> •
  <a href="https://github.com/Eastrall/Rhisis/blob/develop/Features.md">Features</a> •
  <a href="https://github.com/Eastrall/Rhisis/tree/develop/docs/howtos">How To's</a> •
  <a href="#contributing">Contributing</a> •
  <a href="#contributors">Contributors</a> •
  <a href="#supporters">Supporters</a> •
  <a href="https://github.com/Eastrall/Rhisis/blob/develop/LICENSE">License</a>
</p>

<p align="center"><img src="https://i.imgur.com/wpfB1VZ.gif"></p>

## Build Status

| Project 	| Status 	|
|---------	|--------	|
| `Login Server` | [![Build Status](https://dev.azure.com/eastrall/Rhisis/_apis/build/status/Login%20Server?branchName=integration%2Fazuredevops)](https://dev.azure.com/eastrall/Rhisis/_build/latest?definitionId=7&branchName=integration%2Fazuredevops) |
| `Cluster Server` | [![Build Status](https://dev.azure.com/eastrall/Rhisis/_apis/build/status/Cluster%20Server?branchName=integration%2Fazuredevops)](https://dev.azure.com/eastrall/Rhisis/_build/latest?definitionId=8&branchName=integration%2Fazuredevops) |
| `World Server` | [![Build Status](https://dev.azure.com/eastrall/Rhisis/_apis/build/status/World%20Server?branchName=integration%2Fazuredevops)](https://dev.azure.com/eastrall/Rhisis/_build/latest?definitionId=6&branchName=integration%2Fazuredevops) |

## Technical information

- Language: `C#` 9 (latest)
- Framework: `.NET 5`
- Application type: `Console` with `Docker` support
- Database type: `MySQL`
- Configuration files type: `JSON`
- Environment: `Visual Studio 2019`
- External libraries used:
	- [Sylver.Network][sylvernetwork]
	- [Sylver.HandlerInvoker](https://github.com/Eastrall/Sylver.HandlerInvoker)
	- [Entity Framework Core](https://github.com/aspnet/EntityFrameworkCore)
	- [Newtonsoft.Json](https://github.com/JamesNK/Newtonsoft.Json)
	- [NLog](https://github.com/NLog/NLog)

## Getting started

Please take a look at our [contributing](https://github.com/Eastrall/Rhisis/blob/develop/CONTRIBUTING.md) guidelines, if you're interested in helping!

Before getting started, you will need to install the following softwares:

- Visual Studio 2019
- Docker, Docker-Compose, Docker Desktop (if running on Windows)
- Any MySQL database explorer (MySQL Workbench for example)

> The solution is configured to run with Linux containers.

Once you have checked out the repository source code, go to the root directory of the repository and type the following command in a `PowerShell` command prompt and follow the instruction to setup your development environment:

```sh
$> ./setup/setup-environment.ps1
```

> Note: By the MySQL container listen to port 3306 internally, but exposes the port 3307 if you ever need to connect to the MySQL server using a tool like MySQL workbench.
> Note2: The database files are located in the `bin/_database` folder.

### Configure the database access

In the same command prompt you used to setup your development environment, navigate to the `bin/` directory of your repository and type the following commands:

```sh
$> ./rhisis-cli database configure
```

This command will configure the different servers to access the database. Since the servers will be running inside docker containers, please configure your environnement as followed:

```
Database server host address: rhisis.database
```
> The `rhisis.database` is the name of the Docker container where the MySQL Server is running. In order for other containers to access this container, you need to specify the container name as the host.

```
Database server listening port: 3306
```
> Even if the docker container has an acces with the `3307` port, you should use the port `3306` internally so other containers can access the MySQL Server

```
Database username:
Database user password:
Database name:
```
> Type the **same** information you entered during the setup.

For the rest of the options, you can choose to use encryption or not. It's up to you now.

### Configure the servers

You can either use the provided configuration and modify it has you want or use the CLI to configure it in your own.

#### Login Server

```sh
$> ./rhisis-cli configure login
```

#### Cluster Server

```sh
$> ./rhisis-cli configure cluster
----- Core Server -----
Core server host address: rhisis.login
```

> Note: You will need to specify the `rhisis.login` container name has the core server host in order for the cluster server to communicate with the CoreServer.
> Also: The port and passwords should match on both sides.

#### World Server

```sh
$> ./rhisis-cli configure world
...
----- World cluster Server -----
World cluster server host address: rhisis.cluster
```

> Note: You will need to specify the `rhisis.cluster` container name, so the world server can be considered as a "channel" of the given cluster.
> Also: The port and passwords should match on both sides.

## Contributors

- [Eastrall](https://github.com/Eastrall)
- [Steve-Nzr](https://github.com/Steve-Nzr)
- [Freezeraid](https://github.com/Freezeraid)
- [Skeatwin](https://github.com/Skeatwin)
- [johmarjac](https://github.com/johmarjac)
- [Kaev](https://github.com/Kaev)
- [YarinNet](https://github.com/YarinNet)
- [Almewty](https://github.com/Almewty)
- [Anjuts](https://github.com/Anjuts)
- [MarkWilds](https://github.com/MarkWilds)
- [tekinomikata](https://github.com/tekinomikata)

## Supporters

- Ukiyo
- Kinami
- Sauce

[sylvernetwork]: https://github.com/Eastrall/Sylver.Network
