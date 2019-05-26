```
The wind howled by me
As I sat and watched the world;
All but powerless
```

## CantiLib
[![Build Status](https://travis-ci.com/BrandonT42/CantiLib.svg?branch=master)](https://travis-ci.com/BrandonT42/CantiLib)
[![Build status](https://ci.appveyor.com/api/projects/status/o7vekwk9x3t7h2mu?svg=true)](https://ci.appveyor.com/project/BrandonT42/cantilib)

### Prerequisites

You need the `dotnet` command line tools.

#### Windows

Install the dotnet sdk from here - https://www.microsoft.com/net/download

Once this is installed, you should have access to the `dotnet` executable from a standard cmd.

Alternatively you can attempt installing Visual Studio to install dotnet, however I found that it installed the wrong version for me.

Once you have the correct dotnet sdk installed (Verify by running `dotnet build` or another of the below commands via CLI) you can interact with the project via Visual Studio, if you like.

To do so, open up the CantiLib.sln file in Visual Studio. To run the daemon, just hit the green arrow labeled Daemon. To run the tests, choose the Test menu, then Run, then All Tests.

#### Ubuntu, Debian, Fedora, CentOS, RHEL, openSUSE

https://www.microsoft.com/net/download/linux-package-manager/ubuntu18-04/sdk-current

(There is a drop down box to choose the distro you are using)

#### Arch Linux

Yep, it's a special snowflake edition

* Install `dotnet-sdk` from the repos:

`pacman -S dotnet-sdk`

### Compiling

* Run `dotnet build` from this base directory.

### Running

* Enter the `Daemon` directory

* Run `dotnet run`

* This will launch the daemon.

### Exploring

The main code base is available in the `CantiLib` folder.

## License

```
Copyright (c) 2018-2019 Canti, The TurtleCoin Developers

Please see the included LICENSE file for more information.
```

## Thanks

Thank you to all the awesome developers who have made their software open source!

* The CryptoNote Developers, The ByteCoin Developers, The Monero Developers
* Andrey N.Sabelnikov (For his Epee serialization which was reverse-engineered)
* The NewtonSoft Developers (For their excellent JSon library)

(Will add more as added)

If we have used your software and incorrectly attributed you, or not attributed you, please let us know!
