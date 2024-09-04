using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon")]
public class Weapon : ScriptableObject
{
    public int weaponDamage;
    public int fireRate;
    public int blastShots;
    public int accuracy;
    public int ammunitionBase;
    public int ammunition;
    public int ammunitionExtraBase;
    public int ammunitionExtra;

    public float delayBetweenBullets;
    public float delayBetweenShots;
    public float timeToReaload;

    public void StartWeapon()
    {
        float tps = (float)fireRate / 60;
        delayBetweenBullets = 1f / tps;
        ammunition = ammunitionBase;
        ammunitionExtra = ammunitionExtraBase;
    }

    public void Reload()
    {
        ammunition = ammunitionBase;
        ammunitionExtra--;
    }
}
