using UnityEngine;
public static class Bootstrapper
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    public static void Execute()
    {
        Object.DontDestroyOnLoad(Object.Instantiate(Resources.Load("Systems")));
    }

    // Systems prefabs
    // holds all the manager scripts that need to persist across scenes
    // InputManager
    // AudioManager (SoundMixerManager, SoundFXManager, MusicManager)
    // UIManager (Handles all UI related stuff, like opening/closing windows, updating UI elements, etc.)
    // ScreenShakeManager (Handles all screen shake related stuff, like shaking the camera, etc.)
    // GameManager (Handles all game related stuff, like pausing the game, handling game over, handles save/load, handles transition between game scenes or levels etc.)
    // -> on every start of level, GameManager spawns the player, and then all related scripts to player listens to that event and connect to the player, like PlayerHealth, etc.
    // -> LevelManager lives inside the level scene.
    //      - It assigns the math topic to the grimoire on the UIManager.
    //      - It also informs the GameManager about the current level, so it can handle the transition to the next level when the player completes the current level.
    //      - JSONSaveSystem is called by the GameManager on level completion (Handles all save/load related stuff, like saving/loading player progress, etc.)
    //      - JSON file serves as the database for the game, it holds all the player progress, unlocked levels, unlocked math topics, etc.
}