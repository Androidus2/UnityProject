using UnityEngine;

public class LockpickController : MonoBehaviour
{
    [Header("Obiecte de Scena")]
    [Tooltip("Obiectul SPERACLU care se roteste cu mouse-ul")]
    public Transform lockPick;

    [Tooltip("Obiectul BUTUC care se roteste cu tasta 'E'")]
    public Transform lockCylinder;

    [Header("Gameplay settings")]
    [Tooltip("speraclu-sensitivity")]
    public float pickSensitivity = 1.0f;

    [Tooltip("unghi maxim s/d speraclu")]
    public float maxPickAngle = 90.0f;

    [Tooltip("cat de repede se roteste butucul wp")]
    public float lockRotateSpeed = 5.0f;

    [Tooltip("cat de mare este zona in jurul SS")]
    public float tolerance = 10.0f;

    private float pickAngle = 0.0f;         // unghiul curent speraclu
    private float unlockAngle;              // unghiul tinta (SS)
    private float cylinderRotation = 0.0f;  // rotatia curenta a butucului

    // Starea principala a sistemului
    private bool isMovingPick = true;       // true = miscarea speraclului, false = rotirea butucului

    void Start()
    {
        // Genereaza un nou SS
        GenerateNewUnlockAngle();
    }

    void Update()
    {
        // Verifica input-ul jucatorului
        HandleInput();

        // Actualizam logica in functie de stare
        if (isMovingPick)
        {
            // 1: miscarea speraclului 
            MovePick();
        }
        else
        {
            // 2: Aplicarea Tensiunii (Rotirea Butucului)
            TurnLock();
        }
    }

    // Genereaza un nou unghi SS
    void GenerateNewUnlockAngle()
    {
        // Genereaza un unghi aleatoriu in intervalul permis
        // intre -90 si +90 grade
        unlockAngle = Random.Range(-maxPickAngle, maxPickAngle);

        // unghi vazut in consola pentru testare
        Debug.Log($"Punctul dulce este: {unlockAngle}");
    }

    // Gestioneaza input-ul jucatorului
    void HandleInput()
    {
        // tasta 'E' -pressed
        if (Input.GetKeyDown(KeyCode.E))
        {
            isMovingPick = false; // Oprim miscarea speraclului si incepem rotirea butucului
        }

        // tasta 'E' -released
        if (Input.GetKeyUp(KeyCode.E))
        {
            isMovingPick = true; // oprim rotirea si permitem miscarea speraclului

            // Resetam rotatia butucului la 0 usor
            cylinderRotation = 0.0f;
        }
    }

    // logica pentru miscarea speraclului (Starea 1)
    void MovePick()
    {
        // citeste miscarea mouse-ului pe axa X
        float mouseInput = Input.GetAxis("Mouse X") * pickSensitivity;

        //adauga miscarea la unghiul curent
        pickAngle += mouseInput;

        // blocheaza (clampeaza) unghiul intre valorile min si max
        pickAngle = Mathf.Clamp(pickAngle, -maxPickAngle, maxPickAngle);

        // aplica rotatia pe obiectul speraclu (pe axa Z)
        lockPick.localRotation = Quaternion.Euler(0, 0, pickAngle);

        // reseteaza rotatia butucului la 0 ; folosim Lerp pentru o miscare lina de revenire
        cylinderRotation = Mathf.Lerp(cylinderRotation, 0, Time.deltaTime * lockRotateSpeed);
        lockCylinder.localRotation = Quaternion.Euler(0, 0, cylinderRotation);
    }

    // logica pentru rotirea butucului (Starea 2)
    void TurnLock()
    {
        // 1. Calculeaza distanta de la unghiul nostru la SS
        // Mathf.Abs da valoarea absoluta (ignora + si -)
        float distance = Mathf.Abs(pickAngle - unlockAngle);

        // 2. Verifica daca suntem in zona de succes (+/- toleranta)
        if (distance <= tolerance)
        {
            // suntem in zona de toleranta ; rotim butucul complet
            cylinderRotation = Mathf.Lerp(cylinderRotation, 90.0f, Time.deltaTime * lockRotateSpeed);

            // Verifica daca am ajuns la 90 de grade 
            if (cylinderRotation >= 89.0f)
            {
                Debug.Log("DEBLOCAT!");
                isMovingPick = true; // Permite miscarea din nou
                GenerateNewUnlockAngle(); // Genereaza un nou puzzle
            }
        }
        else  
        {
            // Suntem prea departe. Rotim butucul doar putin, proportional cu cat de aproape suntem de SS si butucul incepe sa tremure

            // Calculeaza unghiul maxim permis in functie de distanta ; cu cat suntem mai departe, cu atat mai mic este unghiul
            float maxRotation = 90.0f * (1.0f - (distance / maxPickAngle));

            // asiguram ca rotatia nu trece de acest maxim
            cylinderRotation = Mathf.Lerp(cylinderRotation, maxRotation, Time.deltaTime * lockRotateSpeed);

            // logica shaking si breaking speraclului trebuie adaugata aici(optiuni :speraclul se poate rupe daca fortam prea mult sau daca suntem prea departe de SS si schimbare de unghi rapida pentru a simula tremuratul de dinainte de rupere)
            float shake = Random.Range(-2.0f, 2.0f);
            lockPick.localRotation = Quaternion.Euler(0, 0, pickAngle + shake);

            // verificare daca speraclul se rupe sau nu (de exemplu, daca distanta este prea mare si incercam sa fortam rotirea)
        }

        // 3. Aplica rotatia calculata pe butuc (pe axa Z)
        lockCylinder.localRotation = Quaternion.Euler(0, 0, cylinderRotation);
    }
}