using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MultiplayerARPG;

public class DisableRootMotionWhileMoving : MonoBehaviour
{
    Animator animator;
    BaseCharacterEntity character;
    private void Start()
    {
        animator = GetComponent<Animator>();
        character = GetComponent<BaseCharacterEntity>();
    }

    private void OnAnimatorMove()
    {
        if (character.MovementState.HasFlag(MovementState.IsGrounded) &&
            !character.MovementState.HasFlag(MovementState.Forward) &&
            !character.MovementState.HasFlag(MovementState.Backward) &&
            !character.MovementState.HasFlag(MovementState.Left) &&
            !character.MovementState.HasFlag(MovementState.Right))
            animator.ApplyBuiltinRootMotion();
    }
}
