using System.Resources;

namespace GSCrm.Localization
{
    public class ResManager : ResourceManager
    {
        public ResManager() : base("GSCrm.Resource", typeof(Program).Assembly) { }
    }
}
