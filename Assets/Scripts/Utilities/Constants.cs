namespace Tsarkel.Utilities
{
    /// <summary>
    /// Constants used throughout the game.
    /// </summary>
    public static class Constants
    {
        // Tags
        public const string TAG_PLAYER = "Player";
        public const string TAG_LOW_STRUCTURE = "LowStructure";
        public const string TAG_BUILDING = "Building";
        public const string TAG_GROUND = "Ground";
        
        // Layers
        public const string LAYER_GROUND = "Ground";
        public const string LAYER_WATER = "Water";
        public const string LAYER_BUILDING = "Building";
        
        // Input
        public const string INPUT_HORIZONTAL = "Horizontal";
        public const string INPUT_VERTICAL = "Vertical";
        public const string INPUT_MOUSE_X = "Mouse X";
        public const string INPUT_MOUSE_Y = "Mouse Y";
        
        // Default Values
        public const float DEFAULT_SAFE_ELEVATION = 10f;
        public const float DEFAULT_WATER_LEVEL = 0f;
        public const float DEFAULT_GROUND_CHECK_DISTANCE = 0.1f;
    }
}
