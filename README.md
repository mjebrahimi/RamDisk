[![NuGet](https://img.shields.io/nuget/v/RamDisk.svg)](https://www.nuget.org/packages/RamDisk/)
[![License: MIT](https://img.shields.io/badge/License-MIT-brightgreen.svg)](https://opensource.org/licenses/MIT)
[![Build Status](https://github.com/mjebrahimi/RamDisk/workflows/.NET%20Core/badge.svg)](https://github.com/mjebrahimi/RamDisk)

# RamDisk
**RamDisk** is a library for create virtual disk drive on system memory (**you have to have [ImDisk](http://www.ltr-data.se/opencode.html/#ImDisk) installed, which is used in the backend - [download link of current stable version 2.0.10](http://www.ltr-data.se/files/imdiskinst.exe)**) which supports Windows NT/2000/XP/Vista/7/8/8.1/10 and Windows Server 2003/2003 R2/2008/2008 R2/2012/2012 R2, 32 and 64 bit (and ARM) editions.

## Get Started

### 1. Install Package

```
PM> Install-Package RamDisk
```

### 2. Use it
Please note that all operations require your application to have administrator priviliges.

**Mount** a virtual disk drive for example size : 512MB and fileSystem : NTFS and name : 'Z' and label : "RamDisk".

```csharp
using RamDisk;

RamDrive.Mount(512, FileSystem.NTFS, 'Z', "RamDisk");

//or simpler (other parameters except size has default value)
RamDrive.Mount(512);
```

Then **Unmount** drive name : 'Z'

```csharp
using RamDisk;

RamDrive.Unmount('Z');

//or simpler (drive name has default value 'Z')
RamDrive.Unmount();
```

## Contributing

Create an [issue](https://github.com/mjebrahimi/RamDisk/issues/new) if you find a BUG or have a Suggestion or Question. If you want to develop this project, Fork on GitHub and Develop it and send Pull Request.

A **HUGE THANKS** for your help.

## License

RamDisk is Copyright © 2020 [Mohammd Javad Ebrahimi](https://github.com/mjebrahimi) under the [MIT License](https://github.com/mjebrahimi/RamDisk/LICENSE).
