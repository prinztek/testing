using UnityEngine;

public class ProjectileLauncher : MonoBehaviour
{
    public GameObject projectilePrefab;    // Prefab of the projectile to be launched
    public float launchSpeed = 12f;        // Speed of the projectile
    public Transform launchPoint;          // Position to launch from
    private Move move;
    private void Awake()
    {
        move = GetComponentInParent<Move>();
    }
    public void FireProjectile()
    {
        if (projectilePrefab != null && launchPoint != null)
        {
            Debug.Log($"FacingRight: {move.FacingRight}");

            // Instantiate the projectile
            GameObject projectile = Instantiate(projectilePrefab, launchPoint.position, Quaternion.identity);

            // Flip direction based on character facing
            Vector2 direction = move.FacingRight ? Vector2.right : Vector2.left;

            // Flip projectile visual if needed
            if (!move.FacingRight)
            {
                Vector3 scale = projectile.transform.localScale;
                scale.x *= -1;
                projectile.transform.localScale = scale;
            }

            // Pass direction to the projectile
            Projectile proj = projectile.GetComponent<Projectile>();
            if (proj != null)
            {
                proj.Launch(direction * launchSpeed);
            }
        }
    }
}
