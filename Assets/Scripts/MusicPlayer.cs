using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq; // 需要使用 LINQ 来查找映射

public class MusicPlayer : MonoBehaviour
{
    // ==========================================
    // 可序列化类：用于在 Inspector 中映射场景和音乐
    // ==========================================
    [System.Serializable]
    public class SceneMusicMap
    {
        [Tooltip("场景名称 (必须与项目中的场景名称精确匹配)")]
        public string sceneName;
        
        [Tooltip("该场景对应的音乐文件")]
        public AudioClip musicClip;
    }

    public static MusicPlayer Instance { get; private set; }

    [Header("音乐配置")]
    [Tooltip("设置每个场景对应的音乐片段")]
    public List<SceneMusicMap> sceneMusicMaps;
    
    private AudioSource audioSource;

    void Awake()
    {
        // 1. 实现单例模式
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                Debug.LogError("MusicPlayer: 找不到 AudioSource 组件!");
            }
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void OnEnable()
    {
        // 注册场景加载事件
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /// <summary>
    /// 场景加载完成时调用，用于切换音乐。
    /// </summary>
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (audioSource == null) return;
        
        // 查找当前场景对应的音乐配置
        SceneMusicMap map = sceneMusicMaps.FirstOrDefault(m => m.sceneName == scene.name);
        
        if (map != null && map.musicClip != null)
        {
            // 找到了对应的音乐
            if (audioSource.clip != map.musicClip || !audioSource.isPlaying)
            {
                // 如果当前播放的不是这首，或者音乐停止了，则切换并播放
                audioSource.Stop();
                audioSource.clip = map.musicClip;
                audioSource.Play();
                Debug.Log($"切换到场景音乐: {scene.name} - {map.musicClip.name}");
            }
        }
        else
        {
            // 如果场景没有配置音乐，则停止播放
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
                Debug.Log($"场景 '{scene.name}' 未配置音乐，停止播放。");
            }
        }
    }
}