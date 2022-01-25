using KLauncher.Libs.Core;

namespace KLauncher.Libs
{
    public sealed class Current
    {
        private static Current instance;
        public static Current Instance
        {
            get
            {
                if (instance == null)
                    instance = new Current();
                return instance;
            }
        }
        private object HasInit { get; set; } = false;
        private Current() { }
        public IApiClient ApiClient { get; private set; }
        public void Init()
        {
            if (HasInit is bool value && value == false)
            {
                HasInit = true;
                ApiClient = SettingHelper.UseRest ? new RestClientEx() : new HttpClientEx();
            }
        }
    }
}