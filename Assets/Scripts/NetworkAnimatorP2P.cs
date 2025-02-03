using Unity.Netcode.Components;

public class NetworkAnimatorP2P : NetworkAnimator
{
    /// <summary>
    /// Change Owner authoritative mode to allow client to send animations to the host.
    /// <para>
    /// Use NetworkAnimatorP2P instead of NetworkAnimator,
    /// and add the Animator as a parameter in Unity
    /// </para>
    /// </summary>
    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }
}
