using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationControl : MonoBehaviour
{
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    [ContextMenu("Pistol")]
    public void PlayPistol()
    {
        animator.Play("Pistol");
    }

    [ContextMenu("Rifle")]
    public void PlayRifle()
    {
        animator.Play("Rifle");
    }
}
