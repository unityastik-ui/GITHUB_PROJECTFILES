using UnityEngine;

public class Fireshoot : MonoBehaviour
{
    public GameObject fireBulletPrefab;
    public Transform nozzle;

    GameObject activeBullet;

    bool firePressed;

    void Update()
    {
        // PC FIRE
        if (Input.GetKeyDown(KeyCode.F))
        {
            Shoot();
        }

        // MOBILE FIRE
        if (firePressed)
        {
            Shoot();
            firePressed = false;
        }
    }

    void Shoot()
    {
        // if bullet still exists, don't shoot
        if (activeBullet != null)
            return;

        activeBullet =
            Instantiate(fireBulletPrefab, nozzle.position, Quaternion.identity);

        Sanbullet fb = activeBullet.GetComponent<Sanbullet>();
    }

    // MOBILE BUTTON
    public void FireButton()
    {
        firePressed = true;
    }
}