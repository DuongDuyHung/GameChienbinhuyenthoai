using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLevel : MonoBehaviour
{
    [SerializeField]
    private int fixedLevel = 1; // Cấp độ cố định của quái vật

    // Phương thức để lấy cấp độ
    public int GetLevel()
    {
        return fixedLevel;
    }
    public int FixedLevel
    {
        get { return fixedLevel; }
        set { fixedLevel = value; }
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
