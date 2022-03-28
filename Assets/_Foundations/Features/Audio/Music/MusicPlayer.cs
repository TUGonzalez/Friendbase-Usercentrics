namespace Audio.Music
{
    public interface IMusicPlayer
    {
        void Play(string trackId, float fadeDuration = 0);
        void Stop();
    }
}