
# HID#


HID# is a wrapper for the [hidapi](https://github.com/libusb/hidapi/tree/master) library written in C. It is an implementation to read and write directly to any HID available on your device. Why did I do this? To have another way to monitor my Keychron K10 Pro Keyboards. There are other libraries available for NodeJS and Python, so why not have a C# implementation? I personally believe in trying to expand through other languages to make our lives easier.

## Getting Started

These instructions will give you a copy of the project up and running on
your local machine for development and testing purposes. See deployment
for notes on deploying the project on a live system.

### Prerequisites

Requirements for the software and other tools to build, test and push:  
* Latest .NET Framework

### Installing

Simply clone this repo to your choice of IDE, be it Rider, VS Code, or Visual Studio. For now, the HIDAPI x64 library is available; however, eventually I and future contributors would need to add x86 backwards compatibility and rewrite the code to become more platform agnostic.

## Built With

Built with Visual Studio 2022 Enterprise with .NET 8.0

- [.NET 8.0](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [Visual Studio](https://visualstudio.microsoft.com/)


## Contributing

Please read [CONTRIBUTING.md](CONTRIBUTING.md) for details on our code
of conduct, and the process for submitting pull requests to us.

## Versioning

We use [Semantic Versioning](http://semver.org/) for versioning.  
Examples:  
* 1.0.0
* 1.1.0-alpha
* 1.0.1+32
* 1.0.0-alpha+64  

and so forth.

## Authors

See also the list of
[contributors](https://github.com/Blizzardo1/hid-sharp/contributors)
who participated in this project.

- **Adonis Deliannis** - _Lead Author_ - [Blizzardo1](https://github.com/Blizzardo1)


## License

This project is licensed under the [GPL v3](LICENSE.md)
License.  
See the [LICENSE.md](LICENSE.md) file for
details

## Acknowledgments

- Hat tip to anyone whose code is used
- Inspiration for my Keychron K10 Pro Keyboards
- The [HID API](https://github.com/libusb/hidapi)
- You
