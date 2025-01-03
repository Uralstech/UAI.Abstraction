# UAI.Abstraction

Contains abstraction for common AI APIs so that different LLM providers like Gemini and OpenAI can be used through a unified interface. Currently supports the Google Gemini and OpenAI GPT APIs.

[![openupm](https://img.shields.io/npm/v/com.uralstech.uai.abstraction?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.uralstech.uai.abstraction/)
[![openupm](https://img.shields.io/badge/dynamic/json?color=brightgreen&label=downloads&query=%24.downloads&suffix=%2Fmonth&url=https%3A%2F%2Fpackage.openupm.com%2Fdownloads%2Fpoint%2Flast-month%2Fcom.uralstech.uai.abstraction)](https://openupm.com/packages/com.uralstech.uai.abstraction/)

## Installation

Requires Unity 6.0 because of the plugin's usage of [*Awaitable*](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Awaitable.html). Built and tested in Unity 6.0.

### OpenUPM

1. Open project settings
2. Select `Package Manager`
3. Add the OpenUPM package registry:
    - Name: `OpenUPM`
    - URL: `https://package.openupm.com`
    - Scope(s)
        - `com.uralstech`
4. Open the Unity Package Manager window (`Window` -> `Package Manager`)
5. Change the registry from `Unity` to `My Registries`
6. Add the `UAI.Abstraction` package

### Unity Package Manager

1. Open the Unity Package Manager window (`Window` -> `Package Manager`)
2. Select the `+` icon and `Add package from git URL...`
3. Paste the UPM branch URL and press enter:
    - `https://github.com/Uralstech/UAI.Abstraction.git#upm`

### GitHub Clone

1. Clone or download the repository from the desired branch (master, preview/unstable)
2. Drag the package folder `UAI.Abstraction/UAI.Abstraction/Packages/com.uralstech.uai.abstraction` into your Unity project's `Packages` folder

## Preview Versions

Do not use preview versions (i.e. versions that end with "-preview") for production use as they are unstable and untested.

## Documentation

See <https://uralstech.github.io/UAI.Abstraction/DocSource/QuickStart.html> or `APIReferenceManual.pdf` and `Documentation.pdf` in the package documentation for the reference manual and tutorial.
