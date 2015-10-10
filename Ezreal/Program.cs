using EloBuddy.SDK.Events;

namespace Ezreal
{
    internal class Program
    {
        private static void Main()
        {
            Loading.OnLoadingComplete += eventArgs => Ezreal.OnLoad();
        }
    }
}