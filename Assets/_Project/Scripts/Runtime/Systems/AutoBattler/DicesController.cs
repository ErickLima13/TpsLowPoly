using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class DicesController : MonoBehaviour
{
    private WaveController waveController;

    private string templateBase;

    public List<int> dices = new List<int>();

    public HpController hpPlayer;

    public int randomValue;

    public float speedPlayer;
    public int damagePlayer;

    public float powerUpTime;

    public TextMeshProUGUI diceValue;
    public TextMeshProUGUI diceReward;

    private void Start()
    {
        waveController = FindObjectOfType<WaveController>();
        speedPlayer = hpPlayer.attributes.attackSpeed;
        damagePlayer = hpPlayer.attributes.damageAmount[0];

        diceValue.text = "x " + dices.Count;
        diceReward.text = null;
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
                    templateBase = "ganhei velocidade";
                    break;
                case 1: // tirar 2 no dado 
                    hpPlayer.attributes.damageAmount[0] += 1;
                    templateBase = "ganhei dano";
                    break;
                case 2: // tirar 3 no dado 
                    hpPlayer.attributes.hp += 5;
                    templateBase = "ganhei vida";
                    break;
                case 3: // tirar 4 no dado 
                    hpPlayer.SendMessage("GetDamage",
                waveController.waveNumber, SendMessageOptions.DontRequireReceiver);
                    templateBase = "perdeu " + waveController.waveNumber + " de vida";
                    break;
            }

            dices.Remove(dices[0]);
        }
        else
        {
            templateBase = "sem dados";
        }

        UpdateReward();
    }

    private void UpdateReward()
    {
        diceValue.text = "x " + dices.Count;
        diceReward.text = templateBase;
        StartCoroutine("DelayPowerUpText");
    }

    public void AddDice()
    {
        dices.Add(1);
        templateBase = "ganhei um dado";
        UpdateReward();
    }

    private IEnumerator DelayPowerUpText()
    {
        yield return new WaitForSeconds(1.5f);
        diceReward.text = null;
    }

    private IEnumerator DelayPowerUp()
    {
        hpPlayer.attributes.attackSpeed -= 0.2f;
        yield return new WaitForSeconds(powerUpTime);
        hpPlayer.attributes.attackSpeed = speedPlayer;
    }




}
