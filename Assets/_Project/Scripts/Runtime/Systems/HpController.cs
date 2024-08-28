using System;
using System.Collections;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;


[Serializable]
public struct Attributes
{
    public float attackSpeed; // velocidade de ataque
    public int[] damageAmount; // dano
    public int hp;
}

public class HpController : MonoBehaviour
{
    public TextMeshProUGUI[] hudElements;

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
        UpdateHud();
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

        UpdateHud();
    }

    public void PlayerUp(int value)
    {
        print("player melhora");

        attributes.hp += Random.Range(0, 5) + value;

        if (attributes.damageAmount[0] < value / 2)
        {
            attributes.damageAmount[0] += 1;
        }

        UpdateHud();
    }

    public void LevelUpEnemy(int value)
    {
        attributes.hp = Random.Range(value + totalHp, value * value);

        attributes.attackSpeed = Random.Range(0.8f, 1.6f);

        attributes.damageAmount[0] = Random.Range(1, value / 2);

        print("vida " + attributes.hp);

        UpdateHud();
    }

    private void ResetLevel()
    {
        //Debug.LogError("Morreu");
    }

    private void UpdateHud()
    {
        hudElements[0].text = "Vida "  + attributes.hp.ToString();
        hudElements[1].text = "Ataque " + attributes.damageAmount[0].ToString();
        hudElements[2].text = "Velocidade " + attributes.attackSpeed.ToString("N1");
    }
}
