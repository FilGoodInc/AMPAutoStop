using ModuleShared;

namespace AMPAutoStop
{
    class WebMethods : WebMethodsBase
    {
        private readonly IRunningTasksManager _tasks;

        public WebMethods(IRunningTasksManager tasks)
        {
            _tasks = tasks;
        }

    }
}
