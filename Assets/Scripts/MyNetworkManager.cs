using Mirror;

public class MyNetworkManager : NetworkManager {
    public override void OnServerAddPlayer(NetworkConnectionToClient conn) {
        base.OnServerAddPlayer(conn);

        PlayerInfo pInfo = conn.identity.GetComponent<PlayerInfo>();

        // if (numPlayers == 1) {
        //     pInfo.side = PlayerSide.Attack; // host
        // } else {
        //     pInfo.side = PlayerSide.Defense; // client
        // }

        // Gọi TargetDrawStartingHand cho client vừa vào
        if (GameManager.Instance != null)
            GameManager.Instance.TargetDrawStartingHand(conn);
    }
}
