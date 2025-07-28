using UnityEngine;

[System.Serializable]
public class SaveData
{
    [Header("Loop Tracker")]
    public int currentLoop;
    [Header("Map Save")]
    public bool[] mapsSaved;
    public float[][][][] lineList;
    [Header("Scene Loader")]
    public FlashbackSwitch[] flashbackSwitches;
    [Header("Note Save")]
    public bool[] noteList;


    public SaveData(LoopTracker loopTracker, MapSave mapSave, SceneLoader sceneLoader, NoteSave noteSave)
    {
        if (loopTracker == null) { Debug.LogWarning("Loop Tracker null"); return; }
        if (mapSave == null) { Debug.LogWarning("Map Save null"); return; } 
        if (sceneLoader == null) { Debug.LogWarning("Scene Loader null"); return; }
        if (noteSave == null) { Debug.LogWarning("Note Save null"); return;}

        currentLoop = loopTracker.CurrentLoop();
        mapsSaved = mapSave.MSave();
        lineList = new float[mapsSaved.Length][][][];
        for (int map = 0; map < mapSave.list.Length; map++)
        {
            lineList[map] = new float[mapSave.list[map].Length][][];
            for (int line = 0; line < mapSave.list[map].Length; line++)
            {
                lineList[map][line] = new float[mapSave.list[map][line].Length][];
                for (int point = 0; point < mapSave.list[map][line].Length; point++)
                {
                    lineList[map][line][point] = new float[3];
                    float x = mapSave.list[map][line][point].x;
                    float y = mapSave.list[map][line][point].y;
                    float z = mapSave.list[map][line][point].z;
                    lineList[map][line][point][0] = x;
                    lineList[map][line][point][1] = y;
                    lineList[map][line][point][2] = z;
                }
            }
        }
        flashbackSwitches = sceneLoader.FlashbackSwitches();
        noteList = noteSave.LoadList();
    }
}
