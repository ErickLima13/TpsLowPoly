using System;
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

    public int totalHp;

    public bool isDead;
    public bool isPlayer;

    [Header("Attributes")]
    public Attributes attributes;

    public GameObject popUpText;

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

        float perc = totalHp * 0.1f + value;
        print(perc + " por centos");

        if (attributes.hp < totalHp)
        {
            attributes.hp += (int)perc;
        }


        //if (attributes.damageAmount[0] < value / 2)
        //{
        //    attributes.damageAmount[0] += 1;
        //}

        UpdateHud();
    }

    public void LevelUpEnemy(int value)
    {
        attributes.hp = Random.Range(15, value * 3);
        totalHp = attributes.hp;
        attributes.attackSpeed = Random.Range(0.8f, 1.2f);

        attributes.damageAmount[0] = Random.Range(1, value);

        print("vida " + attributes.hp);

        UpdateHud();
    }

    private void ResetLevel()
    {
        //Debug.LogError("Morreu");
    }

    public void UpdateHud()
    {
        hudElements[0].text = "Life: " + attributes.hp.ToString() + "/" + totalHp;
        hudElements[1].text = "Atk: " + attributes.damageAmount[0].ToString();
        hudElements[2].text = "Speed: " + attributes.attackSpeed.ToString("N1");
    }

    public void InstancePopUp(int value)
    {
        return;

        GameObject temp = Instantiate(popUpText);
        Transform t = temp.transform;

        switch (value)
        {
            case 0:
                t = hudElements[0].transform;
                break;
            case 1:
                t = hudElements[1].transform;
                break;
            case 2:
                t = hudElements[2].transform;
                break;
        }

        temp.transform.SetParent(t);
        temp.transform.localPosition = Vector3.zero;

    }

}
