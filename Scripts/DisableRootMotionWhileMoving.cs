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
        if (character.MovementState.Has(MovementState.IsGrounded) &&
            !character.MovementState.Has(MovementState.Forward) &&
            !character.MovementState.Has(MovementState.Backward) &&
            !character.MovementState.Has(MovementState.Left) &&
            !character.MovementState.Has(MovementState.Right))
            animator.ApplyBuiltinRootMotion();
    }
}
