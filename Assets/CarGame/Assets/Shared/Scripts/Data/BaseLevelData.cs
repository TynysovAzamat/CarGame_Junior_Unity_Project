using UnityEngine;

public class BaseLevelData : ScriptableObject
{
    [Header("Level Data")]
    [SerializeField] private int levelID;
    [SerializeField] private string levelName;
    [SerializeField] private Sprite previewIcon;
    [SerializeField] private AudioClip levelMusic;

    public int LevelID => levelID;
    public string LevelName => levelName;
    public Sprite PreviewIcon => previewIcon;

    public AudioClip LevelMusic => levelMusic;
}
