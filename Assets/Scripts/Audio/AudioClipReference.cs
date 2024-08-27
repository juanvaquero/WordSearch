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

        public bool CompareConstant(string value)
        {
            return Id == value;
        }

        public string GetConstantValue()
        {
            return Id;
        }
    }

    public AudioClip GetClipById(string id)
    {
        foreach (var entry in _audioClips)
        {
            if (entry.Id == id)
            {
                return entry.Clip;
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
