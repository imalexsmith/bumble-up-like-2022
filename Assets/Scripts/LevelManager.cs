using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using NightFramework;


public class LevelManager : MonoBehaviour
{
    // ==============================================================================
    public MinMaxFloat NextSpawnDelay = new MinMaxFloat(1f, 2.5f);
    public MinMaxInt NextSpawnCount = new MinMaxInt(1, 2);
    [Min(3)]
    public int MaxAliveStairs = 3;

    [Space]
    public Transform[] SpawnPoints;
    public Enemy[] EnemyTemplates;
    public Stairs StairsTemplate;

    protected float DespawnDistance => StairsTemplate.Size.z * (MaxAliveStairs / 2f);

    private Player _player;
    private List<Enemy> _aliveEnemies = new List<Enemy>();
    private LinkedList<Stairs> _aliveStairs = new LinkedList<Stairs>();
    private RandomisedSet<Transform> _pointsSpawn = new RandomisedSet<Transform>();
    private RandomisedSet<Enemy> _enemySpawn = new RandomisedSet<Enemy>();


    // ==============================================================================
    protected void Awake()
    {
        foreach (var point in SpawnPoints)
            _pointsSpawn.Values.Add(new RandomisedSetEntry<Transform>(point, false, 1f));

        foreach (var enenmy in EnemyTemplates)
            _enemySpawn.Values.Add(new RandomisedSetEntry<Enemy>(enenmy, false, 1f));

        foreach (var enemy in EnemyTemplates)
            enemy.gameObject.SetActive(false);

        StairsTemplate.gameObject.SetActive(false);
    }

    protected void Start()
    {
        _player = FindObjectOfType<Player>();
        _player.OnMoveForward += PlayerMoved;

        CheckStairs();
        NextSpawnEnemy();
    }

    protected void OnDestroy()
    {
        if (_player)
            _player.OnMoveForward -= PlayerMoved;
    }

    private void PlayerMoved()
    {
        foreach (var point in SpawnPoints)
            point.position += new Vector3(0f, _player.MoveDistance, _player.MoveDistance);

        CheckStairs();
    }

    private void CheckStairs()
    {
        if (_aliveStairs.Count > 0)
        {
            if (_player.transform.position.z - _aliveStairs.First.Value.transform.position.z > DespawnDistance)
            {
                Destroy(_aliveStairs.First.Value.gameObject);
                _aliveStairs.RemoveFirst();
            }
        }

        var offset = new Vector3(0f, StairsTemplate.Size.y, StairsTemplate.Size.z);
        var lastPos = _aliveStairs.Count > 0 ? _aliveStairs.Last.Value.transform.position : -offset;
        while (_aliveStairs.Count < MaxAliveStairs)
        {
            lastPos += offset;
            var s = Instantiate(StairsTemplate, lastPos, Quaternion.identity);
            s.gameObject.SetActive(true);
            _aliveStairs.AddLast(s);
        }
    }

    private void NextSpawnEnemy()
    {
        StartCoroutine(NextSpawnEnemyDelayed(NextSpawnDelay.RandomInRange()));
    }

    private IEnumerator NextSpawnEnemyDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);

        SpawnEnemy();
        NextSpawnEnemy();
    }

    private void SpawnEnemy()
    {
        var count = Mathf.Min(NextSpawnCount.RandomInRangeInclusive(), SpawnPoints.Length - 1);
        var points = _pointsSpawn.SelectRandomValues(RandomValueSelectionMode.SeveralWithNoRepeats, count);
        var enemies = _enemySpawn.SelectRandomValues(RandomValueSelectionMode.SeveralWithRepeats, count);

        for (int i = 0; i < count; i++)
        {
            var ne = Instantiate(enemies[i], points[i].position, enemies[i].transform.rotation);
            ne.gameObject.SetActive(true);
            ne.CachedMovementController.OnCompleteMoving += CheckEnemy;

            void CheckEnemy()
            {
                if (_player.transform.position.z - ne.transform.position.z > DespawnDistance)
                {
                    ne.CachedMovementController.OnCompleteMoving -= CheckEnemy;
                    Destroy(ne.gameObject);
                    _aliveEnemies.Remove(ne);
                }
            }

            _aliveEnemies.Add(ne);
        }
    }
}