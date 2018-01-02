![LINKTIME](https://gitlab.com/tobiaskoch/LinkTime/raw/master/Media/LinkTime-256.png)

# LINKTIME

[![pipeline status](https://gitlab.com/tobiaskoch/LinkTime/badges/master/pipeline.svg)](https://gitlab.com/tobiaskoch/LinkTime/commits/master)

LinkTime is a small .NET based console application reading the [linker](https://en.wikipedia.org/wiki/Linker_(computing)) timestamp of the [Microsoft Windows Portable Executable (PE) format](https://en.wikipedia.org/wiki/Portable_Executable).

## Installation
This application can be deployed using xcopy.

### Option 1: Binary
Stable versions can be downloaded [here](https://gitlab.com/tobiaskoch/LinkTime/pipelines?scope=tags).

### Option 2: Source
#### Requirements
The following applications must be available and included in you *PATH* environment variable:

* [Git](https://git-scm.com/)
* [Nuget.exe](https://www.nuget.org/)
* MSBuild / XBuild ([Visual Studio](https://www.visualstudio.com) recommended for development)

#### Source code
Get the source code using the following command:

    > git clone https://gitlab.com/tobiaskoch/LinkTime.git

#### Build
    > .\Build.cmd

The executable will be located in the directory *.\Build\Release* if the build succeeds.

#### Test
    > .\Test.cmd

The script will report if the unit tests succeeds, the coverage report will be placed in the directory *.\Build\Debug\Coverage*.

## Contributing
see [CONTRIBUTING.md](https://gitlab.com/tobiaskoch/LinkTime/blob/master/CONTRIBUTING.md)

## Contributors
see [AUTHORS.txt](https://gitlab.com/tobiaskoch/LinkTime/blob/master/AUTHORS.txt)

## License
**LinkTime** © 2017  Tobias Koch. Released under the [MIT License](https://gitlab.com/tobiaskoch/LinkTime/blob/master/LICENSE.md).