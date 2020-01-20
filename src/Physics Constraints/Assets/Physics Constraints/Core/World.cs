﻿/******************************************************************************/
/*
  Project - Physics Constraints
            https://github.com/TheAllenChou/unity-physics-constraints
  
  Author  - Ming-Lun "Allen" Chou
  Web     - http://AllenChou.net
  Twitter - @TheAllenChou
*/
/******************************************************************************/

using System.Collections.Generic;
using UnityEngine;

namespace PhysicsConstraints
{
  public class World : MonoBehaviour
  {
    private static int s_velocityIterations = 10;
    public static int VelocityIterations
    {
      get { return s_velocityIterations; }
      set { s_velocityIterations = Mathf.Max(1, value); }
    }

    public static Vector3 Gravity = Vector3.zero;

    private static HashSet<Constraint> s_constraints;
    public static void Register(Constraint c)
    {
      ValidateWorld();
      s_constraints.Add(c);
    }
    public static void Unregister(Constraint c)
    {
      if (s_constraints == null)
        return;

      s_constraints.Remove(c);
    }

    private static HashSet<Body> s_bodies;
    public static void Register(Body b)
    {
      ValidateWorld();
      s_bodies.Add(b);
    }
    public static void Unregister(Body b)
    {
      if (s_bodies == null)
        return;

      s_bodies.Remove(b);
    }

    private static GameObject s_world;
    private static void ValidateWorld()
    {
      
      if (s_world != null)
        return;

      s_constraints = new HashSet<Constraint>();
      s_bodies = new HashSet<Body>();

      s_world = new GameObject("World (Physics Constraints)");
      s_world.AddComponent<World>();
    }

    private void FixedUpdate()
    {
      if (s_world != gameObject)
      {
        Destroy(gameObject);
        return;
      }

      float dt = Mathf.Max(ConstraintUtil.Epsilon, Time.fixedDeltaTime);
      Step(Time.fixedDeltaTime);
    }

    public static void Step(float dt)
    {
      Vector3 gravityImpulse = Gravity * dt;
      foreach (var body in s_bodies)
      {
        body.LinearVelocity += gravityImpulse;
      }

      foreach (var constraint in s_constraints)
      {
        constraint.InitVelocityConstraint(dt);
      }

      for (int i = 0; i < s_velocityIterations; ++i)
      {
        foreach (var constraint in s_constraints)
        {
          constraint.SolveVelocityConstraint(dt);
        }
      }

      foreach (var body in s_bodies)
      {
        body.Integrate(dt);
      }
    }
  }
}