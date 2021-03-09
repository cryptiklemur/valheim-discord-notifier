# Valheim Discord Notifier

### Note

This mod only works on servers (Dedicated and Non-Dedicated). 

## Table of Contents
1. [Installation](#Installation-(manual))
2. [Default Config](#Config)
3. [Changelog](#Changelog)

## Installation (manual)

1. Extract the archive into a folder. **Do not extract into the game folder.**
2. Move the contents of `DiscordNotifier` folder into `<GameDirectory>\Bepinex\plugins` (should create a `<GameDirectory>Bepinex\plugins\DiscordNotifier` directory)
3. Start the server (or game), it will generate automatically an configuration file into `<GameDirectory>\Bepinex\config\CryptikLemur_DiscordNotifier.cfg` and `<GameDirectory>\Bepinex\config\CryptikLemur_DiscordNotifier.messages.json`

## Config

The config is fairly well documented. 

Under `Events`, you can toggle which events you want to receive on the webhook
Under `General` are the basic config settings. Make sure you set the "WebhookUrl" here.

## Changelog

#### 0.0.5

- Added messages for OnServerStart and OnServerStop

#### 0.0.4

- Added new CryptikLemur_DiscordNotifier.messages.json, adds ability to have multiple messages for join, leave, and death
- Added messages for whispers
- Added ability to toggle different type of chat messages
- Added ability to toggle ToUpper on shouts

#### 0.0.3

- Fixing death leave/join bug again, maybe?
- Adding filters to chat events (By Username, by Regex)
- Adding the ability to customize join, leave, and death messages

#### 0.0.2

- Fixing death leave/join bug
- Removing respawn event
- Adding chat event
- Added the ability to show the IP on server startup message

#### 0.0.1
- Initial release, unstable until tested more
