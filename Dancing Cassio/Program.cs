using EloBuddy.SDK.Events;

namespace Dancing_Cassio
{
    class Program
    {
        static void Main()
        {
            Loading.OnLoadingComplete += eventArgs => Cassiopeia.Loading_OnLoadingComplete();
        }
    }
}