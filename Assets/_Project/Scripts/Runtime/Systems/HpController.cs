using UnityEngine;

public class HpController : MonoBehaviour
{
    public int hp;

    private void GetDamage(int amount)
    {
        hp -= amount;
        if (hp <= 0)
        {
            Destroy(gameObject);
        }
    }
}
