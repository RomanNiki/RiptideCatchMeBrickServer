namespace Player
{
    public class PlayerComponents
    {
        public PlayerComponents(Player client, PlayerMover mover, PlayerShooter shooter)
        {
            Client = client;
            Mover = mover;
            Shooter = shooter;
        }

        public Player Client { get; private set; }
        public PlayerMover Mover { get; private set; }
        public PlayerShooter Shooter { get; private set; }
    }
}