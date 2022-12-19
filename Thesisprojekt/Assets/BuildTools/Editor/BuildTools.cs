using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using Unity.EditorCoroutines.Editor;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

//script used to automatically build project for different platforms.
//It can be controlled via Tools - BuildTools window.
public class BuildTools : EditorWindow
{
    //Dictionary keeping targets that shall be built
    Dictionary<BuildTarget, bool> TargetsToBuild = new Dictionary<BuildTarget, bool>();
    //List keeping available build targets
    List<BuildTarget> AvailableTargets = new List<BuildTarget>();

    //strings used for scene enabling/disabling
    private string mainFolder = "Assets/Application/Scenes/";
    private string sceneType = ".unity";


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

        //display build targets
        int numEnabled = 0; //how many targets are enabled to build (checked)

        foreach (var target in AvailableTargets)
        {
            TargetsToBuild[target] = EditorGUILayout.Toggle(target.ToString(), TargetsToBuild[target]);

            if (TargetsToBuild[target])
                numEnabled++;
        }

        //do build(s)
        if(numEnabled > 0)
        {
            //button to try building chosen platforms
            string prompt = numEnabled == 1 ? "Build 1 Platform" : $"Build {numEnabled} Platforms";
            if (GUILayout.Button(prompt))
            {
                List<BuildTarget> selectedTargets = new List<BuildTarget>();
                foreach(var target in AvailableTargets)
                {
                    if (TargetsToBuild[target])
                        selectedTargets.Add(target);
                }

                //start build coroutine
                EditorCoroutineUtility.StartCoroutine(PerformBuild(selectedTargets), this);
            }
        }
    }


    //Coroutine for building chosen platforms
    IEnumerator PerformBuild(List<BuildTarget> targetsToBuild)
    {
        //start sticky progress (progress remains after finishing) and keep track of it via ID
        int buildAllProgressID = Progress.Start("Build All", "Building all selected platforms", Progress.Options.Sticky);
        Progress.ShowDetails(); //display progress
        yield return new EditorWaitForSeconds(1f); //wait for 1 second to finish progress

        BuildTarget originalTarget = EditorUserBuildSettings.activeBuildTarget;  //keep track of original target

        //loop through all targets and build each target
        for(int targetIndex = 0; targetIndex < targetsToBuild.Count; ++targetIndex)
        {
            
            var buildTarget = targetsToBuild[targetIndex]; //current target

            Progress.Report(buildAllProgressID, targetIndex + 1, targetsToBuild.Count); //show progress

            //start sticky progress and keep track of it via ID
            int buildTaskProgressID = Progress.Start($"Build {buildTarget.ToString()}", null, Progress.Options.Sticky, buildAllProgressID);
            yield return new EditorWaitForSeconds(1f); //wait for 1 second to finish progress

            //perform build
            if (!BuildIndividualTarget(buildTarget)) //failed
            {
                Progress.Finish(buildTaskProgressID, Progress.Status.Failed);
                Progress.Finish(buildAllProgressID, Progress.Status.Failed);

                if (EditorUserBuildSettings.activeBuildTarget != originalTarget) //if not original target anymore - switch to async target
                    EditorUserBuildSettings.SwitchActiveBuildTargetAsync(GetTargetGroupForTarget(originalTarget), originalTarget);

                yield break;
            }
            
            //build was successful
            Progress.Finish(buildTaskProgressID, Progress.Status.Succeeded);

            if (EditorUserBuildSettings.activeBuildTarget != originalTarget) //if not original target anymore - switch to async target
                EditorUserBuildSettings.SwitchActiveBuildTargetAsync(GetTargetGroupForTarget(originalTarget), originalTarget);

            yield return new EditorWaitForSeconds(1f);
        }

        //coroutine reaches that point - build was successful
        Progress.Finish(buildAllProgressID, Progress.Status.Succeeded); 
        
        yield return null;
    }


    //returns true if build was successful
    private bool BuildIndividualTarget(BuildTarget target)
    {
        //the player options
        BuildPlayerOptions options = new BuildPlayerOptions(); //the player options

        //eigene Szenen für jeden Build
        SetScenes(target);

        //get list of scenes
        List<string> scenes = new List<string>();
        foreach (var scene in EditorBuildSettings.scenes)
            scenes.Add(scene.path);

        //configure the build
        options.scenes = scenes.ToArray(); //the scenes to build
        options.target = target; //build target (e.g. Windows)
        options.targetGroup = GetTargetGroupForTarget(target);

        //set location path name
        if (target == BuildTarget.Android) //Android needs specific folder
        {
            string apkName = PlayerSettings.productName + ".apk";
            options.locationPathName = System.IO.Path.Combine("Builds", target.ToString(), apkName);
        }
        else
            options.locationPathName = System.IO.Path.Combine("Builds", target.ToString(), PlayerSettings.productName);

        if (BuildPipeline.BuildCanBeAppended(target, options.locationPathName) == CanAppendBuild.Yes)
            options.options = BuildOptions.AcceptExternalModificationsToPlayer; //if build can be appended - accept external modifications
        else
            options.options = BuildOptions.None; //default build options

        //start the build
        BuildReport report = BuildPipeline.BuildPlayer(options);

        if (report.summary.result == BuildResult.Succeeded) //build was successful
        {
            Debug.Log($"Build for {target.ToString()} completed in {report.summary.totalTime.Seconds} seconds"); //log needed time span
            return true;
        }

        //if method reaches that point - build failed
        Debug.LogError($"Build for {target.ToString()} failed"); //log which build failed
        return false;
    }


    //method used to enable/disable scenes for target build
    private void SetScenes(BuildTarget target)
    {
        EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes; //all scenes in EditorBuildSettings
        List<EditorBuildSettingsScene> scenesToEnable = new List<EditorBuildSettingsScene>(); //scenes that shall be enabled for build

        //Build vor VR and Scenario Arachnophobia
        if (target == BuildTarget.Android && (NetworkingManager.Scenario)PhotonNetwork.CurrentRoom.CustomProperties["ChosenScenario"] == NetworkingManager.Scenario.Arachnophobia) 
        {
            //list with scenes to enable for build
            string[] scenesToEnableList = new string[] {  "Menus/MainMenuVR",
                                                          "Menus/FindRoom",
                                                          "Menus/WaitingScene",
                                                          "MapPhobia"};

            //add list components to scenesToEnable list
            for (int i = 0; i < scenesToEnableList.Length; i++)
            {
                scenesToEnable.Add(new EditorBuildSettingsScene(mainFolder + scenesToEnableList[i] + sceneType, true));
            }
        }
        //build for VR and Scenario Machine Operating
        else if (target == BuildTarget.Android && (NetworkingManager.Scenario)PhotonNetwork.CurrentRoom.CustomProperties["ChosenScenario"] == NetworkingManager.Scenario.MachineOperating)
        {
            //list with scenes to enable for build
            string[] scenesToEnableList = new string[] {  "Menus/MainMenuVR",
                                                          "Menus/FindRoom",
                                                          "Menus/WaitingScene",
                                                          "MapLearning"};

            //add list components to scenesToEnable list
            for (int i = 0; i < scenesToEnableList.Length; i++)
            {
                scenesToEnable.Add(new EditorBuildSettingsScene(mainFolder + scenesToEnableList[i] + sceneType, true));
            }
        }
        //build for Windows/iOs/Linux
        else if (target != BuildTarget.Android)
        {
            //list with scenes to enable for build
            string[] scenesToEnableList = new string[] {  "Menus/MainMenuPc",
                                                          "Menus/CreateRoom",
                                                          "Menus/ChooseScenario",
                                                          "UISupervisor",
                                                          "ScenarioEvaluation"};

            //add list components to scenesToEnable list
            for (int i = 0; i < scenesToEnableList.Length; i++)
            {
                scenesToEnable.Add(new EditorBuildSettingsScene(mainFolder + scenesToEnableList[i] + sceneType, true));
            }
        }


        //check for each scene in scenes list if it should be enabled (= is in scenesToEnable)
        foreach (EditorBuildSettingsScene scene in scenes)
        {
            if (scenesToEnable.Contains(scene))
                scene.enabled = true; //enable if yes
            else scene.enabled = false; //disable if no
        }

        //update the scenes list in the EditorBuildSettings so the scenes needed are enabled and the ones not needed
        //are disabled
        EditorBuildSettings.scenes = scenes;
    }
}
