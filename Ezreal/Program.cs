using EloBuddy.SDK.Events;

namespace Ezreal
{
    class Program
    {
        static void Main()
        {
            Loading.OnLoadingComplete += eventArgs => Ezreal.OnLoad();
        }
    }
}
