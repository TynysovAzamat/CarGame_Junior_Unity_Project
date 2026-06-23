using System;
public interface ISceneLoader 
{
    void LoadScene(string sceneName, Action onSceneLoaded = null);
}
