using config;
using events;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class EnemyManager : MonoBehaviour
{
    public List<Vector3> spawnPoints = new List<Vector3>();
    public List<Enemy> enemies = new List<Enemy>();

    //we attach a pin for each pair of enemy and card
    [HideInInspector]
    public Dictionary<(Enemy, CardWrapper), GameObject> pins = new Dictionary<(Enemy, CardWrapper), GameObject>();

    public GameObject pinPrefab;

    public Enemy prefab;

    [SerializeField]
    private EventsConfig eventConfig;
    bool _allowInteraction = true;

    public bool allowInteraction => _allowInteraction;

    public UnityEvent<Enemy> OnEnemySelected;
    public UnityEvent<Enemy> OnEnemyDeselected;
    public UnityAction OnRequiredSelected;

    [SerializeField]
    CardWrapper cardContext;

    public int targetCount = 2;

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
        eventConfig.OnCardPlayed -= OnCardPlayed;
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && _allowInteraction)
        {
            EnemyClick();
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
    public void EnemyClick()
    {
        if (cardContext == null)
            return;
        //DeselectAll();
        for (int i = 0; i < enemies.Count; i++)
        {
            Collider collider = enemies[i].GetComponent<Collider>();
            if (collider.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100.0f))
            {
                if (pins.Keys.Contains((enemies[i], cardContext)))
                {
                    DeselectEnemy(enemies[i], cardContext);
                    return;
                }
                else
                {
                    if (GetConnections(cardContext).Count() < targetCount)
                    {
                        PickEnemy(enemies[i]);
                        return;
                    }
                }
            }
        }
    }
    
    public IEnumerable<Enemy> GetConnections(CardWrapper card)
    {
        List<Enemy> connections = new List<Enemy>();
        foreach (var item in pins)
        {
            if (item.Key.Item2 == card)
                connections.Add(item.Key.Item1);
        }
        return connections;
    }
    public void PickEnemy(Enemy enemy)
    {
        OnEnemySelected.Invoke(enemy);
        DrawPin(enemy, cardContext);
    }
    public void PickAll()
    {
        foreach (Enemy enemy in enemies)
        {
            DrawPin(enemy, cardContext);
            OnEnemySelected.Invoke(enemy);
        }
    }
    public void DeselectEnemy(Enemy enemy)
    {
        OnEnemyDeselected.Invoke(enemy);
        RemovePin(enemy);
    }
    public void DeselectEnemy(Enemy enemy, CardWrapper card)
    {
        OnEnemyDeselected.Invoke(enemy);
        RemovePin(enemy, card);
    }


    public void DeselectAll()
    {
        foreach (Enemy item in GetConnections(cardContext))
            OnEnemyDeselected.Invoke(item);
        foreach (GameObject pin in pins.Values)
            Destroy(pin);
        pins.Clear();
    }
    public void OnCardPlayed(CardPlayed card)
    {
        cardContext = card.card;
        foreach (Enemy enemy in GetConnections(card.card))
        {
            card.card.CardEffect.ApplyEffect(enemy, 10);
        }
    }

    public void DrawPin(Enemy enemy, CardWrapper card)
    {
        GameObject newPin = Instantiate(pinPrefab);
        newPin.GetComponent<PinObject>().SetPositions(enemy.transform.position, card.transform.position);
        pins.Add((enemy, card), newPin);

    }
    public void RemovePin(Enemy enemy)
    {
        List<(Enemy, CardWrapper)> enemyPins = new List<(Enemy, CardWrapper)>();
        foreach (var pin in pins)
        {
            if (pin.Key.Item1 == enemy)
            {
                enemyPins.Add(pin.Key);
            }
        }
        foreach (var pin in enemyPins)
        {
            Destroy(pins[pin]);
            pins.Remove(pin);
        }
    }
    public void RemovePin(Enemy enemy, CardWrapper card)
    {
        GameObject pin = pins[(enemy, card)];
        pins.Remove((enemy, card));
        Destroy(pin);
    }
    public void EnableInteraction()
    {
        _allowInteraction = true;
    }
    public void DisableInteraction()
    {
        _allowInteraction = false;
    }
}