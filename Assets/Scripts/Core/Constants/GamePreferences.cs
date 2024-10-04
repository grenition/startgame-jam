using ParrelSync;

namespace Core.Constants
{
    public static class GamePreferences
    {
        public static int AllClientsCount = 3;
        public static int PlayersCount = 2;
        
        #if UNITY_EDITOR
        public static bool IsServer => !ClonesManager.IsClone();
        #else
        public static bool IsServer => false;
        #endif
    }
}
