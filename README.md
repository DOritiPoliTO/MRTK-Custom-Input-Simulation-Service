# Custom Input Simulation Service for MixedRealityToolkit v2

This alternative Input Simulation Service simulates a VR system and, contrarily to the build-in Input Simulation Service of the MRTKv2, works also when building the project for Windows Desktop.
Since this was meant to work with Win32 build, it has been tested with Win32 only, but it should work on UWP build too.

## Getting Started

### Prerequisites

* [MixedRealityToolkit](https://github.com/Microsoft/MixedRealityToolkit-Unity) - MixedRealityToolkit for Unity

Please, check the MixedRealityToolkit github page for the toolkit specific requirements.

The code has been tested on [Microsoft Mixed Reality Toolkit v2.0.0 RC2.1](https://github.com/microsoft/MixedRealityToolkit-Unity/releases/tag/v2.0.0-RC2.1).

### Installing

* Clone this repository.

* Open the unity project selecting the cloned repository folder from Unity->Open Project->Open->[select_cloned_repository_folder].

## Running the tests

Press key P to disable the mouse and to control the SimulatedHeadset; pressing P again let you control the mouse again.
The SimulatedHeadset can be controlled with the mouse and the keys W, A, S, D.

Press V (or B for right controller) to create a SimulatedMotionController.

Press N and keep the key down (or M for left controller) to move the SimulatedMotionController using the mouse and the keys W, A, S, D.

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details
