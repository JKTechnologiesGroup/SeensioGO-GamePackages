namespace JKTechnologies.SeensioGo.GameInstances
{
    public interface IGameInstanceProgressListener
    {
        public void SetupTargetScore(int targetScore, int currentPlayerScore);
        public void OnPlayerScoreUpdated(int currentPlayerScore);
    }
}