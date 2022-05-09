using System;
using Tofunaut.TofuECS_Boids.Game.ECS;
using UnityEngine;

namespace Tofunaut.TofuECS_Boids.Game
{
    [CreateAssetMenu(menuName = "TofuECS_Boids/SimulationConfig")]
    public class SimulationConfig : ScriptableObject
    {
        public event Action Changed;
        
        public int InitialNumBoids;
        public float InitialBoidMinSpeed;
        public float InitialBoidMaxSpeed;
        public float Alignment;
        public float AlignmentRadius;
        public float Cohesion;
        public float CohesionRadius;
        public float Separation;
        public float SeparationRadius;
        public Vector2 WorldExtents;

        private void OnValidate()
        {
            Changed?.Invoke();
        }

        public void GetBoidConfig(ref BoidConfig config)
        {
            config.Alignment = Alignment;
            config.AlignmentRadius = AlignmentRadius;
            config.Cohesion = Cohesion;
            config.CohesionRadius = CohesionRadius;
            config.Separation = Separation;
            config.SeparationRadius = SeparationRadius;
            config.WorldExtents = new System.Numerics.Vector2(WorldExtents.x, WorldExtents.y);
            config.DeltaTime = Time.deltaTime;
        }
    }
}