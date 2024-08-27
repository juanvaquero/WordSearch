using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioClipReference", menuName = "Audio/AudioClipReference")]
public class AudioClipReference : ConstantScriptableObject
{
    [SerializeField]
    private List<AudioClipEntry> _audioClips = new List<AudioClipEntry>();

    [Serializable]
    public class AudioClipEntry : IConstant
    {
        public string Id;
        public AudioClip Clip;
        [Range(-3f, 3f)]
        public float Pitch = 1f;

        public bool CompareConstant(string value)
        {
            return Id == value;
        }

        public string GetConstantValue()
        {
            return Id;
        }
    }

    public AudioClipEntry GetClipById(string id)
    {
        foreach (var entry in _audioClips)
        {
            if (entry.Id == id)
            {
                return entry;
            }
        }
        Debug.LogWarning("AudioClip with ID: " + id + " not found!");
        return null;
    }

    public override IConstant[] GetConstants()
    {
        return _audioClips.ToArray();
    }
}
