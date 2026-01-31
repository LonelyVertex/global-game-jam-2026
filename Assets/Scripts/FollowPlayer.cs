using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    private Transform _player;

    private void Start()
    {
        _player = GameManager.Instance.Player.transform;
    }

    private void Update()
    {
        if (!_player) return;

        transform.position = Utils.Vector3XY(_player.position, transform.position);
    }
}
