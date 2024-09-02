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
    public float delayText;
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
                    templateBase = "Your atk speed up";
                    hpPlayer.InstancePopUp(2);
                    break;
                case 1: // tirar 2 no dado 
                    hpPlayer.attributes.damageAmount[0] += 1;
                    templateBase = "Your atk up";
                    hpPlayer.InstancePopUp(1);
                    break;
                case 2: // tirar 3 no dado 
                    int randHp = Random.Range(5, waveController.waveNumber * 2);
                    hpPlayer.totalHp += randHp;
                    templateBase = "Your max life increase in " + randHp;
                    hpPlayer.InstancePopUp(0);
                    break;
                case 3: // tirar 4 no dado 
                    hpPlayer.SendMessage("GetDamage",
                waveController.waveNumber, SendMessageOptions.DontRequireReceiver);
                    templateBase = "You lost " + waveController.waveNumber + " of life";
                    break;
            }

            dices.Remove(dices[0]);
        }
        else
        {
            templateBase = "No more dices";
        }

        UpdateReward();
    }

    private void UpdateReward()
    {
        diceValue.text = "x " + dices.Count;
        diceReward.text = templateBase;
        hpPlayer.UpdateHud();
        StartCoroutine("DelayPowerUpText");
    }

    public void AddDice()
    {
        dices.Add(1);
        templateBase = "You won a dice";
        UpdateReward();
    }

    private IEnumerator DelayPowerUpText()
    {
        yield return new WaitForSeconds(delayText);
        diceReward.text = null;
    }

    private IEnumerator DelayPowerUp()
    {
        hpPlayer.attributes.attackSpeed -= 0.2f;
        yield return new WaitForSeconds(powerUpTime);
        hpPlayer.attributes.attackSpeed = speedPlayer;
    }




}
