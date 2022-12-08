using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public enum PlayerType
{
    attacker,
    defender
};

public class Player : NetworkBehaviour
{
    public NetworkVariable<Vector3> PlayerRotation = new NetworkVariable<Vector3>();
    public NetworkVariable<Vector3> PlayerPosition = new NetworkVariable<Vector3>();
    public NetworkVariable<Color> PlayerColor = new NetworkVariable<Color>(Color.red);
    public NetworkVariable<int> PlayerSprite = new NetworkVariable<int>(-1);

    public NetworkVariable<int> Score = new NetworkVariable<int>(50);

    public Quaternion newRotation = new Quaternion();
    public BulletSpawner bulletSpawner;
    public GameObject playerSprite;

    public Image attacker;
    public Image defender;

    public float movementSpeed = .5f;
    public AudioSource source;
    public AudioClip fire;

    public PlayerType CurrentPlayerType
    {
        get { return m_playerType; }
    }

    private GameManager _gameMgr;
    public float rotationSpeed = 4f;
    private BulletSpawner _bulletSpawner;
    private PlayerType m_playerType;
    private Vector3 rotationVector = new Vector3();
    private float lastFire;
    private bool cooldownAttack = true;
    private GameObject gameManager;
    

    public TMPro.TMP_Text txtScoreDisplay;

    private void Start()
    {
        ApplyPlayerShipSprite();
        ApplyPlayerPosition();

        gameManager = GameObject.FindGameObjectWithTag("GameManager");
    

        if (m_playerType == PlayerType.attacker)
        {
            rotationSpeed /= 2f; 
        }
    }

    void Update()
    {

        if (IsOwner)
        {

            Vector3 results = CalcMovement();

            RequestPositionForMovementServerRpc(results);

            if (Input.GetButtonDown("Fire1") && cooldownAttack)
            {
                _bulletSpawner.FireServerRpc();
                source.PlayOneShot(fire);
                StartCoroutine(Cooldown());
            }
        }

        if (!IsOwner || IsHost)
        {
            transform.Rotate(PlayerRotation.Value);
        }
    }

    private IEnumerator Cooldown()
    {
        cooldownAttack = false;
        if (m_playerType == PlayerType.attacker)
        {
            yield return new WaitForSeconds(.1f);
        }
        if (m_playerType == PlayerType.defender)
        {
            yield return new WaitForSeconds(.7f);
        }

        cooldownAttack = true;
    }


    //------------------------
    //Events
    //------------------------

    private void ClientOnScoreChanged(int previous, int current)
    {
        DisplayScore();
    }

    public void OnPlayerSpriteChanged(int previous, int current)
    {
        ApplyPlayerShipSprite();
        ApplyPlayerPosition();
    }

    public void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("Bullet"))
        {
            HandleBulletCollision(collision.gameObject);
        }
        
    }

    public void ApplyPlayerShipSprite()
    {
        if (PlayerSprite.Value == 1)
        {
            attacker.enabled = false;
            defender.enabled = true;
            m_playerType = PlayerType.defender;
            GetComponent<Health>().enabled = false;
            GetComponentInChildren<HealthBar>().rootCanvas.enabled = false;
            GetComponentInChildren<CapsuleCollider>().enabled = false;
        }
        if (PlayerSprite.Value == 2)
        {
            attacker.enabled = true;
            defender.enabled = false;
            m_playerType = PlayerType.attacker;
        }
    }

    public void ApplyPlayerPosition()
    {
        playerSprite.transform.position = PlayerPosition.Value;

        if (m_playerType == PlayerType.attacker)
        {
            playerSprite.transform.rotation = newRotation;
        }
    }

    //------------------------
    //RPC
    //------------------------

    [ServerRpc]
    void RequestPositionForMovementServerRpc(Vector3 rotChange) {
        if (!IsServer && !IsHost) return;

        PlayerRotation.Value = rotChange;
    }

    [ServerRpc]
    public void RequestSetScoreServerRpc(int value)
    {
        Score.Value = value;
    }

    //------------------------
    //Public
    //------------------------
    public override void OnNetworkSpawn()
    {

        Score.OnValueChanged += ClientOnScoreChanged;

        _bulletSpawner = bulletSpawner;

        if (IsHost)
        {
            _bulletSpawner.BulletDamage.Value = 1;
        }

        DisplayScore();
    }

    public void DisplayScore()
    {
        txtScoreDisplay.text = Score.Value.ToString();

    }

    //------------------------
    // Private
    //------------------------

    private void HandleBulletCollision(GameObject bullet)
    {
        Bullet bulletScript = bullet.GetComponent<Bullet>();

        if (!bulletScript.isAttackerBullet)
        {


            Score.Value -= bulletScript.Damage.Value;

            ulong ownerClientId = bullet.GetComponent<NetworkObject>().OwnerClientId;
            Player otherPlayer = NetworkManager.Singleton.ConnectedClients[ownerClientId].PlayerObject.GetComponent<Player>();
            otherPlayer.Score.Value += bulletScript.Damage.Value;
            GetComponent<Health>().ApplyDamage(otherPlayer.Score.Value);


            Destroy(bullet);
        }
    }
       

    private void HostHandleDamageBoostPickup(Collider other)
    {
        if(!_bulletSpawner.IsAtMaxDamage())
        {
            _bulletSpawner.IncreaseDamage();
            other.GetComponent<NetworkObject>().Despawn();
        }
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if(IsHost)
        {
            if (other.gameObject.CompareTag("DamageBoost")) {
                HostHandleDamageBoostPickup(other);
            }
        }
    }

    private Vector3 CalcMovement()
    {
        float yRotation  = Input.GetAxis("Horizontal");
        float currentRotation = transform.rotation.eulerAngles.z;


        if (currentRotation > 85 && currentRotation < 90)
        {
            if (yRotation < 0)
            {
                yRotation = 0;
            }
        }

        if (currentRotation > 265 && currentRotation < 270)
        {
            if (yRotation > 0)
            {
                yRotation = 0;
            }
        }

        rotationVector = new Vector3(0, 0, -yRotation);
        rotationVector *= rotationSpeed;

        return rotationVector;
    }

}