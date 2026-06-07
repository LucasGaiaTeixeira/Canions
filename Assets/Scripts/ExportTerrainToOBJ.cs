using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;

public class ExportTerrainToOBJ : EditorWindow
{
    Terrain terrain;
    int resolution = 1;

    [MenuItem("Tools/Export Terrain to OBJ")]
    static void Init()
    {
        ExportTerrainToOBJ window = (ExportTerrainToOBJ)EditorWindow.GetWindow(typeof(ExportTerrainToOBJ));
        window.titleContent = new GUIContent("Export Terrain");
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("Terrain Exporter", EditorStyles.boldLabel);
        
        terrain = (Terrain)EditorGUILayout.ObjectField("Terrain", terrain, typeof(Terrain), true);
        
        EditorGUILayout.Space();
        GUILayout.Label("Resolution settings:", EditorStyles.label);
        resolution = EditorGUILayout.IntSlider("Vertex Step (1 = High Res, 2 = Medium, 4 = Low)", resolution, 1, 16);

        EditorGUILayout.Space();
        
        if (GUILayout.Button("Export to OBJ"))
        {
            if (terrain != null)
            {
                ExportToObj(terrain, resolution);
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "Please select a terrain in the scene first.", "OK");
            }
        }
    }

    static void ExportToObj(Terrain terrain, int vertexStep)
    {
        string savePath = EditorUtility.SaveFilePanel("Export Terrain as OBJ", "Assets", terrain.name + ".obj", "obj");
        if (string.IsNullOrEmpty(savePath)) return;

        EditorUtility.DisplayProgressBar("Exporting Terrain", "Generating mesh data...", 0.1f);

        try
        {
            TerrainData tData = terrain.terrainData;
            int w = tData.heightmapResolution;
            int h = tData.heightmapResolution;
            
            Vector3 meshScale = tData.size;
            meshScale = new Vector3(meshScale.x / (w - 1), meshScale.y, meshScale.z / (h - 1));

            Vector2 uvScale = new Vector2(1.0f / (w - 1), 1.0f / (h - 1));
            float[,] tDataHeights = tData.GetHeights(0, 0, w, h);

            int tRes = vertexStep;
            w = (w - 1) / tRes + 1;
            h = (h - 1) / tRes + 1;

            Vector3[] tVertices = new Vector3[w * h];
            Vector2[] tUV = new Vector2[w * h];
            int[] tPolys = new int[(w - 1) * (h - 1) * 6];

            EditorUtility.DisplayProgressBar("Exporting Terrain", "Calculating vertices and UVs...", 0.3f);

            // Build vertices and UVs
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    tVertices[y * w + x] = Vector3.Scale(meshScale, new Vector3(x * tRes, tDataHeights[y * tRes, x * tRes], y * tRes));
                    tUV[y * w + x] = Vector2.Scale(new Vector2(x * tRes, y * tRes), uvScale);
                }
            }

            EditorUtility.DisplayProgressBar("Exporting Terrain", "Calculating triangles...", 0.5f);

            // Build triangles
            int index = 0;
            for (int y = 0; y < h - 1; y++)
            {
                for (int x = 0; x < w - 1; x++)
                {
                    tPolys[index++] = (y * w) + x;
                    tPolys[index++] = ((y + 1) * w) + x;
                    tPolys[index++] = (y * w) + x + 1;

                    tPolys[index++] = ((y + 1) * w) + x;
                    tPolys[index++] = ((y + 1) * w) + x + 1;
                    tPolys[index++] = (y * w) + x + 1;
                }
            }

            EditorUtility.DisplayProgressBar("Exporting Terrain", "Writing to file...", 0.7f);

            // Write to file
            using (StreamWriter sw = new StreamWriter(savePath))
            {
                sw.WriteLine("# Unity Terrain to OBJ Export");
                
                // Write vertices (Flip X to match Unity's coordinate system to standard 3D software)
                for (int i = 0; i < tVertices.Length; i++)
                {
                    sw.WriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "v {0} {1} {2}", -tVertices[i].x, tVertices[i].y, tVertices[i].z));
                }

                // Write UVs
                for (int i = 0; i < tUV.Length; i++)
                {
                    sw.WriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "vt {0} {1}", tUV[i].x, tUV[i].y));
                }

                // Write faces
                for (int i = 0; i < tPolys.Length; i += 3)
                {
                    sw.WriteLine(string.Format("f {0}/{0} {1}/{1} {2}/{2}", 
                        tPolys[i] + 1, 
                        tPolys[i + 1] + 1, 
                        tPolys[i + 2] + 1));
                }
            }

            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Success", "Terrain exported successfully to:\n" + savePath, "OK");
        }
        catch (System.Exception e)
        {
            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("Error", "Failed to export terrain:\n" + e.Message, "OK");
        }
    }
}

