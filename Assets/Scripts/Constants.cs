using Codice.Client.BaseCommands;
using UnityEditor.Build.Reporting;
using UnityEngine;

/**
 * Responsible for keeping constants
 * Constants are re-used in multiple components
 */
public class Constants
{
    // Scene names
    public const string INFINITE_SPAWN_SCENE_NAME = "STEAMInfiniteSpawn Map1";
    public const string MAIN_MENU_SCENE_NAME = "MainMenuSteam";
    public const string SNIPE_TO_WIN_SCENE_NAME = "TempSnipeToWin";

    // Player pref ids
    public const string DIFFICULTY_PREF_KEY = "difficulty";
    public const string SENSITIVITY_PREF_KEY = "sensitivity";
    public const string VOLUME_PREF_KEY = "volume";
    public const string MUSIC_PREF_KEY = "music";

    // Escape menu feedback text
    public const string SPAWN_JUGGERNAUT_FEEDBACK = "juggernaut spawned";
    public const string SPAWN_JUGGERNAUT_BOSS_FEEDBACK = "juggernaut boss spawned";
    public const string SPAWN_ASSASSIN_FEEDBACK = "assassin spawned";
    public const string SPAWN_ASSASSIN_BOSS_FEEDBACK = "assassin boss spawned";
    public const string SPAWN_DISABLED_FEEDBACK = "spawning disabled";
    public const string SPAWN_FEEDBACK_ANIM_NAME = "SpawnFeedback";

}
