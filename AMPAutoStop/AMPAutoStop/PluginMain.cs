using ModuleShared;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

//Your namespace must match the assembly name and the filename. Do not change one without changing the other two.
namespace AMPAutoStop
{
    //The first class must be called PluginName
    public class PluginMain : AMPPlugin
    {
        private readonly Settings _settings;
        private readonly ILogger log;
        private readonly IConfigSerializer _config;
        private readonly IPlatformInfo platform;
        private readonly IRunningTasksManager _tasks;
        private readonly IApplicationWrapper application;
        private int timeSinceLastUser { get; set; }
        private Task runningThread { get; set; }

        //All constructor arguments after currentPlatform are optional, and you may ommit them if you don't
        //need that particular feature. The features you request don't have to be in any particular order.
        //Warning: Do not add new features to the feature manager here, only do that in Init();
        public PluginMain(ILogger log, IConfigSerializer config, IPlatformInfo platform,
            IRunningTasksManager taskManager, IApplicationWrapper Application)
        {
            //These are the defaults, but other mechanisms are available.
            config.SaveMethod = PluginSaveMethod.KVP;
            config.KVPSeparator = "=";
            this.log = log;
            _config = config;
            this.platform = platform;
            _settings = config.Load<Settings>(AutoSave: true); //Automatically saves settings when they're changed.
            _tasks = taskManager;
            application = Application;
            _settings.SettingModified += Settings_SettingModified;
        }

        /*
            Rundown of the different interfaces you can ask for in your constructor:
            IRunningTasksManager - Used to put tasks in the left hand side of AMP to update the user on progress.
            IApplicationWrapper - A reference to the running application from the running module.
            IPluginMessagePusher - For 'push' type notifications that your front-end code can react to via PushedMessage in Plugin.js
            IFeatureManager - To expose/consume features to/from other plugins.
        */

        //Your init function should not invoke any code that depends on other plugins.
        //You may expose functionality via IFeatureManager.RegisterFeature, but you cannot yet use RequestFeature.
        public override void Init(out WebMethodsBase APIMethods)
        {
            APIMethods = new WebMethods(_tasks);
        }
        public override bool HasFrontendContent => true;

        //This gets called after every plugin is loaded. From here on it's safe
        //to use code that depends on other plugins and use IFeatureManager.RequestFeature
        public override void PostInit()
        {
            log.Activity("AutoStop Initiated");
            if (_settings.MainSettings.AutoStopActive)
            {
                try
                {
                    runningThread = Task.Run(ExecuteAutoStart);
                }
                catch (Exception exception)
                {
                    log.Activity("Error with the AutoStop Plugin : " + exception.Message);
                }
            }
        }

        void Settings_SettingModified(object sender, SettingModifiedEventArgs e)
        {
            try
            {
                var noThreadRunning = runningThread == null;
                //Stop the thread if its running and we turned AutoStop off
                if (!_settings.MainSettings.AutoStopActive && !noThreadRunning)
                {
                    log.Activity("AutoStop: Settings changed and runningThread is running, waiting for completion...");
                    noThreadRunning = runningThread.Wait(12000);

                    if (noThreadRunning) runningThread = null;
                    else log.Activity("AutoStop Warning: The runningThread couldn't complete. Either AutoStop was reactivated or it's stuck. Restarting the instance might be needed");
                }
                //Start the thread if its not running and we turned AutoStop on
                else if (_settings.MainSettings.AutoStopActive && noThreadRunning)
                {
                    runningThread = Task.Run(ExecuteAutoStart);
                }
            }
            catch (Exception exception)
            {
                log.Activity("AutoStop Error: " + exception.Message);
            }
        }

        public override IEnumerable<SettingStore> SettingStores => Utilities.EnumerableFrom(_settings);

        private void SetTimeSinceLastUser()
        {
            //cast to get player count / info
            IHasSimpleUserList hasSimpleUserList = application as IHasSimpleUserList;
            IHasWriteableConsole hasWriteableConsole = application as IHasWriteableConsole;

            var onlinePlayers = hasSimpleUserList.Users.Count;
            if (onlinePlayers > 0) timeSinceLastUser = 0;
            else
            {
                timeSinceLastUser += _settings.MainSettings.AutoStopRefreshInterval;
                hasWriteableConsole.WriteLine("say AutoStop: Time since last user was on: " + timeSinceLastUser + " minutes. Server will shutdown when it reaches " + _settings.MainSettings.TimeLimit);
            }
        }

        private bool CheckIfServerShouldStop() { return timeSinceLastUser >= _settings.MainSettings.TimeLimit; }

        public async Task ExecuteAutoStart()
        {
            try
            {
                log.Activity("AutoStop: runningThread Start");
                while (_settings.MainSettings.AutoStopActive)
                {
                    if (application.State == ApplicationState.Ready)
                    {
                        SetTimeSinceLastUser();
                        if (CheckIfServerShouldStop()) application.Stop();
                    }
                    else timeSinceLastUser = 0;

                    for(int i = 0; i < _settings.MainSettings.AutoStopRefreshInterval * 6; i++)
                    {
                        if (_settings.MainSettings.AutoStopActive) await Task.Delay(10000);
                        else break;
                    }
                }
                log.Activity("AutoStop: runningThread End");
            }
            catch (System.Net.WebException exception)
            {
                log.Activity("AutoStop Error: " + exception.Message);
            }
        }
    }
}
