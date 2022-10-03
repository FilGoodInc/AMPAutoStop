using ModuleShared;

namespace AMPAutoStop
{
    public class Settings : SettingStore
    {
        public class AutoStopSettings : SettingSectionStore
        {
            [WebSetting("AutoStop Activated", "AutoStop shut downs the server when no players are on after a time limit. Turn on with this setting.", false)]
            public bool AutoStopActive = false;

            [WebSetting("AutoStop Refresh Interval", "How often, in minutes, the bot should update the time since last player was on.", false)]
            public int AutoStopRefreshInterval = 5;

            [WebSetting("AutoStop Time Limit", "How long, in minutes, does the server has to be empty to shut down.", false)]
            public int TimeLimit = 30;
        }

        public AutoStopSettings MainSettings = new AutoStopSettings();
    }
}
