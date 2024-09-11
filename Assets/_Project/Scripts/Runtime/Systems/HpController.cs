using UnityEngine;

public class HpController : MonoBehaviour
{
    public int hp;

    public GameObject manager;

    private void GetDamage(int amount)
    {
        hp -= amount;
        manager.SendMessage("GetShot", SendMessageOptions.DontRequireReceiver);
        if (hp <= 0)
        {
            manager.SendMessage("Die",SendMessageOptions.DontRequireReceiver);
        }
    }
}
