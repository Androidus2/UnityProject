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
    // --- ADAUGARE NOUA: Variabile pentru sunet (Generate automat) ---
    [Header("Audio Clips (Drag & Drop here)")]
    [SerializeField] private AudioClip unlockClip; // Trage sunetul de succes aici
    [SerializeField] private AudioClip breakClip;  // Trage sunetul de rupere aici
    [SerializeField] private AudioClip rattleClip; // Trage sunetul de tremurat aici

    private AudioSource audioSource;

    // --- ADAUGARE NECESARA: Referinte pentru noul sistem de inventar ---
    [Header("New Inventory System")]
    [SerializeField]
    private ItemObject lockpickItemReference; // TRAGE AICI ItemObject-ul tau "Lockpick"
    private InventoryObject connectedInventory; // Aici primim inventarul de la Chest

    // -------------------------------------------------------------------

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
        // --- FIX PENTRU PARENT: Folosim GetComponentInChildren ---
        animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        // 1. Facem rost de componenta audio
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        // --- AM STERS GENERAREA AUTOMATA ---
        // Nu mai avem nevoie de GenerateMechanicalUnlock(), etc.
        // Asigura-te doar ca ai pus sunetele in Inspectorul din Unity!

        if (unlockClip == null) Debug.LogWarning("Nu ai pus sunetul de Unlock in Inspector!");
        if (breakClip == null) Debug.LogWarning("Nu ai pus sunetul de Break in Inspector!");
        if (rattleClip == null) Debug.LogWarning("Nu ai pus sunetul de Rattle in Inspector!");

        Init();
    }

    // --- FUNCTIE NOUA: Primim inventarul de la Chest ---
    public void SetInventory(InventoryObject inventory)
    {
        this.connectedInventory = inventory;
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

        // --- PLAY AUDIO TREMURAT ---
        if (audioSource != null && rattleClip != null && !audioSource.isPlaying)
        {
            // Randomizam putin pitch-ul ca sa nu sune identic de fiecare data
            audioSource.pitch = UnityEngine.Random.Range(0.85f, 1.15f);
            audioSource.PlayOneShot(rattleClip);
            audioSource.pitch = 1f; // Resetam la normal
        }
        // ---------------------------
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

        // --- PLAY AUDIO SUCCESS ---
        if (success && audioSource != null && unlockClip != null)
        {
            audioSource.PlayOneShot(unlockClip);
        }
        // --------------------------

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
        // --- FIX PENTRU EROARE CONSOLA: Verificam daca are controller inainte sa setam valori ---
        if (animator != null && animator.runtimeAnimatorController != null)
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
        // --- PLAY AUDIO RUPERE ---
        if (audioSource != null && breakClip != null)
        {
            audioSource.PlayOneShot(breakClip);
        }
        // -------------------------

        // animator?.SetTrigger("BreakPick");
        // AudioSource.PlayClipAtPoint(breakClip, transform.position);

        Debug.Log("Lockpick broken");

        // --- INTEGRARE NOUA CU INVENTARUL TAU ---
        if (connectedInventory != null && lockpickItemReference != null)
        {
            // Incercam sa stergem 1 bucata din inventar
            bool removed = connectedInventory.RemoveItem(lockpickItemReference);

            if (removed)
            {
                // Verificam daca jucatorul mai are ALTE speracluri ramase
                bool hasMore = false;
                foreach (var slot in connectedInventory.GetItems())
                {
                    if (slot.GetItem() == lockpickItemReference)
                    {
                        hasMore = true;
                        break;
                    }
                }

                if (hasMore) ReplacePick();
                else Lose();
            }
            else
            {
                Lose();
            }
            return;
        }

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
        // AICI URMEAZA SA PUI ALTE ANIMATII
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

    // --- FUNCTII GENERATOARE DE SUNET (ACTUALIZATE - FIZICA METALICA) ---

    // Sunet de succes: "Clunk" + "Ping" metalic cu decay rapid
    private AudioClip GenerateMechanicalUnlock()
    {
        int sampleRate = 44100;
        float duration = 0.4f; // Puternic si scurt
        int sampleCount = (int)(sampleRate * duration);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / sampleRate;

            // 1. Lovitura joasa a mecanismului (Low Thud) - scade in frecventa
            float freqLow = 150f - (t * 300f);
            float thud = Mathf.Sin(2 * Mathf.PI * freqLow * t) * Mathf.Exp(-15f * t);

            // 2. Rezonanta metalica (High Ping)
            float metal = Mathf.Sin(2 * Mathf.PI * 1200 * t)
                        + 0.5f * Mathf.Sin(2 * Mathf.PI * 1740 * t)
                        + 0.3f * Mathf.Sin(2 * Mathf.PI * 3200 * t);

            // Envelope foarte scurt (se stinge repede)
            metal *= Mathf.Exp(-20f * t);

            // Combinam:
            samples[i] = (thud * 0.6f) + (metal * 0.4f);
        }

        AudioClip clip = AudioClip.Create("RealUnlock", sampleCount, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    // Sunet de rupere: Snap scurt, zgomotos
    private AudioClip GenerateMetalSnap()
    {
        int sampleRate = 44100;
        float duration = 0.15f; // Foarte scurt
        int sampleCount = (int)(sampleRate * duration);
        float[] samples = new float[sampleCount];

        System.Random rng = new System.Random();

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / sampleRate;

            // Zgomot alb
            float noise = ((float)rng.NextDouble() * 2f - 1f);

            // Textura metalica (modulam zgomotul)
            float texture = Mathf.Sin(2 * Mathf.PI * 800 * t);

            // Anvelopa brutala (se opreste brusc)
            float envelope = Mathf.Exp(-40f * t);

            samples[i] = noise * texture * envelope;
        }

        AudioClip clip = AudioClip.Create("MetalSnap", sampleCount, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }

    // REPARAT: Sunet de tremurat (Rattle) - Fara zgomot alb (TV), folosim vibratie
    private AudioClip GenerateMetalScrape()
    {
        int sampleRate = 44100;
        float duration = 0.25f;
        int sampleCount = (int)(sampleRate * duration);
        float[] samples = new float[sampleCount];

        for (int i = 0; i < sampleCount; i++)
        {
            float t = (float)i / sampleRate;

            // 1. Tonul metalic de baza (ca o coarda de chitara foarte scurta)
            float metalTone = Mathf.Sin(2 * Mathf.PI * 1000 * t);

            // 2. Vibratia (Tremuratul) - 40 de ori pe secunda
            // Asta face sunetul sa fie "Drrr-Drrr-Drrr" in loc de "Fâââș"
            float vibration = Mathf.Sin(2 * Mathf.PI * 40 * t);

            // Adaugam o usoara distorsiune (Square wave approximation) ca sa sune mai dur
            if (vibration > 0) vibration = 1f; else vibration = -1f;

            // Combinam: Tonul metalic este intrerupt de vibratie
            samples[i] = metalTone * vibration * 0.3f; // 0.3 volum mai mic

            // Fade out
            if (i > sampleCount - 1000) samples[i] *= (sampleCount - i) / 1000f;
        }

        AudioClip clip = AudioClip.Create("MetalRattle", sampleCount, 1, sampleRate, false);
        clip.SetData(samples, 0);
        return clip;
    }
}