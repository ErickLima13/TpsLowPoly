using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationControl : MonoBehaviour
{
    private Animator animator;
    public int idRandom;

    private void Start()
    {
        animator = GetComponent<Animator>();
        idRandom = Random.Range(0, 2);
        animator.SetInteger("idW", idRandom);
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
