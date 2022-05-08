﻿using System.Numerics;

namespace Game.ECS
{
    public struct BoidConfig
    {
        public float Alignment;
        public float AlignmentRadius;
        public float Cohesion;
        public float CohesionRadius;
        public float Separation;
        public float SeparationRadius;
        public float DeltaTime;
        public Vector2 WorldExtents;
    }
}