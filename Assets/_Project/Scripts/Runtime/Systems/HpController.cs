using System;
using UnityEngine;
using UnityEngine.SceneManagement;


[Serializable]
public struct Attributes
{
    public float attackSpeed; // velocidade de ataque
    public int[] damageAmount; // dano
    public int hp;
}

public class HpController : MonoBehaviour
{
    public event Action OnEnemyDie;
    public event Action OnPlayerDie;

    private int totalHp;

    public bool isDead;
    public bool isPlayer;

    [Header("Attributes")]
    public Attributes attributes;

    private void Start()
    {
        totalHp = attributes.hp;
    }

    private void GetDamage(int amount)
    {
        attributes.hp -= amount;
        if (attributes.hp <= 0)
        {
            isDead = true;
            if (isPlayer)
            {
               ResetLevel();
            }
            else
            {
                OnEnemyDie?.Invoke();
            }
        }
    }

    public void PlayerUp(int value)
    {
        print("player melhora");
        attributes.hp +=  5 + value;
        attributes.damageAmount[0] += 1;
    }

    public void LevelUpEnemy(int value)
    {
        attributes.hp = totalHp * value;
        //attributes.attackSpeed = 
        attributes.damageAmount[0] += 1;


        print("vida " + totalHp * value);
        print("dano " + (1 + value / 2));

    }

    private void ResetLevel()
    {
        Debug.LogError("Morreu");
    }
}
