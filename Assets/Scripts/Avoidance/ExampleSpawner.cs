using UnityEngine;
using UnityEngine.AI;

namespace NavMeshAvoidance
{
    public sealed class ExampleSpawner : MonoBehaviour
    {
        [Header("Testing settings")]
        [SerializeField] GameObject agentPrefab;
        [SerializeField, Range(1,500)] int count = 16;
        
        [Header("Components links")]
        [SerializeField] Ordering ordering;
        [SerializeField] Avoidance avoidance;

        readonly IFormation spawnFormation = new SquareFormation();
        
        void Start()
        {
            //ordering.Avoidance = avoidance;
            
            var spawnPositions = spawnFormation.GetPositions(transform.position, count, 1);
            
            for (var i = 0; i < count; i++)
                DoSpawn(spawnPositions[i]);
        }

        void DoSpawn(Vector3 position)
        {
            var spawned = Instantiate(agentPrefab, position, Quaternion.identity);
            var agent = spawned.GetComponent<NavMeshAgent>();
                
            if(ordering != null)
                ordering.AddAgent(agent);
            avoidance.AddAgent(agent);
        }
    }
}