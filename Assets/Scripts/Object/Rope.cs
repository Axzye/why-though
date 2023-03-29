using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;
    public float lineWidth = 0.075f;
    private int segments;
    private float ropeSegLen = 0.25f;

    private LineRenderer lineRenderer;
    private List<RopeSegment> ropeSegments = new();

    // Use this for initialization
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        Vector3 ropeStartPoint = startPoint.position;

        for (int i = 0; i < segments; i++)
        {
            ropeSegments.Add(new RopeSegment(ropeStartPoint));
            ropeStartPoint.y -= ropeSegLen;
        }
    }

    // Update is called once per frame
    void Update()
    {
        DrawRope();
    }

    private void FixedUpdate()
    {
        Simulate();
    }

    private void Simulate()
    {
        for (int i = 1; i < segments; i++)
        {
            RopeSegment segNew = ropeSegments[i];

            Vector2 velocity = segNew.posNew - segNew.posOld;
            velocity *= 0.98f;
            velocity -= 0.5f * Time.deltaTime * Vector2.down;

            segNew.posOld = segNew.posNew;
            segNew.posNew += velocity;

            ropeSegments[i] = segNew;
        }

        for (int i = 0; i < 50; i++)
        {
            ApplyConstraint();
        }
    }

    private void ApplyConstraint()
    {
        RopeSegment firstSegment = ropeSegments[0];
        RopeSegment endSegment = ropeSegments[^1];
        if (startPoint)
            firstSegment.posNew = startPoint.position;
        ropeSegments[0] = firstSegment;


        //Constrant to Second Point 
        if (endPoint)
            endSegment.posNew = endPoint.position;
        ropeSegments[^1] = endSegment;

        for (int i = 0; i < segments - 1; i++)
        {
            RopeSegment firstSeg = ropeSegments[i];
            RopeSegment secondSeg = ropeSegments[i + 1];

            float dist = (firstSeg.posNew - secondSeg.posNew).magnitude;
            float error = Mathf.Abs(dist - ropeSegLen);
            Vector2 changeDir = default;

            if (dist > ropeSegLen)
            {
                changeDir = (firstSeg.posNew - secondSeg.posNew).normalized;
            }
            else if (dist < ropeSegLen)
            {
                changeDir = (secondSeg.posNew - firstSeg.posNew).normalized;
            }

            Vector2 changeAmount = changeDir * error;
            if (i != 0)
            {
                firstSeg.posNew -= changeAmount * 0.5f;
                ropeSegments[i] = firstSeg;
                secondSeg.posNew += changeAmount * 0.5f;
                ropeSegments[i + 1] = secondSeg;
            }
            else
            {
                secondSeg.posNew += changeAmount;
                ropeSegments[i + 1] = secondSeg;
            }
        }
    }

    private void DrawRope()
    {
        float lineWidth = this.lineWidth;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;

        Vector3[] ropePositions = new Vector3[segments];
        for (int i = 0; i < segments; i++)
        {
            ropePositions[i] = ropeSegments[i].posNew;
        }

        lineRenderer.positionCount = ropePositions.Length;
        lineRenderer.SetPositions(ropePositions);
    }

    public struct RopeSegment
    {
        public Vector2 posNew;
        public Vector2 posOld;

        public RopeSegment(Vector2 pos)
        {
            posNew = pos;
            posOld = pos;
        }
    }
}
