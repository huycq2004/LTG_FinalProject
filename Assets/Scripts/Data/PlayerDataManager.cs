using UnityEngine;

/// PlayerDataManager - Quan ly luu tru va tai du lieu nguoi choi bang PlayerPrefs
/// Luu tru: Vang, Mau hien tai, Mau toi da, Toc do, Sat thuong, Luong mau hoi
public class PlayerDataManager : MonoBehaviour
{
    // ====================
    // SINGLETON
    // ====================

    public static PlayerDataManager Instance { get; private set; }

    // ====================
    // PLAYERPREFS KEYS
    // ====================

    private const string KEY_GOLD = "PlayerGold";
    private const string KEY_CURRENT_HEALTH = "PlayerCurrentHealth";
    private const string KEY_MAX_HEALTH = "PlayerMaxHealth";
    private const string KEY_MOVE_SPEED = "PlayerMoveSpeed";
    private const string KEY_ATTACK_DAMAGE = "PlayerAttackDamage";
    private const string KEY_HEAL_AMOUNT = "PlayerHealAmount";
    private const string KEY_HAS_BOW = "PlayerHasBow";
    private const string KEY_FIRST_TIME = "IsFirstTime";

    // ====================
    // CAC GIA tri MAC DINH - NGUON CHAN LY DUY NHAT
    // ====================
    // Cac gia tri nay se duoc dung de khoi tao lan dau choi
    // va khi reset game. Tat ca cac Manager khac se lay gia tri tu day.

    [Header("Default Values")]
    [Tooltip("So vang khoi dau")]
    public int defaultGold = 10;
    
    [Tooltip("Mau toi da khoi dau")]
    public int defaultMaxHealth = 5;
    
    [Tooltip("Toc do di chuyen mac dinh")]
    public float defaultMoveSpeed = 5f;
    
    [Tooltip("Sat thuong mac dinh")]
    public int defaultAttackDamage = 1;
    
    [Tooltip("Luong mau hoi moi lan dung Soul")]
    public int defaultHealAmount = 1;

    // ====================
    // KHOI TAO
    // ====================

    void Awake()
    {
        SetupSingleton();
    }

    void SetupSingleton()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Neu la lan dau choi, khoi tao gia tri mac dinh
        if (IsFirstTime())
        {
            InitializeDefaultValues();
            SetFirstTime(false);
        }
    }

    // ====================
    // KIEM TRA LAN DAU CHOI
    // ====================

    private bool IsFirstTime()
    {
        return PlayerPrefs.GetInt(KEY_FIRST_TIME, 1) == 1;
    }

    private void SetFirstTime(bool isFirstTime)
    {
        PlayerPrefs.SetInt(KEY_FIRST_TIME, isFirstTime ? 1 : 0);
        PlayerPrefs.Save();
    }

    // ====================
    // KHOI TAO GIA TRI MAC DINH
    // ====================

    private void InitializeDefaultValues()
    {
        SaveGold(defaultGold);
        SaveCurrentHealth(defaultMaxHealth);
        SaveMaxHealth(defaultMaxHealth);
        SaveMoveSpeed(defaultMoveSpeed);
        SaveAttackDamage(defaultAttackDamage);
        SaveHealAmount(defaultHealAmount);
        SaveHasBow(false);  // Mac dinh chua co cung
        
        Debug.Log("Khoi tao du lieu nguoi choi mac dinh");
    }

    // ====================
    // LUU DU LIEU - VANG
    // ====================

    public void SaveGold(int gold)
    {
        PlayerPrefs.SetInt(KEY_GOLD, gold);
        PlayerPrefs.Save();
        Debug.Log("Da luu vang: " + gold);
    }

    public int LoadGold()
    {
        return PlayerPrefs.GetInt(KEY_GOLD, defaultGold);
    }

    // ====================
    // LUU DU LIEU - MAU HIEN TAI
    // ====================

    public void SaveCurrentHealth(int health)
    {
        PlayerPrefs.SetInt(KEY_CURRENT_HEALTH, health);
        PlayerPrefs.Save();
        Debug.Log("Da luu mau hien tai: " + health);
    }

    public int LoadCurrentHealth()
    {
        return PlayerPrefs.GetInt(KEY_CURRENT_HEALTH, defaultMaxHealth);
    }

    // ====================
    // LUU DU LIEU - MAU TOI DA
    // ====================

    public void SaveMaxHealth(int maxHealth)
    {
        PlayerPrefs.SetInt(KEY_MAX_HEALTH, maxHealth);
        PlayerPrefs.Save();
        Debug.Log("Da luu mau toi da: " + maxHealth);
    }

    public int LoadMaxHealth()
    {
        return PlayerPrefs.GetInt(KEY_MAX_HEALTH, defaultMaxHealth);
    }

    // ====================
    // LUU DU LIEU - TOC DO
    // ====================

    public void SaveMoveSpeed(float speed)
    {
        PlayerPrefs.SetFloat(KEY_MOVE_SPEED, speed);
        PlayerPrefs.Save();
        Debug.Log("Da luu toc do: " + speed);
    }

    public float LoadMoveSpeed()
    {
        return PlayerPrefs.GetFloat(KEY_MOVE_SPEED, defaultMoveSpeed);
    }

    // ====================
    // LUU DU LIEU - SAT THUONG
    // ====================

    public void SaveAttackDamage(int damage)
    {
        PlayerPrefs.SetInt(KEY_ATTACK_DAMAGE, damage);
        PlayerPrefs.Save();
        Debug.Log("Da luu sat thuong: " + damage);
    }

    public int LoadAttackDamage()
    {
        return PlayerPrefs.GetInt(KEY_ATTACK_DAMAGE, defaultAttackDamage);
    }

    // ====================
    // LUU DU LIEU - LUONG MAU HOI
    // ====================

    public void SaveHealAmount(int healAmount)
    {
        PlayerPrefs.SetInt(KEY_HEAL_AMOUNT, healAmount);
        PlayerPrefs.Save();
        Debug.Log("Da luu luong mau hoi: " + healAmount);
    }

    public int LoadHealAmount()
    {
        return PlayerPrefs.GetInt(KEY_HEAL_AMOUNT, defaultHealAmount);
    }

    // ====================
    // LUU DU LIEU - CUNG (BOW)
    // ====================

    public void SaveHasBow(bool hasBow)
    {
        PlayerPrefs.SetInt(KEY_HAS_BOW, hasBow ? 1 : 0);
        PlayerPrefs.Save();
        Debug.Log("Da luu trang thai cung: " + (hasBow ? "Co" : "Khong"));
    }

    public bool LoadHasBow()
    {
        return PlayerPrefs.GetInt(KEY_HAS_BOW, 0) == 1;
    }

    // ====================
    // TAI TAT CA DU LIEU
    // ====================

    public PlayerData LoadAllPlayerData()
    {
        PlayerData data = new PlayerData
        {
            gold = LoadGold(),
            currentHealth = LoadCurrentHealth(),
            maxHealth = LoadMaxHealth(),
            moveSpeed = LoadMoveSpeed(),
            attackDamage = LoadAttackDamage(),
            healAmount = LoadHealAmount()
        };

        Debug.Log($"Da tai du lieu: Gold={data.gold}, HP={data.currentHealth}/{data.maxHealth}, Speed={data.moveSpeed}, ATK={data.attackDamage}, Heal={data.healAmount}");
        
        return data;
    }

    // ====================
    // XOA DU LIEU
    // ====================

    public void ResetAllData()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        InitializeDefaultValues();
        Debug.Log("Da reset tat ca du lieu ve gia tri mac dinh");
    }

    public void DeletePlayerData()
    {
        PlayerPrefs.DeleteKey(KEY_GOLD);
        PlayerPrefs.DeleteKey(KEY_CURRENT_HEALTH);
        PlayerPrefs.DeleteKey(KEY_MAX_HEALTH);
        PlayerPrefs.DeleteKey(KEY_MOVE_SPEED);
        PlayerPrefs.DeleteKey(KEY_ATTACK_DAMAGE);
        PlayerPrefs.DeleteKey(KEY_HEAL_AMOUNT);
        PlayerPrefs.DeleteKey(KEY_HAS_BOW);
        PlayerPrefs.Save();
        Debug.Log("Da xoa du lieu nguoi choi");
    }
}

// ====================
// CLASS DU LIEU
// ====================

[System.Serializable]
public class PlayerData
{
    public int gold;
    public int currentHealth;
    public int maxHealth;
    public float moveSpeed;
    public int attackDamage;
    public int healAmount;
}
