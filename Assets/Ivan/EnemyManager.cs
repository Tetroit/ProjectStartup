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

    [ExecuteAlways]
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (Vector3 point in spawnPoints)
        {
            Gizmos.DrawSphere(point, 0.5f);
        }
    }

    private void Start()
    {
        SpawnEnemy(prefab);
        SpawnEnemy(prefab);
        SpawnEnemy(prefab);
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
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
        enemies.Add(instance);
    }
    public void PickEnemy()
    {
        picked.Clear();
        for (int i = 0; i < enemies.Count; i++)
        {
            Collider collider = enemies[i].GetComponent<Collider>();
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
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
}
