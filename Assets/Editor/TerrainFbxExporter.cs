#if UNITY_EDITOR
using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class TerrainFbxExporter
{
    public static bool TryExportGameObjectToFbx(string path, GameObject source)
    {
        if (source == null)
        {
            Debug.LogError("FBX export failed: source GameObject is null.");
            return false;
        }

        Type exporterType = GetModelExporterType();
        if (exporterType == null)
        {
            Debug.LogError("FBX export failed: Unity FBX Exporter package is not installed.");
            return false;
        }

        MethodInfo exportMethod = exporterType.GetMethod(
            "ExportObject",
            BindingFlags.Public | BindingFlags.Static,
            null,
            new Type[] { typeof(string), typeof(UnityEngine.Object) },
            null);

        if (exportMethod == null)
        {
            Debug.LogError("FBX export failed: could not find ExportObject method on ModelExporter.");
            return false;
        }

        exportMethod.Invoke(null, new object[] { path, source });
        AssetDatabase.Refresh();
        return true;
    }

    private static Type GetModelExporterType()
    {
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type type = assembly.GetType("UnityEditor.Formats.Fbx.Exporter.ModelExporter");
            if (type != null)
                return type;
        }

        return null;
    }
}
#endif
