// Bullet Spawner
using UnityEngine;


public class BulletSpawner : MonoBehaviour
{

    [Header("State")]
    [SerializeField] private bool isActive = true;

    enum SpawnerType { Straight, Spin }


    [Header("Bullet Attributes")]
    public GameObject bullet;
    public float bulletLife = 1f;
    public float speed = 1f;


    [Header("Spawner Attributes")]
    [SerializeField] private SpawnerType spawnerType;
    [SerializeField] private float firingRate = 1f;


    private GameObject spawnedBullet;
    private float timer = 0f;
    // Start is called before the first frame update

    // Update is called once per frame
    void Update()
    {
        if (!isActive) return;   // Stops the spawner if it's not active

        timer += Time.deltaTime;
        if (spawnerType == SpawnerType.Spin) transform.eulerAngles = new Vector3(0f, 0f, transform.eulerAngles.z + 1f);
        if (timer >= firingRate)
        {
            Fire();
            timer = 0;
        }
    }


    private void Fire()
    {
        if (bullet)
        {
            spawnedBullet = Instantiate(bullet, transform.position, Quaternion.identity);
            spawnedBullet.GetComponent<RoundRedBullet>().speed = speed;
            spawnedBullet.GetComponent<RoundRedBullet>().bulletLife = bulletLife;
            spawnedBullet.transform.rotation = transform.rotation;
        }
    }

    // 🔹 Public Control Method
    public void Activate()
    {
        isActive = true;

        if (!isActive)
        {
            timer = 0f; // Reset firing timer
            Debug.Log("Bullet Spawner Disabled");
        }
        else
        {
            Debug.Log("Bullet Spawner Enabled");
        }
    }

    public void Deactivate()
    {
        isActive = false;

        if (!isActive)
        {
            timer = 0f; // Reset firing timer
            Debug.Log("Bullet Spawner Disabled");
        }
        else
        {
            Debug.Log("Bullet Spawner Enabled");
        }
    }
}
