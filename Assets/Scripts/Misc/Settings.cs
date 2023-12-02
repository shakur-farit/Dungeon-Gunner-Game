using UnityEngine;
using System;

public static class Settings 
{
    // Units
    public const float pixelsPerUnit = 16f;
    public const float tileSizePixels = 16f;

    // Dungeon build settings
    public const int maxDungeonRebuildAttemptsForRoomGraph = 1000;
    public const int maxDungeonBuildAttempts = 10;

    // Room settings
    public const float faidInTime = 0.5f;
    public const int maxChildCorridors = 3;
    public const float doorUnlockDelay = 1f;

    // Player animation parameters
    public static int aimUp = Animator.StringToHash("aimUp");
    public static int aimDown = Animator.StringToHash("aimDown");
    public static int aimUpRight = Animator.StringToHash("aimUpRight");
    public static int aimUpLeft = Animator.StringToHash("aimUpLeft");
    public static int aimRight = Animator.StringToHash("aimRight");
    public static int aimLeft = Animator.StringToHash("aimLeft");
    public static int isIdle = Animator.StringToHash("isIdle");
    public static int isMoving = Animator.StringToHash("isMoving");
    public static int rollUp = Animator.StringToHash("rollUp");
    public static int rollRight = Animator.StringToHash("rollRight");
    public static int rollLeft = Animator.StringToHash("rollLeft");
    public static int rollDown = Animator.StringToHash("rollDown");
    public static int flipUp = Animator.StringToHash("flipUp");
    public static int flipRight = Animator.StringToHash("flipRight");
    public static int flipLeft = Animator.StringToHash("flipLeft");
    public static int flipDown = Animator.StringToHash("flipDown");
    public static float baseSpeedForPlayerAnimations = 8f;

    // Enemy animator parametrs
    public static float baseSpeedForEnemyAnimations = 3f;

    // Door animation parameters
    public static int open = Animator.StringToHash("open");

    // Damageable decoration animation parametrs
    public static int destroy = Animator.StringToHash("destroy");
    public static String stateDestroyed = "Destroyed";

    // Game object tags
    public const string playerTag = "Player";
    public const string playerWeapon = "playerWeapon";

    // Firing control
    // If the target distance is less than this value then the aim angle will be used (form player),
    // else weapon aim angle (from the weapon shoot position)
    public const float useAimAngleDistance = 3.5f;

    // AStar pathfinding parameters
    public const int defaultAStarMovementPenalty = 40;
    public const int prefferedPathAStarMovementPenalty = 1;
    public const int targetFrameRateToSpreadPathfidingOver = 60;
    public const float playerMoveDistanceToRebuildPath = 3f;
    public const float enemyPathRebuildCooldown = 2f;

    // Enemy parametrs
    public const int defaultEnemyHealth = 20;

    // UI parametrs
    public const float uiHeartSpacing = 16f;
    public const float uiAmmoIconSpacing = 4f;

    // Contact damage parametrs
    public const float contancDamageCollisionResetDellay = 0.5f;
}

