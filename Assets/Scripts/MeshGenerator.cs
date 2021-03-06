﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{
    public static MeshData GenerateTerrainMesh(float[,] heightMap, float heightMultiplier, AnimationCurve heightCurve, int levelOfDetail)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);
        //offsets used to place in the center of the map
        float topLeftX = (width - 1) / -2f;
        float topLeftZ = (height - 1) / 2f;

        int meshSimplificationIncrement = (levelOfDetail == 0) ? 1 :levelOfDetail * 2;
        int verticesPerLine = (width - 1) / meshSimplificationIncrement + 1;

        MeshData meshData = new MeshData(verticesPerLine, verticesPerLine);
        int vertexIndex = 0;

        for(int y=0; y<height; y+= meshSimplificationIncrement)
        {
            for(int x=0; x<width; x+=meshSimplificationIncrement)
            {
                //offsets used to place in the center of the map
                meshData.vertices[vertexIndex] = new Vector3(topLeftX+x,heightCurve.Evaluate(heightMap[x, y]) * heightMultiplier, topLeftZ-y);
                meshData.uvs[vertexIndex] = new Vector2(x / (float)width, y / (float)height);
                //no triangles to find on the right and bottom edges

                if (x<width-1 && y < height - 1)
                {
                    /*
                    yay ascii art!
                    i-------i+1
                    |\\      |
                    | \\     |
                    |  \\    |
                    |   \\   |
                    |    \\  |
                    |     \\ |
                    |      \\|
                    i+w -----i+w+1

                    */
                    meshData.AddTriangle(vertexIndex, vertexIndex + verticesPerLine + 1, vertexIndex + verticesPerLine);
                    meshData.AddTriangle(vertexIndex + verticesPerLine + 1, vertexIndex, vertexIndex + 1);
                }

                vertexIndex++;
            }
        }
        return meshData;
    }

 
}
public class MeshData
{
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uvs;

    int triangleIndex;
    public MeshData(int meshWidth, int meshHeight)
    {
        //total number of vertices being generated
        vertices = new Vector3[meshWidth * meshHeight];
        //need one for each vertex (where is the vertex in relation to the rest of the map as percentage 
        uvs = new Vector2[meshWidth * meshHeight];
        //total number of triangles possible to be created with the given number of vertices
        triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
    }
    //Adds a complete set of triangle vertices and then updates the index to the next empty space
    public void AddTriangle(int a, int b, int c)
    {
        triangles[triangleIndex] = a;
        triangles[triangleIndex + 1] = b;
        triangles[triangleIndex + 2] = c;

        triangleIndex += 3;
    }

    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        return mesh;
    }


}
