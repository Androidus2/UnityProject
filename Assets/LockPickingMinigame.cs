using System;
using UnityEngine;

// interfata optionala pentru sistemul de inventar (de ex. pentru a gestiona speraclurile)
public interface ILockpickInventory
{
    int GetLockpickCount();
    bool ConsumeLockpick(); // consuma un speraclu returneaza true daca a avut ce sa consume
    void AddLockpicks(int amount);
}

public class LockPickingMinigame : MonoBehaviour
{
    // Variabile pentru pozitia speraclului (Pick)
    float pickPosition;
    // Ne asiguram ca pozitia speraclului ramane intre 0 si 1 si nu iese din ecran
    public float PickPosition
    {
        get { return pickPosition; }
        set { pickPosition = Mathf.Clamp(value, 0f, 1f); }
    }

    [SerializeField] float pickSpeed = 3f; // cat de repede se misca speraclu stanga-dreapta

    // variabile pentru pozitia butucului (Cyllinder)
    float cyllinderPosition;
    // ne asiguram ca pozitia butucului ramane intre 0 si 1
    public float CyllinderPosition
    {
        get { return cyllinderPosition; }
        set { cyllinderPosition = Mathf.Clamp(value, 0f, MaxRotationDistance); }
    }
    [SerializeField] float cyllinderRotationSpeed = 0.4f; // Viteza de rotire a butucului cand apesi
    [SerializeField] float cyllinderRetentionSpeed = 0.4f; // Viteza cu care butucul revine la loc cand dai drumul

    [SerializeField] float rotationTolerance = 0.05f; // Marja de eroare: cat de aproape trebuie sa fii de punctul perfect ca sa inceapa sa se invarta

    [Header("Controls")]
    [SerializeField] KeyCode cancelKey = KeyCode.Escape; // apasa esc ca sa te dai batut ca esti fraier

    [Header("Inventory")]
    [SerializeField, Tooltip("Inventar aici !!!")]
    MonoBehaviour inventoryProvider;
    [SerializeField, Tooltip("Numar de speracluri pana la conectarea cu inventarul")]
    int fallbackLockpickCount = 1;

    // Cat de mult se poate deschide lacatul fizic. 1 = complet, 0.95 = aproape complet (pentru realism).
    [SerializeField]
    float maxOpenCap = 0.95f;

    // sistem de durabilitate speraclu 

    [Header("Pick Durability")]
    [SerializeField]
    int minPickUses = 7; // Minimul de utilizari posibile /speraclu
    [SerializeField]
    int maxPickUses = 20; // Maximul de utilizari posibile /speraclu
    // Cate utilizari mai are speraclu curent (se calculeaza random cand iei unul nou)
    int currentPickUsesRemaining;

    [Header("Shake(facut cu cod )")]
    [SerializeField]
    float shakeDuration = 0.15f; // Cat timp tremura speraclu cand gresesti
    [SerializeField]
    float shakeMagnitude = 0.03f; // Cat de tare tremura

    // Variabile interne pentru efectul de tremurat
    float shakeTimeRemaining = 0f;
    float visualPickOffset = 0f; // Cat de mult deviem vizual speraclu fata de pozitia lui reala

    // Eveniment ca sa anuntam restul jocului ca am terminat (true = succes, false = esec)
    public event Action<bool> OnFinished;

    Animator animator; // Referinta la animatorul care misca modelul 3D

    bool paused = false; // Daca jocul e pus pe pauza sau s-a terminat

    float sweetSpot; // SS ca sa deschizi lacatul
    [SerializeField] float leanency = 0.1f; // marja de eroare fata de SS care iti permite sa deschizi lacatul mai usor

    // Calculeaza distanta maxima la care poate ajunge butucul in functie de cat de aproape e speraclu de sweet spot
    float MaxRotationDistance
    {
        get
        {
            float raw = 1f - Mathf.Abs(sweetSpot - PickPosition) + leanency;
            return Mathf.Clamp(raw, 0f, maxOpenCap);
        }
    }

  
    float lastVerticalInput;// variabile de stare ( ca sa tin minte ce s-a intamplat frame-ul trecut)
    bool isApplyingTorque; // true = jucatorul apasa si e in zona buna (butucul se invarte)
    bool wasApplyingTorque; // tine minte daca frame-ul trecut se invartea
    bool wasAttemptingTorque; // tine minte daca frame-ul trecut jucatorul apasa pe W
    bool consumedThisAttempt; // safety measure ca sa nu consume mai multe vieti din speraclu intr-o singura apasare

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        Init();
    }

    // Resetarea jocului la valorile initiale
    void Init()
    {
        CyllinderPosition = 0f;
        PickPosition = 0f;

        // Alegem un nou SS random
        sweetSpot = UnityEngine.Random.Range(0f, 1f);

        SetupNewPick(); // Ne da un speraclu nou cu viata random
        consumedThisAttempt = false;
    }

    private void SetupNewPick()
    {
        // Calculam cata viata are noul speraclu 
        currentPickUsesRemaining = UnityEngine.Random.Range(minPickUses, maxPickUses + 1);
        Debug.Log($"New lockpick created with {currentPickUsesRemaining} uses.");
    }

    private void Update()
    {
        if (paused == true)
        { return; }

        // Daca jucatorul apasa Escape, pierde
        if (Input.GetKeyDown(cancelKey))
        {
            Lose();
            return;
        }

        // pentru rotire butuc (apasare W)  
        lastVerticalInput = Input.GetAxisRaw("Vertical");

        // Verificam daca jucatorul apasa pe W in acest frame
        bool isAttemptingTorque = lastVerticalInput > 0f;

        // Verificam daca rotatia este aplicata (apasare W + in zona buna)
        isApplyingTorque = isAttemptingTorque && IsPickInRange();

        // logica durabilitate speraclu
        // daca incepe sa incerce sa roteasca butucul
        if (isAttemptingTorque && !wasAttemptingTorque)
        {
            // Daca nu e in raza de actiune a SS-ului+ marja de eroare
            if (!IsPickInRange())
            {
                // Incepe tremuratul vizual
                TriggerSimpleShake();

                // Daca nu am taxat deja aceasta incercare, scadem o unitate de durabilitate din speraclu
                if (!consumedThisAttempt)
                {
                    ConsumePickUse();
                    consumedThisAttempt = true;
                }
            }
            else
            {
                // Daca e in zona buna nu taxam speraclul pana nu incepe efectiv sa roteasca
                consumedThisAttempt = false;
            }
        }

        // logica de taxare speraclu
        // Daca incepe efectiv sa roteasca butucul
        if (isApplyingTorque && !wasApplyingTorque)
        {
            if (!consumedThisAttempt)
            {
                ConsumePickUse();
                consumedThisAttempt = true;
            }
        }

        // cand jucatorul ia degetul de pe W, resetam siguranta pentru lucru mecanic aplicat ca sa putem taxa urmatoarea apasare
        if (!isAttemptingTorque)
            consumedThisAttempt = false;

        // ne lasa sa miscam speraclul doar daca nu forteaza butucul in acel moment
        if (!isApplyingTorque)
            Pick();

        // Apelam functiile care misca fizic obiectele si animatiile
        Cyllinder(lastVerticalInput);
        UpdateShake(Time.deltaTime);
        UpdateAnimator();

        // Salvam starea curenta pentru frame-ul urmator
        wasApplyingTorque = isApplyingTorque;
        wasAttemptingTorque = isAttemptingTorque;
    }

    // Activeaza cronometrul pentru tremurat
    private void TriggerSimpleShake()
    {
        shakeTimeRemaining = shakeDuration;
    }

    // Calculeaza efectiv cat de mult sa tremure speraclu frame-ul asta/discutie daca esti mai aproape de SS ar trebui sa tremure mai putin sau mai mult???
    private void UpdateShake(float dt)
    {
        if (shakeTimeRemaining > 0f)
        {
            shakeTimeRemaining = Mathf.Max(0f, shakeTimeRemaining - dt);
            // Cu cat a mai ramas mai putin timp, cu atat tremura mai incet (damper)
            float damper = (shakeDuration > 0f) ? (shakeTimeRemaining / shakeDuration) : 0f;
            // Calcul random pentru tremurici
            visualPickOffset = (UnityEngine.Random.value * 2f - 1f) * shakeMagnitude * damper;
        }
        else
        {
            visualPickOffset = 0f; // Daca s-a terminat timpul se opreste tremuratul
        }
    }

    // Scade viata speraclului
    private void ConsumePickUse()
    {
        // Siguranta: daca are 0 viata, se rupe
        if (currentPickUsesRemaining <= 0)
        {
            BreakPick();
            return;
        }

        currentPickUsesRemaining--;
        Debug.Log($"Lockpick used, {currentPickUsesRemaining} uses remaining.");

        if (currentPickUsesRemaining <= 0)
        {
            // Daca a ajuns la 0 acum se rupe
            BreakPick();
        }
    }

    // Verifica daca speraclul este destul de aproape de SweetSpot ca sa permita rotirea
    private bool IsPickInRange()
    {
        return Mathf.Abs(PickPosition - sweetSpot) <= rotationTolerance;
    }

    // Logica miscare butuc
    private void Cyllinder(float vertical)
    {
        // Butucul tinde mereu sa revina la pozitia 0 (rezistenta arcului)
        CyllinderPosition -= cyllinderRetentionSpeed * Time.deltaTime;

        // Daca apasam W si suntem in zona, il fortam sa se roteasca spre deschidere
        if (vertical > 0f && IsPickInRange())
        {
            CyllinderPosition += vertical * Time.deltaTime * cyllinderRotationSpeed;
        }

        // Conditia de win: daca butucul s-a rotit aproape complet
        if (CyllinderPosition >= MaxRotationDistance - 0.0001f)
        {
            Win();
        }
    }

    // Functie helper care opreste jocul si anunta rezultatul
    private void Finish(bool success)
    {
        if (paused) return;
        paused = true;
        Debug.Log(success ? "You picked the lock!" : "You failed the lockpick.");
        OnFinished?.Invoke(success);
    }

    private void Win()
    {
        Finish(true);
    }

    // Poate fi apelata din exterior ca sa fortezi pierderea
    public bool Lose()
    {
        Finish(false);
        return false;
    }

    // Logica de miscare stanga-dreapta a speraclului
    private void Pick()
    {
        // Siguranta extra: nu pot misca speraclul daca abia fortai butucul
        if (isApplyingTorque) return;

        PickPosition += Input.GetAxis("Horizontal") * Time.deltaTime * pickSpeed;
    }

    // Trimite valorile catre Animator in Unity
    private void UpdateAnimator()
    {
        if (animator != null)
        {
            // Aici combinam pozitia reala cu tremuratul (visualPickOffset) doar vizual
            float visual = Mathf.Clamp(PickPosition + visualPickOffset, 0f, 1f);
            animator.SetFloat("PickPosition", visual);
            animator.SetFloat("LockOpen", CyllinderPosition);
        }
    }

    // Inventory / lockpick helpers
    // Returneaza cate speracluri ai (din inventar sau din variabila de rezerva)
    public int GetLockpickCount()
    {
        if (inventoryProvider is ILockpickInventory inv)
            return inv.GetLockpickCount();

        // Daca nu avem inventar conectat, folosim numarul de test
        return fallbackLockpickCount;
    }

    public bool HasLockpicks()
    {
        return GetLockpickCount() > 0;
    }

    // Se apeleaza cand speraclul s-a rupt
    public void BreakPick()
    {
        // animator?.SetTrigger("BreakPick");
        // AudioSource.PlayClipAtPoint(breakClip, transform.position);

        Debug.Log("Lockpick broken");

        /*
        // initializam lockpick-ul din inventar daca exista inventar
        if (inventoryProvider is ILockpickInventory inv)
        {
            bool consumed = inv.ConsumeLockpick();
            if (!consumed)
            {
                // nu are ce sa consume deci pierde 
                Lose();
                return;
            }

            if (inv.GetLockpickCount() > 0)
            {
                ReplacePick();
            }
            else
            {
                // a consumat ultimul speraclu deci pierde
                Lose();
            }

            return;
        }
        */

        // Daca nu avem inventar scadem din variabila de test
        if (fallbackLockpickCount > 0)
        {
            fallbackLockpickCount--;
            // Daca mai avem rezerve punem unul nou altfel pierdem
            if (fallbackLockpickCount > 0) ReplacePick();
            else Lose();
        }
        else
        {
            Lose();
        }
    }

    // Inlocuieste speraclul rupt cu unul nou
    public void ReplacePick()
    {
        // animator?.SetTrigger("ReplacePick");
        // AudioSource.PlayClipAtPoint(replaceClip, transform.position);

        Debug.Log("Replacing lockpick...");

        // Resetam pozitia speraclului la 0
        PickPosition = 0f;

        // Resetam pozitia butucului la 0 daca vrem sau apar probleme de UX
        // CyllinderPosition = 0f;

        // Generam noile statistici de durabilitate
        SetupNewPick();

        // Scoatem pauza ca sa poata continua
        paused = false;

        // Resetam toate variabilele de stare ca sa nu ii ia o viata instantaneu cand reincepe
        wasApplyingTorque = false;
        wasAttemptingTorque = false;
        consumedThisAttempt = false;
    }
}