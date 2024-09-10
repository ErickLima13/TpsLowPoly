using UnityEngine;

public class AnimationControl : MonoBehaviour
{
    private Animator animator;
    public int idWeapon;

    private void Start()
    {
        animator = GetComponent<Animator>();
        idWeapon = Random.Range(0, 2);
        animator.SetInteger("idW", idWeapon);
    }


    public void DisableAnimator()
    {
        animator.enabled = false;
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
