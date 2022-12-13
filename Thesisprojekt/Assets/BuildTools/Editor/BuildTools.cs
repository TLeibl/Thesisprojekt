using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//script used to automatically build project for different platforms.
//It can be controlled via Tools - BuildTools window.
public class BuildTools : EditorWindow
{
    //Dictionary keeping targets that shall be built
    Dictionary<BuildTarget, bool> TargetsToBuild = new Dictionary<BuildTarget, bool>();
    //List keeping available build targets
    List<BuildTarget> AvailableTargets = new List<BuildTarget>();


    //Create editor window
    [MenuItem("Tools/Build Tools")]
    public static void OnShowTools()
    {
        EditorWindow.GetWindow<BuildTools>();
    }


    //Determine available build targets
    private void OnEnable()
    {
        AvailableTargets.Clear(); //clear list of available targets

        //go through all targets to build and check if they are available
        var buildTargets = System.Enum.GetValues(typeof(BuildTarget));
        
        foreach(var buildTargetValue in buildTargets)
        {
            BuildTarget target = (BuildTarget)buildTargetValue;

            //if current build target is not supported - skip it
            if (!BuildPipeline.IsBuildTargetSupported(GetTargetGroupForTarget(target), target))
                continue;

            //if available target found - add to AvailableTargets list
            AvailableTargets.Add(target);

            //if current build target is not in target list - add it
            if (!TargetsToBuild.ContainsKey(target))
                TargetsToBuild[target] = false;
        }

        //check if any targets have gone away
        if(TargetsToBuild.Count > AvailableTargets.Count)
        {
            //build list of removed targets
            List<BuildTarget> targetsToRemove = new List<BuildTarget>();

            foreach(var target in TargetsToBuild.Keys) 
            {
                if (!AvailableTargets.Contains(target))
                    targetsToRemove.Add(target);
            }

            //clean up removed targets
            foreach(var target in targetsToRemove)
            {
                TargetsToBuild.Remove(target);
            }
        }
    }


    //method used to determine whether a build target is supported
    private BuildTargetGroup GetTargetGroupForTarget(BuildTarget
        target) => target switch
    {
        //list of supported build targets
        BuildTarget.StandaloneWindows => BuildTargetGroup.Standalone, //Windows 
        BuildTarget.StandaloneWindows64 => BuildTargetGroup.Standalone,
        BuildTarget.StandaloneLinux64 => BuildTargetGroup.Standalone, //Linux
        BuildTarget.StandaloneOSX => BuildTargetGroup.Standalone, //OSX
        BuildTarget.iOS => BuildTargetGroup.iOS, //iOS
        BuildTarget.Android => BuildTargetGroup.Android, //Android / Oculus/Meta Quest 1/2
        _ => BuildTargetGroup.Unknown
    };


    //select platforms for build
    private void OnGUI()
    {
        //header
        GUILayout.Label("Platforms to build", EditorStyles.boldLabel);
    }
}
