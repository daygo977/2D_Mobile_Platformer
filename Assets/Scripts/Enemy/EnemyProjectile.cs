using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [Header("Projectile")]
    [SerializeField] private float speed = 6f;
    [SerializeField] private float maxTravelDistance = 8f;
    [SerializeField] private int damage = 1;

    private Vector2 direction;
    private Vector3 startPosition;

    //Set direction when projectile spawns
    public void Initialize(Vector2 fireDirection)
    {
        direction = fireDirection.normalized;
        startPosition = transform.position;
    }

    private void Update()
    {
        //Move forward each frame
        transform.position += (Vector3)(direction * speed * Time.deltaTime);

        //Destroy if it travels far (max range)
        float traveledDistance = Vector3.Distance(startPosition, transform.position);
        if (traveledDistance >= maxTravelDistance)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //If it hits player, deal damage, then destroy projectile
        if (other.CompareTag("Player"))
        {
            PlayerHealth health = other.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
        //Destroy if it hits other solid objects
        else if (!other.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}