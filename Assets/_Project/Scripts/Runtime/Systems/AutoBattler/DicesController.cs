using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DicesController : MonoBehaviour
{
    public List<int> dices = new List<int>();

    public HpController hpPlayer;
    public AutoCombat combatPlayer;

    public int randomValue;

    public float speedPlayer;
    public int damagePlayer;

    public float powerUpTime;

    public TextMeshProUGUI diceValue;

    private void Start()
    {
        speedPlayer = combatPlayer.attackSpeed;
        damagePlayer = combatPlayer.damageAmount[0];

        diceValue.text = "x " + dices.Count; 
    }

    [ContextMenu("SortANumber")]
    public void SortANumber()
    {
        if (dices.Count > 0)
        {
            randomValue = Random.Range(0, 4);

            switch (randomValue)
            {
                case 0: // tirar 1 no dado 
                    StartCoroutine("DelayPowerUp");
                    print("ganhei velocidade");
                    break;
                case 1: // tirar 2 no dado 
                    combatPlayer.damageAmount[0] += 1;
                    print("ganhei dano");
                    break;
                case 2: // tirar 3 no dado 
                    hpPlayer.hp += 5;
                    print("ganhei vida");
                    break;
                case 3: // tirar 4 no dado 
                    dices.Add(1);
                    print("ganhei um dado");
                    break;
            }

            dices.Remove(dices[0]);
        }
        else
        {
            print("sem dados");
        }

        diceValue.text = "x " + dices.Count;
    }

    private IEnumerator DelayPowerUp()
    {
        combatPlayer.attackSpeed -= 0.2f;

        yield return new WaitForSeconds(powerUpTime);

        combatPlayer.attackSpeed = speedPlayer;
    }

}
