using System;
using Tofunaut.TofuECS;
using Tofunaut.TofuECS_Boids.Game.ECS;
using Tofunaut.TofuECS_Boids.Game.View;
using UnityEngine;
using Vector2 = System.Numerics.Vector2;

namespace Tofunaut.TofuECS_Boids.Game
{
    public class SimulationRunner : IDisposable
    {
        public int NumBoids => _boidViewManager.NumBoidViews;
        
        private readonly SimulationConfig _config;
        private readonly Simulation _simulation;
        private readonly BoidViewManager _boidViewManager;
        private SetBoidConfigInput _setBoidConfigInput;
        
        public SimulationRunner(SimulationConfig config, GameObject boidViewPrefab)
        {
            _config = config;
            _simulation = new Simulation(new UnityLogService(), new ISystem[]
            {
                new BoidSystem(),
            });
            _simulation.RegisterComponent<Boid>(_config.InitialNumBoids);

            _setBoidConfigInput = new SetBoidConfigInput
            {
                Config = new BoidConfig(),
            };
            _config.GetBoidConfig(ref _setBoidConfigInput.Config);
            _simulation.RegisterSingletonComponent(_setBoidConfigInput.Config);
            
            BoidSystem.BoidCreated += OnBoidCreated;
            
            _simulation.Initialize();

            _boidViewManager = new BoidViewManager(boidViewPrefab);

            for (var i = 0; i < _config.InitialNumBoids; i++)
            {
                _simulation.SystemEvent(new CreateBoidInput
                {
                    Position = new Vector2(
                        UnityEngine.Random.Range(-_config.WorldExtents.x, _config.WorldExtents.x),
                        UnityEngine.Random.Range(-_config.WorldExtents.y, _config.WorldExtents.y)),
                    Velocity = new Vector2(1, 0).RotatedByRadians(UnityEngine.Random.Range(0, ShapeMath2D.PI2)) 
                               * UnityEngine.Random.value * _config.MaxSpeed,
                });
            }
        }

        private void OnBoidCreated(int entityid, in Boid boid)
        {
            _boidViewManager.CreateBoidView(entityid);
            _boidViewManager.SyncBoidView(entityid, boid);
        }

        public void DoTick()
        {
            _config.GetBoidConfig(ref _setBoidConfigInput.Config);
            _simulation.SystemEvent(_setBoidConfigInput);
            _simulation.Tick();
            var buffer = _simulation.Buffer<Boid>();
            var i = 0;
            while(buffer.Next(ref i, out var entityId, out var boid))
                _boidViewManager.SyncBoidView(entityId, boid);
        }

        public void Dispose()
        {
            _simulation?.Dispose();
        }
    }
}