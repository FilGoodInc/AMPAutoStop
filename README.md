
# AMP Auto Stop

A instance auto stop plugin for [AMP by Cubecoders](https://cubecoders.com/AMP) that can be used to stop an instance whenever no players are online.

**Note:**
I recommend https://github.com/winglessraven/AMP-Discord-Bot to be paired with this for ability to start the server again outside of AMP.
I used their code to help me learn how to make mine so thanks to @winglessraven

# Configuration Steps


## Configure AMP
Before the plugin can be used you need to configure AMP in a specific way.  **NOTE: The plugin cannot be used on ADS. It can only run on non-ADS instances.**
### Activate with a Developer Licence
* Request a developer licence from AMP via the licence portal https://manage.cubecoders.com/Login
* Once you have received the licence log into your server console and as the amp user run `ampinstmgr stop [ADS INSTANCE NAME]` followed by `ampinstmgr reactivate [INSTANCE NAME] [DEVELOPER KEY]`  where `[INSTANCE NAME]` is the name of your instance you want to set up a bot for and `[DEVELOPER KEY]` is the key received from CubeCoders.
* Start your ADS instance again with `ampinstmgr start [ADS INSTANCE NAME]`.

### Installing and Enabling the Plugin
* Edit the AMPConfig.conf file in the root folder of your instance (e.g. `/home/amp/.ampdata/instances/INSTANCENAME01/AMPConfig.conf`)
* Under AMP.LoadPlugins add `AMPAutoStop` to the list (e.g. `AMP.LoadPlugins=["FileManagerPlugin","EmailSenderPlugin","WebRequestPlugin","LocalFileBackupPlugin","CommonCorePlugin","AMPAutoStop"]`)
* In the plugins folder for your instance (e.g. `/home/amp/.ampdata/instances/INSTANCENAME01/Plugins/`) create a new folder called `AMPAutoStop` and insert the .dll file from the current [release](https://github.com/FilGoodInc/AMPAutoStop/releases "release")
* Restart your instance with `ampinstmgr restart [INSTANCE NAME]`
* Log in to your instance and you will see a new menu item under Configuration for the AMPAutoStop

![Menu Item](docs\config.png "Menu Item")

## AMP Menu Settings
| Option | Description                    |
| ------------- | ------------------------------ |
|AutoStop Activated|AutoStop shut downs the server when no players are on after a time limit. Turn on with this setting.|
|AutoStop Refresh Interval|How often, in minutes, the bot should update the time since last player was on. <br /> *Note: the refresh interval runs independantly. If the last player disconnects at the 9th minute of the interval, when the refresh occus (in 1 minute), it will add up 10 minutes even if only 1 has passed.*|
|AutoStop Time Limit|How long, in minutes, does the server has to be empty to shut down.|

![Config Settings](docs\config_settings.png "Config Settings")