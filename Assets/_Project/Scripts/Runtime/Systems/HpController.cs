using System;
using UnityEngine;

public class HpController : MonoBehaviour
{
    public event Action Ondie;

    public int hp;

    private int totalHp;

    public bool isDead;
    public bool isPlayer;

    private void Start()
    {
        totalHp = hp;
    }

    private void GetDamage(int amount)
    {
        hp -= amount;
        if (hp <= 0)
        {
            isDead = true;
            if (isPlayer)
            {

            }
            else
            {
                hp = totalHp;
                Ondie?.Invoke();
            }
        }
    }
}
