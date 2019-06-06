# MTGAHue
Integrate MTG Arena with LEDs (Philips Hue Lights, Razer Keyboards and Corsair Keyboards)

![Demo](Docs/demo.gif)

## To-do:

* No UI. Customization is done using the `config.json` file
* No artifact support
* A base effect based on the current state of the game (Eg. Make the keyboard show life totals?)

## Development:

 * Compiles in C# 8.0, so you'll need VS2019 or equivalent (I really like nullable reference types)
 * The start up application should be `MagicLights.Console`
 * You may need to download the CUE SDK if using Corsair and NuGet doesn't automatically download it 

## Other Related Projects

[MTGA Tracker](https://mtgatracker.com/) - MTG Arena Deck/Game Tracker application. A lot of logic was borrowed from here

[Q42.HueApi](https://github.com/Q42/Q42.HueApi) - Philips Hue API

[Colore](https://github.com/chroma-sdk/Colore/) - Razer Chroma SDK

[CUE.Net](https://github.com/DarthAffe/CUE.NET) - Corsair CUE SDK
