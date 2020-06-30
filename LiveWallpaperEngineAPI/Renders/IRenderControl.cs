namespace Giantapp.LiveWallpaper.Engine.Renders
{
    public interface IRenderControl
    {
        void InitRender();
        void Load(string path);
        void Stop();
        void Pause();
        void Resum();
        void DisposeRender();
        void SetVolume(int volume);
    }
}
