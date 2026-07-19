using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    [Header("Target Prefab Settings")]
    [SerializeField] private AimTarget _targetPrefab;
    [SerializeField] private int _poolSize = 20;

    [Tooltip("How many targets should display at same time")]
    [SerializeField] private int _concurrentTargets = 3;

    [Header("Target Spawn Area Settings")]
    [SerializeField] private Vector3 _spawnAreaCenter = new Vector3(0f, 2f, 10f);
    [SerializeField] private Vector3 _spawnAreaSize = new Vector3(5f, 10f, 0f);

    [Header("Target Explostion FX")]
    [SerializeField] private TargetExplosionFX _explosionFxPrefab;

    private ComponentPool<AimTarget> _targetPool;
    private ComponentPool<TargetExplosionFX> _targetExplosionFxPool;

    private void Awake()
    {
        if (_targetPrefab != null)
        {
            _targetPool = new ComponentPool<AimTarget>(_targetPrefab,_poolSize);
        }
        if (_explosionFxPrefab != null)
        {
            _targetExplosionFxPool = new ComponentPool<TargetExplosionFX>(_explosionFxPrefab,_poolSize);
        }
    }
    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnRoundStart += HandleRoundStart;
            GameManager.Instance.OnRoundEnd += HandleRoundEnd;
        }
    }
    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnRoundStart -= HandleRoundStart;
            GameManager.Instance.OnRoundEnd -= HandleRoundEnd;
        }
    }

    private void HandleRoundStart() 
    {
        for (int i = 0; i < _concurrentTargets; i++)
        {
            SpawnTarget();
        }
    }
    private void HandleRoundEnd() { }
    private void SpawnTarget()
    {
        if (_targetPool == null) return;
        
        Vector3 randomPosition = new Vector3(Random.Range(-_spawnAreaSize.x/2,_spawnAreaSize.x/2),
                                             Random.Range(-_spawnAreaSize.y/2,_spawnAreaSize.y/2),
                                             Random.Range(-_spawnAreaSize.z/2,_spawnAreaSize.z/2)) + _spawnAreaCenter;
        AimTarget activeTarget = _targetPool.GetInstanceFromPool(randomPosition,Quaternion.identity);

        activeTarget.Initialize((targetInstance) =>
        {
            Vector3 deathPosition = targetInstance.transform.position;

            _targetPool.ReturnInstanceToPool(targetInstance); //returning the dead into pool

            PlayTargetExplosionFx(deathPosition);

            if (GameManager.Instance.IsRoundActive) 
            { 
                SpawnTarget(); 
            }
        });
    }
    private void PlayTargetExplosionFx(Vector3 deathPosition)
    {
        if (_targetExplosionFxPool == null) return;


        TargetExplosionFX activeExplosionFx = _targetExplosionFxPool.GetInstanceFromPool(deathPosition, Quaternion.identity);
        activeExplosionFx.InitializeExplosion((explosionFxInstance) =>
        {
            _targetExplosionFxPool.ReturnInstanceToPool(explosionFxInstance);
        } );
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(_spawnAreaCenter,_spawnAreaSize);
    }
}
