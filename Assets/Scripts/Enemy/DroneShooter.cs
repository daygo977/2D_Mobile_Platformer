using UnityEngine;

public class DroneShooter : MonoBehaviour
{
    [Header("Projectile")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;

    [Header("Firing")]
    //Direction drone will shoot in
    [SerializeField] private Vector2 fireDirection = Vector2.right;
    //Shoots per second
    [SerializeField] private float shotsPerSecond = 1f;

    private float shotTimer;

    private void Update()
    {
        //Count up over time
        shotTimer += Time.deltaTime;

        //Fire when enough time passed
        if (shotTimer >= 1f / shotsPerSecond)
        {
            Fire();
            shotTimer = 0f;
        }
    }

    private void Fire()
    {
        //Do nothing if setup is not complete
        if (projectilePrefab == null || firePoint == null)
        {
            return;
        }
        //Spawn projectile at fire point
        GameObject projectileObject = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        //Set direction of movement
        EnemyProjectile projectile = projectileObject.GetComponent<EnemyProjectile>();
        if (projectile != null)
        {
            projectile.Initialize(fireDirection);
        }
    }
}