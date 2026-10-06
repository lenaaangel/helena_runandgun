using System;
using UnityEngine;

public class PlayerEvents
{
    public static event Action<bool, float> OnMove;
    public static event Action<bool, float> OnLook;
    public static event Action<bool> OnJump;
    public static event Action<bool> onDeath;
    public static event Action<bool> onAttack;

    public static void RaiseMovePerformed(bool isMoving, float direction)
    {
        OnMove?.Invoke(isMoving, direction);
    }

    public static void RaiseJumpPerformed(bool isJumping)
    {
        OnJump?.Invoke(isJumping);
    }

    public static void RaisePlayerDeath(bool isDead)
    {
        onDeath?.Invoke(isDead);
    }

    public static void RaisePlayerAttack(bool isAttacking)
    {
        onAttack?.Invoke(isAttacking);
    }

    public static void RaiseLookPerformed(bool isLooking, float direction)
    {
        OnLook?.Invoke(isLooking, direction);
    }
}
