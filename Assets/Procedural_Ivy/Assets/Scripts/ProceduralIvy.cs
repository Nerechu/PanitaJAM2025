using System.Collections.Generic;
using UnityEngine;

public class ProceduralIvy : MonoBehaviour
{
    public float recycleInterval = 30;

    [Header("Branch Settings")]
    public int branches = 3;
    public int maxPointsForBranch = 20;
    public float segmentLength = .002f;
    public float branchRadius = 0.02f;

    [Header("Materials")]
    public Material branchMaterial;
    public Material leafMaterial;
    public Material flowerMaterial;

    [Header("Prefabs")]
    public Blossom leafPrefab;
    public Blossom flowerPrefab;

    [Header("Options")]
    public bool wantBlossoms;
    public LayerMask plantLayer;

    private int ivyCount = 0;

    public void createIvy(Vector3 point, Vector3 normal)
    {
        Vector3 tangent = findTangentFromArbitraryNormal(normal);
        GameObject ivy = new GameObject("Ivy " + ivyCount);
        ivy.transform.SetParent(transform);

        for (int i = 0; i < branches; i++)
        {
            Vector3 dir = Quaternion.AngleAxis(360 / branches * i + Random.Range(0, 360 / branches), normal) * tangent;
            List<IvyNode> nodes = createBranch(maxPointsForBranch, point, normal, dir);

            GameObject branch = new GameObject("Branch " + i);
            Branch b = branch.AddComponent<Branch>();

            if (!wantBlossoms)
            {
                b.init(nodes, branchRadius, branchMaterial);
            }
            else
            {
                b.init(nodes, branchRadius, branchMaterial, leafMaterial, leafPrefab, flowerMaterial, flowerPrefab, i == 0);
            }

            branch.transform.SetParent(ivy.transform);
        }

        ivyCount++;
    }

    public void combineAndClear()
    {
        MeshManager.instance.combineUnder(transform);

        foreach (Transform t in transform)
        {
            Destroy(t.gameObject);
        }
    }

    Vector3 findTangentFromArbitraryNormal(Vector3 normal)
    {
        Vector3 t1 = Vector3.Cross(normal, Vector3.forward);
        Vector3 t2 = Vector3.Cross(normal, Vector3.up);
        return (t1.magnitude > t2.magnitude) ? t1 : t2;
    }

    List<IvyNode> createBranch(int count, Vector3 pos, Vector3 normal, Vector3 dir)
    {
        List<IvyNode> nodes = new List<IvyNode>();

        if (count == maxPointsForBranch)
        {
            nodes.Add(new IvyNode(pos, normal));
            nodes.AddRange(createBranch(count - 1, pos, normal, dir));
            return nodes;
        }
        else if (count < maxPointsForBranch && count > 0)
        {
            if (count % 2 == 0)
            {
                dir = Quaternion.AngleAxis(Random.Range(-20.0f, 20.0f), normal) * dir;
            }

            RaycastHit hit;
            Vector3 p1 = pos + normal * segmentLength;

            if (Physics.Raycast(pos, normal, out hit, segmentLength))
            {
                p1 = hit.point;
            }

            if (Physics.Raycast(p1, dir, out hit, segmentLength))
            {
                Vector3 p2 = hit.point;
                nodes.Add(new IvyNode(p2, -dir));
                nodes.AddRange(createBranch(count - 1, p2, -dir, normal));
                return nodes;
            }
            else
            {
                Vector3 p2 = p1 + dir * segmentLength;

                if (Physics.Raycast(applyCorrection(p2, normal), -normal, out hit, segmentLength))
                {
                    Vector3 p3 = hit.point;
                    if (isOccluded(p3, pos, normal))
                    {
                        Vector3 middle = calculateMiddlePoint(p3, pos, (normal + dir) / 2);
                        Vector3 m0 = (pos + middle) / 2;
                        Vector3 m1 = (p3 + middle) / 2;

                        nodes.Add(new IvyNode(m0, normal));
                        nodes.Add(new IvyNode(m1, normal));
                        nodes.Add(new IvyNode(p3, normal));
                        nodes.AddRange(createBranch(count - 3, p3, normal, dir));
                        return nodes;
                    }

                    nodes.Add(new IvyNode(p3, normal));
                    nodes.AddRange(createBranch(count - 1, p3, normal, dir));
                    return nodes;
                }
                else
                {
                    Vector3 p3 = p2 - normal * segmentLength;

                    if (Physics.Raycast(applyCorrection(p3, normal), -normal, out hit, segmentLength))
                    {
                        Vector3 p4 = hit.point;
                        if (isOccluded(p4, pos, normal))
                        {
                            Vector3 middle = calculateMiddlePoint(p4, pos, (normal + dir) / 2);
                            Vector3 m0 = (pos + middle) / 2;
                            Vector3 m1 = (p4 + middle) / 2;

                            nodes.Add(new IvyNode(m0, normal));
                            nodes.Add(new IvyNode(m1, normal));
                            nodes.Add(new IvyNode(p4, normal));
                            nodes.AddRange(createBranch(count - 3, p4, normal, dir));
                            return nodes;
                        }

                        nodes.Add(new IvyNode(p4, normal));
                        nodes.AddRange(createBranch(count - 1, p4, normal, dir));
                        return nodes;
                    }
                    else
                    {
                        Vector3 p4 = p3 - normal * segmentLength;

                        if (isOccluded(p4, pos, normal))
                        {
                            Vector3 middle = calculateMiddlePoint(p4, pos, (normal + dir) / 2);
                            Vector3 m0 = (pos + middle) / 2;
                            Vector3 m1 = (p4 + middle) / 2;

                            nodes.Add(new IvyNode(m0, dir));
                            nodes.Add(new IvyNode(m1, dir));
                            nodes.Add(new IvyNode(p4, dir));
                            nodes.AddRange(createBranch(count - 3, p4, dir, -normal));
                            return nodes;
                        }

                        nodes.Add(new IvyNode(p4, dir));
                        nodes.AddRange(createBranch(count - 1, p4, dir, -normal));
                        return nodes;
                    }
                }
            }
        }

        return nodes;
    }

    bool isOccluded(Vector3 from, Vector3 to)
    {
        return Physics.Raycast(from, (to - from).normalized, (to - from).magnitude);
    }

    bool isOccluded(Vector3 from, Vector3 to, Vector3 normal)
    {
        return isOccluded(applyCorrection(from, normal), applyCorrection(to, normal));
    }

    Vector3 calculateMiddlePoint(Vector3 p0, Vector3 p1, Vector3 normal)
    {
        Vector3 middle = (p0 + p1) / 2;
        float distance = (p0 - p1).magnitude;
        return middle + normal * distance;
    }

    Vector3 applyCorrection(Vector3 p, Vector3 normal)
    {
        return p + normal * 0.01f;
    }
}
