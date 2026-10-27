using UnityEngine;

public class Snowbullet : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform nozzle;
    public Transform player;

    public void Bullet()
    {
        GameObject bullet = Instantiate(bulletPrefab, nozzle.position, Quaternion.identity);
        bullet.GetComponent<BulletMove>().SetTarget(player);
       
    }
}
