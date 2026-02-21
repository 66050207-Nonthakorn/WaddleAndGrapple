using Microsoft.Xna.Framework;

namespace ComputerGameFinal.Engine.Components.Physics;

public class CircleCollider : Collider
{
    public float Radius { get; set; }

    public Vector2 GetCenter()
    {
        return base.GameObject.Position + Offset;
    }

    public override bool IsIntersect(Collider other)
    {
        return other.IsIntersect(this);
    }

    // Circle vs Circle collision
    public override bool IsIntersect(CircleCollider other)
    {
        float distanceSquared = Vector2.DistanceSquared(GetCenter(), other.GetCenter());
        float radiusSum = Radius + other.Radius;

        return distanceSquared <= radiusSum * radiusSum;
    }

    // Circle vs Box collision
    public override bool IsIntersect(BoxCollider other)
    {
        Vector2 circleCenter = GetCenter();
        Vector2 boxCenter = other.GetCenter();
        Vector2 boxHalfSize = new Vector2(other.Bounds.Width / 2, other.Bounds.Height / 2);

        Vector2 difference = circleCenter - boxCenter;
        Vector2 clamped = Vector2.Clamp(difference, -boxHalfSize, boxHalfSize);
        Vector2 closest = boxCenter + clamped;

        difference = closest - circleCenter;
        
        return difference.LengthSquared() <= Radius * Radius;
    }
}