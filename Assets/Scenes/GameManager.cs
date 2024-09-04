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

            // Photon�I�u�W�F�N�g�����݂��邩�m�F
            if (FindObjectOfType<Photon.Pun.PhotonView>() == null)
            {
                Debug.LogWarning("PhotonMono��������܂���ł����B");
            }

            // AudioSource�R���|�[�l���g��ǉ�
            audioSourceSE = gameObject.AddComponent<AudioSource>();
            audioSourceSE.volume = 0.8f;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // SE�{�����[����ݒ肷�郁�\�b�h
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
            Debug.Log("SE�p�̃I�[�f�B�I�\�[�X���ݒ肳��Ă��܂���");
        }
    }
}
