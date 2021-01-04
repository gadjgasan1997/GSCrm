using System.Resources;

namespace GSCrm.Localization
{
    public class ResManager : ResourceManager, IResManager
    {
        public ResManager() : base("GSCrm.Resource", typeof(Program).Assembly) { }
    }
}
