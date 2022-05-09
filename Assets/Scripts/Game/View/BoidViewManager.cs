using System.Collections.Generic;
using Tofunaut.TofuECS_Boids.Game.ECS;
using UnityEngine;
using UnityEngine.Pool;

namespace Tofunaut.TofuECS_Boids.Game.View
{
    public class BoidViewManager
    {
        public int NumBoidViews => _entityIdToBoid.Count;
        
        private readonly ObjectPool<GameObject> _boidViewPool;
        private Dictionary<int, GameObject> _entityIdToBoid;

        public BoidViewManager(GameObject boidViewPrefab)
        {
            _boidViewPool = new ObjectPool<GameObject>(
                () => GameObject.Instantiate(boidViewPrefab),
                boid => boid.SetActive(true), 
                boid => boid.SetActive(false), 
                boid => GameObject.Destroy(boid));
            _entityIdToBoid = new Dictionary<int, GameObject>();
        }

        public void CreateBoidView(int entityId)
        {
            var boid = _boidViewPool.Get();
            _entityIdToBoid.Add(entityId, boid);
        }

        public void DestroyBoidView(int entityId)
        {
            var boid = _entityIdToBoid[entityId];
            _entityIdToBoid.Remove(entityId);
            _boidViewPool.Release(boid);
        }

        public void DestroyAll()
        {
            _boidViewPool.Clear();
            _entityIdToBoid.Clear();
        }

        public void SyncBoidView(int entityId, in Boid boid)
        {
            var t = _entityIdToBoid[entityId].transform;
            t.position = new Vector3(boid.Position.X, boid.Position.Y);
            t.rotation = Quaternion.LookRotation(new Vector3(boid.Velocity.X, boid.Velocity.Y), Vector3.forward);
        }
    }
}