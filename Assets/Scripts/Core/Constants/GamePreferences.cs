#if UNITY_EDITOR
using ParrelSync;
#endif

namespace Core.Constants
{
    public static class GamePreferences
    {
        public static int AllClientsCount = 3;
        public static int PlayersCount = 2;
        
        #if UNITY_EDITOR
        public static bool IsServer => !ClonesManager.IsClone();
        #elif UNITY_STANDALONE
        public static bool IsServer => true;
        #elif UNITY_ANDROID
        public static bool IsServer => false;
        #elif UNITY_IOS
        public static bool IsServer => false;
        #endif
    }
}
