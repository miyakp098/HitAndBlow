using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    public AudioSource audioSourceSE = null;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);

            // Photonオブジェクトが存在するか確認
            if (FindObjectOfType<Photon.Pun.PhotonView>() == null)
            {
                Debug.LogWarning("PhotonMonoが見つかりませんでした。");
            }

            // AudioSourceコンポーネントを追加
            audioSourceSE = gameObject.AddComponent<AudioSource>();
            audioSourceSE.volume = 0.8f;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // SEボリュームを設定するメソッド
    public void SetVolumeSE(float volume)
    {
        if (audioSourceSE != null)
            audioSourceSE.volume = volume;
    }

    public void PlaySE(AudioClip clip)
    {
        if (audioSourceSE != null)
        {
            audioSourceSE.PlayOneShot(clip);
        }
        else
        {
            Debug.Log("SE用のオーディオソースが設定されていません");
        }
    }
}
