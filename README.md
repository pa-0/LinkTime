![LINKTIME](https://gitlab.com/tobiaskoch/LinkTime/raw/master/Media/LinkTime-256.png)

# LINKTIME

LinkTime is a small .NET based console application reading the [linker](https://en.wikipedia.org/wiki/Linker_(computing)) timestamp of the [Microsoft Windows Portable Executable (PE) format](https://en.wikipedia.org/wiki/Portable_Executable).

## Installation
This application can be deployed using xcopy.

### Option 1: Binary
Stable versions can be downloaded [here](https://gitlab.com/tobiaskoch/LinkTime/pipelines?scope=tags).

### Option 2: Source
#### Requirements
The following applications must be available:

* MSBuild (.NET Framework / Mono; [Visual Studio](https://www.visualstudio.com) recommended for development)

#### Source code
Get the source code using the following command:

    > git clone https://gitlab.com/tobiaskoch/LinkTime.git

#### Build
    > ./Build.ps1 --configuration=Release

The executable will be located in the directory *.\Build\Release* if the build succeeds, a zip file containing the software will be located in the root directory of the repository.

#### Test
    > ./Build.ps1

The script will report if the unit tests succeeds, the coverage report will be placed in the directory *.\Build\Debug\Coverage*.

## Contributing
see [CONTRIBUTING.md](https://gitlab.com/tobiaskoch/LinkTime/blob/master/CONTRIBUTING.md)

## Contributors
see [AUTHORS.txt](https://gitlab.com/tobiaskoch/LinkTime/blob/master/AUTHORS.txt)

## Donating
Thanks for your interest in this project. You can show your appreciation and support further development by [donating](https://www.tk-software.de/donate).

## License
**LinkTime** © 2017-2024  Tobias Koch. Released under the [MIT License](https://gitlab.com/tobiaskoch/LinkTime/blob/master/LICENSE.md).