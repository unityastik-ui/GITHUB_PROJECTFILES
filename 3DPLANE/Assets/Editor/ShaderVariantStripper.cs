using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public sealed class ShaderVariantStripper : IPreprocessShaders
{
    private static readonly ShaderKeyword[] CascadeKeywords =
    {
        new ShaderKeyword("SHADOWS_CASCADE"),
        new ShaderKeyword("_MAIN_LIGHT_SHADOWS_CASCADE")
    };

    private static readonly ShaderKeyword[] SoftShadowKeywords =
    {
        new ShaderKeyword("SHADOWS_SOFT"),
        new ShaderKeyword("_SHADOWS_SOFT"),
        new ShaderKeyword("_SHADOWS_SOFT_LOW"),
        new ShaderKeyword("_SHADOWS_SOFT_MEDIUM"),
        new ShaderKeyword("_SHADOWS_SOFT_HIGH")
    };

    public int callbackOrder => 0;

    public void OnProcessShader(
        Shader shader,
        ShaderSnippetData snippet,
        IList<ShaderCompilerData> data)
    {
        if (shader == null || !IsTargetShader(shader.name))
        {
            return;
        }

        bool stripCascades = ShouldStripCascadeVariants();
        bool stripSoftShadows = ShouldStripSoftShadowVariants();

        for (int i = data.Count - 1; i >= 0; i--)
        {
            ShaderCompilerData compilerData = data[i];
            if (HasUnsupportedKeyword(compilerData) ||
                (stripCascades && HasAnyKeyword(compilerData.shaderKeywordSet, CascadeKeywords)) ||
                (stripSoftShadows && HasAnyKeyword(compilerData.shaderKeywordSet, SoftShadowKeywords)))
            {
                data.RemoveAt(i);
            }
        }
    }

    [MenuItem("Tools/Sky Runner/Inspect Graphics Settings")]
    private static void InspectGraphicsSettings()
    {
        const string graphicsSettingsPath = "ProjectSettings/GraphicsSettings.asset";
        Object graphicsSettingsAsset = AssetDatabase.LoadAssetAtPath<Object>(graphicsSettingsPath);
        if (graphicsSettingsAsset == null)
        {
            Debug.LogError($"Could not load Graphics Settings at '{graphicsSettingsPath}'.");
            return;
        }

        SerializedObject serializedGraphicsSettings = new SerializedObject(graphicsSettingsAsset);
        SerializedProperty alwaysIncludedShaders =
            serializedGraphicsSettings.FindProperty("m_AlwaysIncludedShaders");
        if (alwaysIncludedShaders == null || !alwaysIncludedShaders.isArray)
        {
            Debug.LogError(
                $"Graphics Settings at '{graphicsSettingsPath}' does not contain " +
                "the expected m_AlwaysIncludedShaders array.");
            return;
        }

        for (int i = 0; i < alwaysIncludedShaders.arraySize; i++)
        {
            Object shaderObject = alwaysIncludedShaders.GetArrayElementAtIndex(i).objectReferenceValue;
            if (shaderObject is Shader shader)
            {
                Debug.Log($"Always Included Shader: {shader.name}", shader);
            }
            else
            {
                Debug.LogWarning($"Always Included Shaders entry {i} is empty or is not a shader.");
            }
        }

        Debug.Log(
            $"Always Included Shaders has {alwaysIncludedShaders.arraySize} registered entries. " +
            "Per-shader variant counts are not exposed by the Shader API.");
    }

    private static bool IsTargetShader(string shaderName)
    {
        return shaderName == "Standard" ||
               shaderName.StartsWith("Standard (", System.StringComparison.Ordinal) ||
               shaderName.StartsWith("UI/", System.StringComparison.Ordinal) ||
               shaderName.StartsWith("Universal Render Pipeline/", System.StringComparison.Ordinal);
    }

    private static bool HasUnsupportedKeyword(ShaderCompilerData compilerData)
    {
        return compilerData.shaderKeywordSet.IsEnabled(new ShaderKeyword("STEREO_INSTANCING_ON")) ||
               compilerData.shaderKeywordSet.IsEnabled(new ShaderKeyword("STEREO_MULTIVIEW_ON")) ||
               compilerData.shaderKeywordSet.IsEnabled(new ShaderKeyword("STEREO_CUBEMAP_RENDER_ON")) ||
               compilerData.shaderKeywordSet.IsEnabled(new ShaderKeyword("DYNAMICLIGHTMAP_ON")) ||
               compilerData.shaderKeywordSet.IsEnabled(new ShaderKeyword("DIRLIGHTMAP_COMBINED")) ||
               compilerData.shaderKeywordSet.IsEnabled(new ShaderKeyword("FOG_EXP")) ||
               compilerData.shaderKeywordSet.IsEnabled(new ShaderKeyword("FOG_EXP2"));
    }

    private static bool HasAnyKeyword(ShaderKeywordSet keywords, ShaderKeyword[] candidates)
    {
        foreach (ShaderKeyword keyword in candidates)
        {
            if (keywords.IsEnabled(keyword))
            {
                return true;
            }
        }

        return false;
    }

    private static bool ShouldStripCascadeVariants()
    {
        if (GraphicsSettings.currentRenderPipeline is UniversalRenderPipelineAsset urpAsset)
        {
            return urpAsset.shadowCascadeCount <= 1;
        }

        return QualitySettings.shadowCascades <= 1;
    }

    private static bool ShouldStripSoftShadowVariants()
    {
        if (GraphicsSettings.currentRenderPipeline is UniversalRenderPipelineAsset urpAsset)
        {
            return !urpAsset.supportsSoftShadows;
        }

        return QualitySettings.shadows != UnityEngine.ShadowQuality.All;
    }
}
