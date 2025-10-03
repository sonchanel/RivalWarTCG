using Mirror;

public enum PlayerSide { None, Attack, Defense }

public class PlayerInfo : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnSideChanged))] public PlayerSide side;

    void OnSideChanged(PlayerSide oldSide, PlayerSide newSide)
    {
        if (isLocalPlayer)
        {
            UIManager.Instance.SetupUI(newSide);
        }
    }

    public override void OnStartLocalPlayer()
    {

        // UIManager.Instance.SetupUI(side);
    }
    public void LoadUISide()
    {
        UIManager.Instance.SetupUI(side);
    }
}
