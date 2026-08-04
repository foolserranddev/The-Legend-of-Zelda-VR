using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Concentrates the legacy project's real-time shadow budget near the player.
/// Its original Fantastic preset used 150m and four cascades, which produces
/// poor near-field precision and excessive cost on standalone Quest hardware.
/// </summary>
public static class QuestShadowCompatibility
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
        ApplyForScene(SceneManager.GetActiveScene());
#endif
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplyForScene(scene);
    }

    private static void ApplyForScene(Scene scene)
    {
        if (scene.name.Contains("Dungeon"))
        {
            // Preserve the lighting profile around which the enclosed rooms,
            // local lights, and DungeonRoom light activation were authored.
            QualitySettings.shadowDistance = 150f;
            QualitySettings.shadowCascades = 4;
            QualitySettings.shadowResolution = ShadowResolution.High;
            QualitySettings.pixelLightCount = 4;
            return;
        }

        // The overworld uses a directional sun and benefits from concentrating
        // its shadow budget close to the player.
        QualitySettings.shadowDistance = 35f;
        QualitySettings.shadowCascades = 2;
        QualitySettings.shadowResolution = ShadowResolution.High;
        QualitySettings.pixelLightCount = 2;

        Light[] lights = Object.FindObjectsOfType<Light>(true);
        foreach (Light light in lights)
        {
            if (light.shadows == LightShadows.None) continue;

            // Point and spot shadow maps are disproportionately expensive on
            // standalone Quest. Dungeon rooms already limit active lights to
            // the current room, so retain their illumination but not their
            // multi-pass real-time shadow maps.
            if (light.type != LightType.Directional)
            {
                light.shadows = LightShadows.None;
                continue;
            }

            light.shadowBias = Mathf.Max(light.shadowBias, 0.08f);
            light.shadowNormalBias = Mathf.Max(light.shadowNormalBias, 0.4f);
        }
    }
}
