using UnityEngine;
using System.Collections.Generic;

public class Player_Weapon_Audio : MonoBehaviour
{
    public enum ClipStates { None, Move, Hit, On, Off, Count }

    public AudioSource saberAudio;

    //public AudioClip[] idleClips;
    public AudioClip[] moveClips;
    public AudioClip[] hitClips;
    public AudioClip[] onClips;
    public AudioClip[] offClips;

    private ClipStates m_curState;

    private Dictionary<ClipStates, AudioClip[]> m_states;

    // Use this for initialization
    void Awake()
    {
        m_states = new Dictionary<ClipStates, AudioClip[]>();
        //m_states.Add(ClipStates.Idle, idleClips);
        m_states.Add(ClipStates.Move, moveClips);
        m_states.Add(ClipStates.Hit, hitClips);
        m_states.Add(ClipStates.On, onClips);
        m_states.Add(ClipStates.Off, offClips);
        m_states.Add(ClipStates.None, null);
    }

    public void SetState(ClipStates state, bool overridePlaying = false)
    {
        if (state == ClipStates.None)
        {
            saberAudio.Stop();
        }

        if(!overridePlaying && saberAudio.isPlaying)
        {
            return;
        }

        m_curState = state;
        AudioClip curClip = m_states[m_curState][Random.Range(0, m_states[m_curState].Length)];
        saberAudio.clip = curClip;
        saberAudio.Play();
    }
}
