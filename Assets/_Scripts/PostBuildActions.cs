using System.IO;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
#endif

using UnityEngine;

public class PostBuildActions : MonoBehaviour 
{
    #if UNITY_EDITOR
    [PostProcessBuild(1)]
	public static void DestoryStreamingAssets(BuildTarget target, string buildPath)
    {
        if (target == BuildTarget.StandaloneWindows ||
            target == BuildTarget.StandaloneWindows64 ||
            target == BuildTarget.StandaloneOSXIntel ||
            target == BuildTarget.StandaloneOSXIntel64 ||
            target == BuildTarget.StandaloneOSXUniversal)
        {
            string buildName = Path.GetFileNameWithoutExtension(buildPath);
            string buildFolder = Path.GetDirectoryName(buildPath);
            string buildDataFolder = Path.Combine(buildFolder, $"{buildName}_Data");
            string buildStreamingAssetsFolder = Path.Combine(buildDataFolder, "StreamingAssets");

            if (Directory.Exists(buildStreamingAssetsFolder) == true)
            {
                foreach (string file in Directory.GetFiles(buildStreamingAssetsFolder))
                {
                    File.Delete(file);
                }

                Debug.Log("[POST-BUILD] All of the files within the StreamingAssets folder have been deleted!");
            }
        }
    }
    #endif
}
