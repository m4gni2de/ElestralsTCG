using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SpriteExtensions 
{
    public static List<Vector2> SetOutline(this Sprite sp, float subDiv = 2f)
    {
        List<Vector2> points = new List<Vector2>();
        List<Vector2> simplifiedPoints = new List<Vector2>();
        List<Vector2> simplifiedOutline = new List<Vector2>();


        int pathCount = sp.GetPhysicsShapeCount();

        for (int i = 0; i < pathCount; i++)
        {
            sp.GetPhysicsShape(i, points);
            LineUtility.Simplify(points, .05f, simplifiedPoints);

        }

        float width = sp.rect.width;


        List<Vector2> outline = GenerateOutline(simplifiedPoints, subDiv);

        return outline;
    }

    /// <summary>
    /// Use this to set the outline of the sprite in relation to it's world position
    /// </summary>
    /// <param name="sp"></param>
    /// <param name="transform"></param>
    /// <param name="subDiv"></param>
    /// <returns></returns>

    public static List<Vector2> SetLocalOutline(this Sprite sp, Transform transform, float subDiv = 2f)
    {
        List<Vector2> points = new List<Vector2>();
        List<Vector2> simplifiedPoints = new List<Vector2>();
        List<Vector2> simplifiedOutline = new List<Vector2>();


        int pathCount = sp.GetPhysicsShapeCount();

        for (int i = 0; i < pathCount; i++)
        {
            sp.GetPhysicsShape(i, points);
            LineUtility.Simplify(points, .05f, simplifiedPoints);

        }

        float width = sp.rect.width;


        List<Vector2> outline = GenerateOutline(simplifiedPoints, subDiv);
        List<Vector2> worldOutline = new List<Vector2>();
        for (int i = 0; i < outline.Count; i++)
        {
            worldOutline.Add(transform.TransformPoint(outline[i]));
        }

        return worldOutline;
    }

    static List<Vector2> GenerateOutline(List<Vector2> vertsToCopy, float subDivides)
    {
        List<Vector2> returnList = new List<Vector2>();

        vertsToCopy.Add(vertsToCopy[0]);
        returnList.Add(vertsToCopy[0]);

        //get the average distance between the base outline
        float avgDistance = 0f;
        for (int i = 1; i < vertsToCopy.Count; i++)
        {
            avgDistance += Vector2.Distance(vertsToCopy[i], vertsToCopy[i - 1]);
        }

        avgDistance /= vertsToCopy.Count;

        for (int i = 1; i < vertsToCopy.Count; i++)
        {
            //gets the distance between any two given vertices. 
            float distanceBetweenVerts = Vector2.Distance(vertsToCopy[i], vertsToCopy[i - 1]);


            //gets the amount of vertices to put between 2 of the base verts. if the distance between them is more than the average, try and add more vertices. 
            //try and add less if the distance between them is less than the average.
            int verts = Mathf.RoundToInt((distanceBetweenVerts / avgDistance) * subDivides);

            float newLength = distanceBetweenVerts / (float)verts;

            for (int j = 1; j < verts; j++)
            {
                Vector2 newVert = Vector2.MoveTowards(vertsToCopy[i - 1], vertsToCopy[i], newLength * j);


                returnList.Add(newVert);

            }


            if (i < vertsToCopy.Count - 1)
                returnList.Add(vertsToCopy[i]);
        }

        //returnList.ConvexHull();
        //return returnList;

        //List<Vector2> toReturn = Converter.ConvexHull(returnList);
        List<Vector2> toReturn = ConvexHull(returnList);
        return toReturn;

    }


    public static List<Vector2> GenerateGridPoints(this SpriteRenderer sp, float  subdivLevel, List<Vector2> edgePoints)
    {
        List<Vector2> GridPoints = new List<Vector2>();
        float numberDivisions = 6;

        Bounds bounds = sp.bounds;

        float width = bounds.max.x - bounds.min.x;
        float height = bounds.max.y - bounds.min.y;

        float subdivWidth = width / (subdivLevel * numberDivisions);
        float subdivHeight = height / ((subdivLevel * numberDivisions));

        float averagedLength = (subdivWidth + subdivHeight) / 2;
        float widthHeight = (width + height) / 2;



        for (int i = 1; i < (subdivLevel * numberDivisions / widthHeight) * width; i++)
        {
            for (int j = 1; j < (subdivLevel * numberDivisions / widthHeight) * height; j++)
            {
                float xPos = (i * averagedLength) + bounds.min.x;
                float yPos = (j * averagedLength) + bounds.min.y;
                Vector2 t = new Vector2(xPos, yPos);
                if (t.IsPointInside(edgePoints))
                {
                    GridPoints.Add(t);
                }
            }
        }
        return GridPoints;
    }
    public static List<Vector2> GenerateGridPoints(Bounds bounds, float subdivLevel, List<Vector2> edgePoints)
    {
        List<Vector2> GridPoints = new List<Vector2>();
        float numberDivisions = 6;

        float width = bounds.max.x - bounds.min.x;
        float height = bounds.max.y - bounds.min.y;

        float subdivWidth = width / (subdivLevel * numberDivisions);
        float subdivHeight = height / ((subdivLevel * numberDivisions));

        float averagedLength = (subdivWidth + subdivHeight) / 2;
        float widthHeight = (width + height) / 2;



        for (int i = 1; i < (subdivLevel * numberDivisions / widthHeight) * width; i++)
        {
            for (int j = 1; j < (subdivLevel * numberDivisions / widthHeight) * height; j++)
            {
                float xPos = (i * averagedLength) + bounds.min.x;
                float yPos = (j * averagedLength) + bounds.min.y;
                Vector2 t = new Vector2(xPos, yPos);
                if (t.IsPointInside(edgePoints))
                {
                    GridPoints.Add(t);
                }
            }
        }
        return GridPoints;
    }

    public static List<Vector2> ConvexHull(List<Vector2> toOrder)
    {
        List<Vector2> newOrder = new List<Vector2>();
        List<Vector2> oldOrder = new List<Vector2>();

        oldOrder.AddRange(toOrder);


        Vector2 startVertex = SmallestX(oldOrder);

        Vector3 startPos = startVertex;
        newOrder.Add(startVertex);
        oldOrder.Remove(startVertex);

        Vector2 currentPoint = startVertex;

        List<Vector2> colinearPoints = new List<Vector2>();

        for (int i = 1; i < toOrder.Count; i++)
        {

            Vector2 closest = ClosestPoint(oldOrder, currentPoint);
            newOrder.Add(closest);
            oldOrder.Remove(closest);
            currentPoint = closest;


        }

        return newOrder;

    }


    public static Vector2 SmallestX(List<Vector2> toOrder)
    {
        //Step 1. Find the vertex with the smallest x coordinate
        //If several have the same x coordinate, find the one with the smallest y
        Vector2 startVertex = toOrder[0];

        Vector3 startPos = startVertex;

        for (int i = 1; i < toOrder.Count; i++)
        {
            Vector3 testPos = toOrder[i];

            //Because of precision issues, we use Mathf.Approximately to test if the x positions are the same
            if (testPos.x < startPos.x || (Mathf.Approximately(testPos.x, startPos.x) && testPos.y < startPos.y))
            {
                startVertex = toOrder[i];

                startPos = startVertex;
            }
        }

        return startPos;
    }

    public static Vector2 ClosestPoint(List<Vector2> points, Vector2 toCheck)
    {
        float distance = 5000f;
        Vector2 closest = toCheck;

        for (int i = 0; i < points.Count; i++)
        {
            float checkDistance = Vector2.SqrMagnitude(toCheck - points[i]);
            if (checkDistance < distance)
            {
                distance = checkDistance;
                closest = points[i];
            }
        }


        return closest;
    }

    public static bool IsPointInside(this Vector2 p, List<Vector2> edgePoints)
    {
        int numVerts = edgePoints.Count;
        Vector2 p0 = edgePoints[numVerts - 1];
        bool bYFlag0 = (p0.y >= p.y) ? true : false;


        bool bInside = false;
        for (int j = 0; j < numVerts; ++j)
        {
            Vector2 p1 = edgePoints[j];
            bool bYFlag1 = (p1.y >= p.y) ? true : false;
            if (bYFlag0 != bYFlag1)
            {
                if (((p1.y - p.y) * (p0.x - p1.x) >= (p1.x - p.x) * (p0.y - p1.y)) == bYFlag1)
                {
                    bInside = !bInside;
                }
            }

            // Move to the next pair of vertices, retaining info as possible.
            bYFlag0 = bYFlag1;
            p0 = p1;
        }

        return bInside;
    }

    public static bool IsPointInside(this SpriteRenderer sp,  Vector2 clickPos)
    {
        List<Vector2> edges = new List<Vector2>();
        edges = sp.sprite.SetLocalOutline(sp.transform);
        bool contains = false;
        if (clickPos.IsPointInside(edges))
        {
            contains = true;
        }
        return contains;
    }

}
