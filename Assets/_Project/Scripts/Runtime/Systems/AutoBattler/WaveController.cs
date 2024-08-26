using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaveController : MonoBehaviour
{
    private DicesController dicesController;

    public event Action NewEnemyEvent;

    public List<HpController> enemies = new();

    public float timeToSpawn;

    private HpController enemy;

    private void Start()
    {
        enemies = FindObjectsOfType<HpController>(true).ToList(); // true para pegar objetos desativados
        dicesController = FindObjectOfType<DicesController>();

        foreach (HpController h in enemies)
        {
            if (!h.isPlayer)
            {
                h.Ondie += RespawEnemy;
            }
        }
    }

    private void OnDestroy()
    {
        enemies = FindObjectsOfType<HpController>().ToList();

        foreach (HpController h in enemies)
        {
            if (!h.isPlayer)
            {
                h.Ondie -= RespawEnemy;
            }
        }
    }

    private void RespawEnemy()
    {
        StartCoroutine("RespawnDelay");
    }

    private IEnumerator RespawnDelay()
    {
        foreach (HpController h in enemies)
        {
            if (!h.isPlayer)
            {
                h.gameObject.SetActive(false);
                enemy = h;
            }
        }

        dicesController.AddDice();

        yield return new WaitForSeconds(timeToSpawn);
        enemy.isDead = false;
        enemy.gameObject.SetActive(true);     
        NewEnemyEvent?.Invoke();
    }


}
