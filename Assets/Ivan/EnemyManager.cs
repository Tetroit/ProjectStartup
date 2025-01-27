using config;
using events;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
public interface IEnemyManager
{
    public void PickEnemy();
    public void PickAll();
}
public class EnemyManager : MonoBehaviour, IEnemyManager
{
    public List<Vector3> spawnPoints = new List<Vector3>();
    public List<Enemy> enemies = new List<Enemy>();

    public List<Enemy> picked;
    public Enemy prefab;

<<<<<<< Updated upstream
    [SerializeField]
    private EventsConfig eventConfig;
=======
    bool _allowInteraction = true;

    public bool allowInteraction => _allowInteraction;
>>>>>>> Stashed changes

    [ExecuteAlways]
    void OnDrawGizmos()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = Color.red;
        foreach (Vector3 point in spawnPoints)
        {
            Gizmos.DrawSphere(point, 0.5f);
        }
    }

    private void Awake()
    {
        eventConfig.OnCardPlayed += OnCardPlayed;
    }

    private void Start()
    {
        SpawnEnemy(prefab);
        SpawnEnemy(prefab);
        SpawnEnemy(prefab);
    }

    private void OnDestroy()
    {
        eventConfig.OnCardPlayed += OnCardPlayed;
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && _allowInteraction)
        {
            PickEnemy();
        }
    }
    public void SpawnEnemy(Enemy enemy)
    {
        if (spawnPoints.Count == 0)
        {
            Debug.LogError("No spawn points set up!");
            return;
        }
        if (enemies.Count >= spawnPoints.Count)
        {
            Debug.LogError("All spawn points are occupied!");
            return;
        }
        int id = enemies.Count;
        Vector3 nextSpawnpoint = spawnPoints[id];
        Enemy instance = Instantiate(enemy, nextSpawnpoint, Quaternion.identity, transform);
        instance.transform.localPosition = nextSpawnpoint;
        enemies.Add(instance);
    }
    public void PickEnemy()
    {
        //picked.Clear();
        for (int i = 0; i < enemies.Count; i++)
        {
            if(picked.Contains(enemies[i]))
            {
                //or we can do this instead
                //picked.Remove(enemies[i]);
                return;
            }

            Collider collider = enemies[i].GetComponent<Collider>();
            if (collider.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100.0f))
            {
                picked.Add(enemies[i]);
                return;
            }
        }
    }
    public void PickAll()
    {
        foreach (Enemy enemy in enemies)
        {
            picked.Add(enemy);
        }
    }
    public void OnCardPlayed(CardPlayed card)
    {
        foreach (Enemy enemy in picked)
        {
            card.card.CardEffect.ApplyEffect(enemy, 5);
        }
    }

    public void Enableinteraction()
    {
        _allowInteraction = true;
    }
    public void DisableInteraction()
    {
        picked.Clear();
        _allowInteraction = false;
    }
}