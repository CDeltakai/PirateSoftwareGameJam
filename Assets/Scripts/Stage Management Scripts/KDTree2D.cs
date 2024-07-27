using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KDTree2D
{
    private class KDNode
    {
        public Vector3 point;
        public KDNode left;
        public KDNode right;

        public KDNode(Vector3 point)
        {
            this.point = point;
            left = null;
            right = null;
        }
    }

    private KDNode root;
    private int k = 2;

    public KDTree2D(List<Vector3> points)
    {
        root = BuildTree(points, 0);
    }

    private KDNode BuildTree(List<Vector3> points, int depth)
    {
        if (points.Count == 0)
            return null;

        int axis = depth % k;
        points.Sort((a, b) => a[axis].CompareTo(b[axis]));

        int medianIndex = points.Count / 2;
        KDNode node = new KDNode(points[medianIndex])
        {
            left = BuildTree(points.GetRange(0, medianIndex), depth + 1),
            right = BuildTree(points.GetRange(medianIndex + 1, points.Count - (medianIndex + 1)), depth + 1)
        };

        return node;
    }

/// <summary>
/// Finds all points within a given radius of the target point.
/// </summary>
/// <param name="target"></param>
/// <param name="radius"></param>
/// <returns></returns>
    public Vector3[] FindNearby(Vector3 target, float radius)
    {
        List<Vector3> points = new List<Vector3>();
        FindNearby(root, target, 0, radius, points);
        return points.ToArray();
    }

/// <summary>
/// Finds all points within a given radius of the target point.
/// </summary>
/// <param name="root"></param>
/// <param name="target"></param>
/// <param name="v"></param>
/// <param name="radius"></param>
/// <param name="points"></param>
    private void FindNearby(KDNode root, Vector3 target, int v, float radius, List<Vector3> points)
    {
        if (root == null)
            return;

        int axis = v % k;
        float distance = Vector3.Distance(new Vector3(target.x, target.y, 0), new Vector3(root.point.x, root.point.y, 0));

        if (distance <= radius)
        {
            points.Add(root.point);
        }

        if (target[axis] - radius < root.point[axis])
        {
            FindNearby(root.left, target, v + 1, radius, points);
        }

        if (target[axis] + radius > root.point[axis])
        {
            FindNearby(root.right, target, v + 1, radius, points);
        }
    }

    public Vector3 FindNearest(Vector3 target)
    {
        return FindNearest(root, target, 0).point;
    }

    private KDNode FindNearest(KDNode node, Vector3 target, int depth)
    {
        if (node == null)
            return null;

        KDNode nextBranch = null;
        KDNode oppositeBranch = null;
        int axis = depth % k;

        if (target[axis] < node.point[axis])
        {
            nextBranch = node.left;
            oppositeBranch = node.right;
        }
        else
        {
            nextBranch = node.right;
            oppositeBranch = node.left;
        }

        KDNode best = CloserDistance(target, FindNearest(nextBranch, target, depth + 1), node);

        if (Vector3.Distance(new Vector3(target.x, target.y, 0), new Vector3(best.point.x, best.point.y, 0)) > Mathf.Abs(target[axis] - node.point[axis]))
        {
            best = CloserDistance(target, FindNearest(oppositeBranch, target, depth + 1), best);
        }

        return best;
    }

    private KDNode CloserDistance(Vector3 target, KDNode a, KDNode b)
    {
        if (a == null) return b;
        if (b == null) return a;

        float distanceA = Vector3.Distance(new Vector3(target.x, target.y, 0), new Vector3(a.point.x, a.point.y, 0));
        float distanceB = Vector3.Distance(new Vector3(target.x, target.y, 0), new Vector3(b.point.x, b.point.y, 0));

        return distanceA < distanceB ? a : b;
    }
}
