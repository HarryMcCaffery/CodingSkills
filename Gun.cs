
//an older script that I wrote, that handles some weapon stuff for my unity game. written in C#

using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using HardShellStudios.CompleteControl;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;
public enum FireModeType
{
    Safe = 0,
    Semi = 1,
    Burst = 2,
    FullAuto = 3
}
public enum WeaponType
{
    Gun = 0,
    Placeable = 1,
    Throwable = 2,
    Melee = 3,
    Grenade = 4,
    Supply = 5
}
public enum AttachmentType
{
    Secondary = 0,
    Tertiary = 1,
    Quaternary = 2,
    Melee = 3,
    Tactical = 4,
    Underbarrel = 5,
    Sight = 6,
    Grip = 7,
    Bipod = 8,
    Barrel = 9,
    Magazine = 10,
    Stock = 11,
    All = 12,
    Camo = 13
}
[System.Serializable]
public class Attachment : System.Object
{
    public GameObject obj;
    public int id;
    public AttachmentType type;
//    public GameObject useObj;
  //  public int slots;
    public Attachment(int attachmentID,Gun g, int slot)
    {
        if (WeaponArray.Instance.attachments[attachmentID].GetComponent<AttachmentInfo>().type != g.attachmentPositions[slot].GetComponent<AttachmentPosition>().type)
      //      Debug.Log(WeaponArray.Instance.attachments[attachmentID].GetComponent<AttachmentInfo>().type);
            attachmentID = 0;
        attachmentID = attachmentID % WeaponArray.Instance.attachments.Length;
        foreach (Transform t in g.attachmentPositions[slot].transform)
         if (t.gameObject != g.attachmentPositions[slot].transform)   GameObject.Destroy(t.gameObject);
        id = attachmentID;
        obj = GameObject.Instantiate(WeaponArray.Instance.attachments[id]);
    //     = g.GunModel.transform;
        g.attachments[slot] = this;
     //   slots = slot;
       obj.transform.parent = g.attachmentPositions[slot].transform;
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localEulerAngles = Vector3.zero;
        type = WeaponArray.Instance.attachments[id].GetComponent<AttachmentInfo>().type;
      GameObject  attachment = WeaponArray.Instance.attachments[id];
        if (type == AttachmentType.Camo)
        {
            if (attachment.GetComponent<AttachmentInfo>().defaultCamo)
            {

                if (g.defaultMats.Length > 0)
                {
                    foreach (Renderer r in g.affectedRenderers)
                        for (int i = 0; i < r.materials.Length; i++)
                            if (i < g.defaultMats.Length)
                                r.material = g.defaultMats[i];
                   
                }
            }
            else {
                bool go = (g.defaultMats.Length == 0);
  
                    List<Material> m = null;
                if (go) m = new List<Material>();
               
                foreach (Renderer r in g.affectedRenderers)
                    for (int i = 0; i < r.materials.Length; ++i)
                    {
                        if (go) m.Add(r.materials[i]);
                        r.material = attachment.GetComponent<AttachmentInfo>().camoMat;

                    }
                if (go) g.defaultMats = m.ToArray();
                
                // r.materials[i] = obj.GetComponent<AttachmentInfo>().camoMat;
            }
        }
    }
    public Attachment(GameObject attachment, Gun g, int slot)
    {
        // id = attachmentID;
        if (attachment.GetComponent<AttachmentInfo>() && attachment.GetComponent<AttachmentInfo>().type != g.attachmentPositions[slot].GetComponent<AttachmentPosition>().type) attachment = WeaponArray.Instance.attachments[0];
        foreach (Transform t in g.attachmentPositions[slot].transform)
            if (t.gameObject != g.attachmentPositions[slot].transform) GameObject.Destroy(t.gameObject);
        obj = GameObject.Instantiate(attachment);
        
        //     = g.GunModel.transform;
        g.attachments[slot] = this;
     //   slots = slot;
        obj.transform.parent = g.attachmentPositions[slot].transform;
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localEulerAngles = Vector3.zero;
    //    id = slot;
        type = attachment.GetComponent<AttachmentInfo>().type;
        if (type == AttachmentType.Camo)
        {
            if (attachment.GetComponent<AttachmentInfo>().defaultCamo)
            {
                if (g.defaultMats.Length > 0 )
                {

                    foreach (Renderer r in g.affectedRenderers)
                        for (int i = 0; i < r.materials.Length; i++)
                            if (i < g.defaultMats.Length)
                            { r.material = g.defaultMats[i];}
                }
            }
            else {
                bool go = (g.defaultMats.Length == 0);
              List<Material> m = null;
                if (go) m = new List<Material>();

                foreach (Renderer r in g.affectedRenderers)
                    for (int i = 0; i < r.materials.Length; ++i)
                    {
                        if (go) m.Add(r.materials[i]);
                        r.material = attachment.GetComponent<AttachmentInfo>().camoMat;

                    }
                if (go) g.defaultMats = m.ToArray();
              
                // r.materials[i] = obj.GetComponent<AttachmentInfo>().camoMat;
            }
        }
    }
}


[System.Serializable]
public class Placeable : System.Object
{
    
    [SerializeField] public float range;
    public GameObject objectPlaceHolder;
    [SerializeField] public bool remoteDetonated,detonatorOut;
    [SerializeField] public GameObject explosive,objectPreview,heldobject,detonator;
    [SerializeField] public List<RemoteExplosive> placedExplosives;
    [SerializeField] public  bool isRemote;
    [SerializeField] public bool terrainOnly,faceAwayPlayer;
    [SerializeField] public float maxAngle;
    //[SerializeField] float tempShader;
}
[System.Serializable]
public class Melee : System.Object
{
    [SerializeField] public float damage,range;
}


public class Gun : MonoBehaviour
{
      [SerializeField] public bool notAvailableInClassCreator;
    [SerializeField] public WeaponType type;
    [SerializeField] public Transform cam;
    [SerializeField] public bool AIWantsToFire, AIWantsToReload;

    [SerializeField] float sway, swaySpeed, maxSway;
    [SerializeField] float tiltOnStrafe;
 //   [SerializeField] Vector3 maxSwayVector;
    private Vector3 swayHolder;
    private float tiltHolder;

    [SerializeField] float sprintOffsetRotate,sprintBounceAmount;
    private float sprintOffsetHolder;

    [Header("Main Weapon Settings")]
    [SerializeField] public GameObject bullet;
    [SerializeField] public float fireRate, shots, burst, burstDelay, velocityAccuracy,fireDelay;
    [SerializeField] public CrossHairType crossHairType;
    [SerializeField] public float accuracy;
    [SerializeField] public FireModeType fireMode;
    [SerializeField] public Transform muzzlePos;
    [SerializeField] public Transform ejectPos;
    [SerializeField] public GameObject muzzleFlashEffect,suppressedFlashEffect;
    public bool isSuprressed;
    [SerializeField] public AudioClip fire,supressedFire, reloadSound;
    [SerializeField] public float fireVolume = 1,fireMaxDistance =200;
    [SerializeField] AudioSource reloadSource;
   
    [SerializeField] Behaviour gunShake;
    [SerializeField] public FireModeType [] firemodes;
    int currentFireMode;
    [SerializeField] bool fireFromMuzzlePos;
    [SerializeField] float equipTime;
    [Header("Character Recoil Settings")] 
    [SerializeField] bool arcRecoil;
    [SerializeField] public float shotRecoil, maxRecoil, recoilReturnTime, waitAfterFireTime;
    [SerializeField] public float recoilReduceHolder, currentRecoil, targetRecoil, horizontalRecoil, horizontalRecoilHolder, horizontalRecoilReturnSpeed, horizontalRecoilAmount;
    

    [Header("Procedural Recoil Settings")]
    [SerializeField] Vector3 randomRecoilTranslate;
      [SerializeField] Vector3 staticRecoilTranslate, randomRecoilTranslateAim,staticRecoilTranslateAim;
    Vector3 randomRecoilTranslateHolder,currentRandomRecoilTranslate,calculatedRandomRecoilTranslate;
    [SerializeField] float  proceduralRecoilAimTranslate, proceduralRecoilTranslateToTime, proceduralRecoilTranslateFromTime;

    [SerializeField]
    Vector3 randomRecoilRotate;
    [SerializeField]
    Vector3 staticRecoilRotate, randomRecoilRotateAim, staticRecoilRotateAim;
    Vector3 randomRecoilRotateHolder, currentRandomRecoilRotate, calculatedRandomRecoilRotate;
    [SerializeField] float proceduralRecoilRotateToTime, proceduralRecoilRotateFromTime;

    float lastFireTime;
    [SerializeField] Vector3 DefaultPos, DefaultModelPos, defaultModelRot;
    public float velocity;
    [SerializeField]public Transform AimPos, GunModel;
    [SerializeField] public float normalFOV, aimFOV,scopeFOV;
    [SerializeField] float fovDampHolder;

    Vector3 aimPosHolder,aimRotHolder;
    //Quaternion aimRotHolder;
    [Header("Aim Settings")]
    [SerializeField] bool cantAim;
    [SerializeField] float AimSpeed = 0.4f;
    [SerializeField] Vector2 aimSensitivity;
    public bool Aiming;
    [SerializeField] Vector3 offsetWithScope;
    public bool hasScope;
    [SerializeField] public float crossHairSize, crosshairSpreadAmount, crosshairSpreadDelay, crossHairSpreadDecreaseRate, crossHairMaxSpread;
    public HUD hud;
    GameObject BulletFireCalc;
    [SerializeField] Texture2D overlayCrossHair;


    [Header("Special Weapon Settings")]
    [SerializeField] public Placeable p;
    [SerializeField] public Melee m;
    [SerializeField] Vector3 weaponBobHip, weaponBobAim;
    WeaponBobbing b;
    public ScopeSway scopeSway;
    public bool doScopeSway;
    [Header("Ammo Settings")]
    [SerializeField] public float reloadTime;
    [SerializeField] public float ammo, magSize, totalAmmo, maxAmmo;
    [HideInInspector] public float lastReload = -100;
    [HideInInspector] public bool reloading = false;
    [Header("Attachment Weapon Settings")]
    [SerializeField]  public Attachment[] attachments;
    [SerializeField]  public AttachmentPosition[] attachmentPositions;
    Vector3 gunOffset;
    bool aimProcess;
    private  GameObject proceduralRecoilObj;
    
    [SerializeField] public GameObject disableInEditor;
    [Header("Animator Settings")]
    [SerializeField] Animation anim;
    [SerializeField] AnimationClip fireClip,reloadClip,drawClip;
    [Header("Third Person Settings")]
    public GameObject thirdPersonGun ;
    public GameObject thirdPersonMuzzlePos;
    public GameObject thirdPersonEjectPos;
    [SerializeField] public Vector3 thirdPersonPosition,thirdPersonRotation,thirdPersonScale;
    [SerializeField] public GameObject leftHandPos,rightHandPos,thirdPersonLeftHandPos,thirdPersonRightHandPos;
    [SerializeField] public bool forceFaceAim;
    [Header("Turret Settings")]
    [SerializeField] public bool isTurret;

    [SerializeField] public GameObject useObj;
    [Header("Weapon Drop Settings")]
    [SerializeField] public GameObject droppedWeapon;
    [Header("Lockon")]
    [SerializeField] bool isLockon;
    [SerializeField] float lockOnSpeed,lockOnRange;
    [SerializeField] RectTransform targIm;
    public GameObject targetLocked;
    float lockOnVal;

   [SerializeField] AudioSource lockOnToneSource;
    [SerializeField] float fullSize,LockedOnSize;
    [Header("Vehicle Settings")]
    [SerializeField] public float PhysicalRecoil;
    [SerializeField]public  Rigidbody vehicleBody;
    [Header("Animation Things")]
    public bool useHands;
    public AnimationClip equipAnim, idleAnim, fireAnim, reloadAnim;
    public GameObject copyToHands;
    public Vector3 handsPos,handsRot;
    public Vector3 gunModelPos,gunModelRot;
    public Vector3 aimHandsPos,aimHandsRot;
    public Animator gunAnimator;
  [SerializeField]  float swayAmount,swayReturnSpeed,swayWeight;
  [SerializeField] Vector3 currentSway;
    [Header("Attachment Settings")]
    public Renderer[] affectedRenderers;
     public Material [] defaultMats;
    public Collider [] otherIgnoreCols;
   [SerializeField] public bool hasThermal, hasNightvision;
    
    void Awake()
    {
        
        if (!cam) cam = transform.parent;
         
        if (!reloadSource) reloadSource = GetComponent<AudioSource>();
        if (reloadSource)
        {
            reloadSource.clip = reloadSound;
          UnityEngine.Audio.AudioMixer mixer =  (UnityEngine.Audio.AudioMixer)Resources.Load("MasterAudioMixer");

            reloadSource.outputAudioMixerGroup = mixer.FindMatchingGroups("SoundEffects")[0];
        }
        DefaultPos = transform.localPosition;
        if (GunModel)
        {
            DefaultModelPos = GunModel.localPosition;
            defaultModelRot = GunModel.localEulerAngles;

            proceduralRecoilObj = new GameObject("MeshHolder");
            proceduralRecoilObj.transform.parent = transform;
            Vector3 pos,rot;
           
           pos = proceduralRecoilObj.transform.position = GunModel.position;
           rot= GunModel.localEulerAngles;
          
      //     GunModel.parent = proceduralRecoilObj.transform;
        //    GunModel.transform.position = pos;
      //      GunModel.localEulerAngles = rot;
          //  GunModel.transform.localEulerAngles = 
       //     Transform[] t = new Transform[GunModel.childCount];

           // for (int i = 0; i < GunModel.childCount; i++)
         //   {
                
        //        t[i] = GunModel.GetChild(i);

        //    }
        //    foreach (Transform obj in t)
        //    {
        //        if (obj != proceduralRecoilObj.transform && obj.GetComponent<Renderer>()) obj.parent = proceduralRecoilObj.transform;
        //    }
        }
        if (cam) hud = cam.GetComponent<HUD>();
        if (hud)
        {
      //      BulletFireCalc = new GameObject("BulletFireCalcHelper");
      //      BulletFireCalc.transform.parent = cam.transform;
     //       BulletFireCalc.transform.localPosition = Vector3.zero;
    //        BulletFireCalc.transform.localEulerAngles = Vector3.zero;
            normalFOV = cam.GetComponent<Camera>().fieldOfView;
        }
        b = GetComponent<WeaponBobbing>();
        if (b) b.setWeaponBob(weaponBobHip);
        scopeSway = GetComponent<ScopeSway>();
        if (scopeSway && hud) scopeSway.fx = hud.f.cameraFXHolder;
        if (anim)
        {
            anim.clip = drawClip;

            anim.Play();
        }
        




    }

    // Update is called once per frame
    void Update()
    {

        if (!hud)
        {
            hud = cam.GetComponent<HUD>();
        }

        else if (hud)

        {
            //    hud.f.body.SetTrigger("DoneWithWeapon");
        }
        if (!hud || Pause.isPaused && (!hud.f.aisettings.isNpc && !hud.f.isServer)) return;
        if (!hud.f.isLocalPlayer && (!hud.f.aisettings.isNpc && !hud.f.isServer)) return;
        if (Time.time < equippedAt + equipTime) return;

        if (useHands && hud.f.isLocalPlayer)
        {
            float yRot = CrossPlatformInputManager.GetAxis("Mouse X") * hud.f.m_MouseLook.XSensitivity;
            float xRot = CrossPlatformInputManager.GetAxis("Mouse Y") * hud.f.m_MouseLook.YSensitivity;

            currentSway += new Vector3(xRot*Time.deltaTime*swayWeight,yRot*Time.deltaTime*swayWeight);
          
        }

        if (shotRecoil > 0) hud.f.recoil.y = currentRecoil;
        hud.f.recoil.x = horizontalRecoil;

        // MoveOnX = Input.GetAxis("Mouse X") * Time.deltaTime * sway;

        //   MoveOnY = Input.GetAxis("Mouse Y") * Time.deltaTime * sway;
        CalculateGunSway();
        if (hInput.GetButtonDown("CycleFireMode") && !Pause.isPaused && !Pause.outOfFocus) CycleFireMode();

        if (type == WeaponType.Placeable && hud.f.isLocalPlayer)
        {
            RaycastHit h;
            var layerMask = ~((1 << 9) | (1 << 12));
            if (Physics.Raycast(cam.position, cam.forward, out h, p.range, layerMask) && (!p.terrainOnly || h.transform.tag == "Terrain"))
            {
                Quaternion rot = Quaternion.FromToRotation(Vector3.forward, h.normal);
                Debug.DrawLine(cam.position, h.point);


                if (!p.detonatorOut)
                {
                    if (!p.objectPlaceHolder && ammo > 0)
                    {
                        p.objectPlaceHolder = Instantiate(p.objectPreview, h.point, Quaternion.FromToRotation(Vector3.up, h.normal));
                        if (p.faceAwayPlayer) p.objectPlaceHolder.transform.eulerAngles = new Vector3(p.objectPlaceHolder.transform.eulerAngles.x, hud.f.transform.eulerAngles.y, p.objectPlaceHolder.transform.eulerAngles.z);
                    }
                    else if (ammo > 0)
                    {
                        p.objectPlaceHolder.transform.position = h.point;
                        p.objectPlaceHolder.transform.rotation = Quaternion.FromToRotation(Vector3.up, h.normal);
                        if (p.faceAwayPlayer) p.objectPlaceHolder.transform.eulerAngles = new Vector3(p.objectPlaceHolder.transform.eulerAngles.x, hud.f.transform.eulerAngles.y, p.objectPlaceHolder.transform.eulerAngles.z);
                    }
                    if (((hInput.GetButtonDown("Fire1") && !hud.f.isAI) || AIWantsToFire) && ammo > 0)
                    {

                        Destroy(p.objectPlaceHolder);
                        ammo--;
                        Shoot(true, 1);
                    }
                }


            }
            else
            {
                if (p.objectPlaceHolder) Destroy(p.objectPlaceHolder);
            }
            if (hInput.GetButtonDown("Fire2") && !hud.f.isAI && p.isRemote && !hud.f.m_MouseLook.freeLook && !Pause.isPaused && !Pause.outOfFocus)
            {
                p.detonatorOut = !p.detonatorOut;
                p.detonator.SetActive(p.detonatorOut);
                p.heldobject.SetActive(!p.detonatorOut);

            }
            if (p.detonatorOut)
            {
                if (p.detonatorOut && p.objectPlaceHolder) Destroy(p.objectPlaceHolder);

                if (((hInput.GetButtonDown("Fire1") && !hud.f.isAI) || AIWantsToFire))
                {
                    Shoot(false, 1);
                    //   foreach (RemoteExplosive r in p.placedExplosives)
                    //    {
                    //      r.Explode();

                    //   }
                    //   p.placedExplosives.Clear();
                }
            }
            return;
        }

        if (hud) hud.targetCrossHairSize = crossHairSize;
        //  transform.position = cam.position;
        //    transform.rotation = cam.rotation;

        // Time.deltaTime 
        //     transform.eulerAngles = Vector3.LerpUnclamped(transform.eulerAngles, cam.eulerAngles, 0.6f);
        //  transform.rotation = Quaternion.LerpUnclamped(transform.rotation, cam.rotation, Time.deltaTime*sway);print(Vector3.Distance(transform.eulerAngles, cam.eulerAngles));
        //     transform.eulerAngles = new Vector3(Mathf.SmoothDampAngle(transform.eulerAngles.x, cam.eulerAngles.x, ref curvel.x, sway, maxSpeed), Mathf.SmoothDampAngle(transform.eulerAngles.y, cam.eulerAngles.y, ref curvel.y, sway, maxSpeed));
        Recoil();
        hud.fadeOutCrossHair = Aiming;

        if (!cantAim)
        {
            if (!SettingUtils.ToggleAim && !hud.f.isAI && AimPos && (!reloading || (reloading && hud.fadeOutCrossHair)) && !hud.f.m_MouseLook.freeLook && !Pause.isPaused && !Pause.outOfFocus)
            {
                Aiming = Input.GetButton("Fire2");
                aimProcess = (Aiming && aimPosHolder.magnitude > 0.01f || Vector3.Distance(AimPos.localPosition , GunModel.localPosition) > 0.0001);
              
            }
            if (SettingUtils.ToggleAim && hInput.GetButtonDown("Fire2") && !hud.f.isAI && AimPos && (!reloading || (reloading && hud.fadeOutCrossHair)) && !hud.f.m_MouseLook.freeLook && !Pause.isPaused && !Pause.outOfFocus)
            {
                aimProcess = true;
                Aiming = !Aiming;
                aimPosHolder = Vector3.zero;
                hud.fadeOutCrossHair = Aiming;



            }
        }
      
        if (!Aiming)
        {
            hud.f.setMouseSensitivity(Vector2.zero, true);
            //   if (normalFOV > 1) cam.GetComponent<Camera>().fieldOfView = normalFOV;
        }
      
        if (b)

        {
            if (!Aiming) b.setWeaponBob(weaponBobHip);
            else b.setWeaponBob(weaponBobAim);
        }
        if (gunShake)
        {
            gunShake.enabled = !Aiming;

        }
        if (useHands)
        {
            GunModel.transform.localPosition = gunModelPos;
            GunModel.transform.localEulerAngles = gunModelRot;
            hud.f.hands.transform.localRotation =Quaternion.Euler( handsRot);
        }
        if (Aiming && GunModel && !useHands)
        {
            if (!useHands)
            {
                if (!hasScope)
                    GunModel.localPosition = Vector3.SmoothDamp(GunModel.localPosition, AimPos.localPosition, ref aimPosHolder, AimSpeed);
                else
                    GunModel.localPosition = Vector3.SmoothDamp(GunModel.localPosition, AimPos.localPosition + offsetWithScope, ref aimPosHolder, AimSpeed);
            }
            
            if (aimFOV > 1)
                cam.GetComponent<Camera>().fieldOfView = Mathf.SmoothDamp(cam.GetComponent<Camera>().fieldOfView,aimFOV,ref fovDampHolder,AimSpeed);
        }

        else if (GunModel && !useHands)
        {
          
            GunModel.localPosition = Vector3.SmoothDamp(GunModel.localPosition, DefaultModelPos, ref aimPosHolder, AimSpeed);
            if (aimFOV > 1)
                cam.GetComponent<Camera>().fieldOfView = Mathf.SmoothDamp(cam.GetComponent<Camera>().fieldOfView, normalFOV, ref fovDampHolder, AimSpeed);

        }
        else if (Aiming && useHands)
        {
            if (!hasScope)
                hud.f.hands.transform.localPosition = Vector3.SmoothDamp(hud.f.hands.transform.localPosition, aimHandsPos, ref aimPosHolder, AimSpeed);
            else
                hud.f.hands.transform.localPosition = Vector3.SmoothDamp(hud.f.hands.transform.localPosition, aimHandsPos + offsetWithScope, ref aimPosHolder, AimSpeed);
            if (aimFOV > 1)
                cam.GetComponent<Camera>().fieldOfView = Mathf.SmoothDamp(cam.GetComponent<Camera>().fieldOfView, aimFOV, ref fovDampHolder, AimSpeed);
            //   hud.f.hands.transform.localEulerAngles = Vector3.SmoothDamp(hud.f.hands.transform.localEulerAngles,aimHandsRot,ref aimRotHolder,AimSpeed);

        }
        else if (!Aiming && useHands)
        {
            hud.f.hands.transform.localPosition = Vector3.SmoothDamp(hud.f.hands.transform.localPosition, handsPos, ref aimPosHolder, AimSpeed);
          //  hud.f.hands.transform.localEulerAngles = Vector3.SmoothDamp(hud.f.hands.transform.localEulerAngles,handsRot,ref aimRotHolder,AimSpeed);
            if (aimFOV > 1)
                cam.GetComponent<Camera>().fieldOfView = Mathf.SmoothDamp(cam.GetComponent<Camera>().fieldOfView, normalFOV, ref fovDampHolder, AimSpeed);
        }
        if (!isTurret)
        {
            proceduralRecoilObj.transform.localPosition =/* new Vector3(0, currentProceduralRecoilTranslate, 0) + */currentRandomRecoilTranslate;
            proceduralRecoilObj.transform.localRotation = Quaternion.Euler(currentRandomRecoilRotate);
           
        }
        if (scopeSway && doScopeSway)
        {
            scopeSway.sway = Aiming;
          
        }
        if (((hInput.GetButtonDown("Reload") && !hud.f.isAI) || AIWantsToReload) && lastReload + reloadTime < Time.time && totalAmmo > 0 && ammo != magSize && ammo != Mathf.Infinity && !hud.f.m_MouseLook.freeLook && !Pause.isPaused && !Pause.outOfFocus)
        {

            Reload();
        }
        if (lastReload + reloadTime < Time.time && reloading)
        {
            if (totalAmmo >= magSize)
            {


                reloading = false;
                totalAmmo -= magSize - ammo;
                ammo = magSize;
            }
            else
            {
                ammo = totalAmmo;
                totalAmmo = 0;
            }
        }
        if (lastFireTime + fireRate > Time.time || reloading) return;

        if (fireMode == FireModeType.Semi && ((hInput.GetButtonDown("Fire1") && !hud.f.isAI && !hud.f.aisettings.isNpc  && !Pause.isPaused && !Pause.outOfFocus && hud.f.isLocalPlayer) || AIWantsToFire) && ammo > 0 )
        {
            //    for (int i = 0; i < shots; i++)
            //      {
              Shoot(true, (int)shots); 
            //        else Shoot(false);
            //     }
            lastFireTime = Time.time;
        }
        if (fireMode == FireModeType.Burst && ((hInput.GetButtonDown("Fire1") && !hud.f.isAI && !hud.f.aisettings.isNpc  && !Pause.isPaused && !Pause.outOfFocus && hud.f.isLocalPlayer) || AIWantsToFire) && ammo > 0 )
        {

            StartCoroutine("Burst");
            lastFireTime = Time.time + (burstDelay * burst);
        }

        if (fireMode == FireModeType.FullAuto && ((hInput.GetButton("Fire1") && !hud.f.isAI && !hud.f.aisettings.isNpc  && !Pause.isPaused && !Pause.outOfFocus && hud.f.isLocalPlayer) || AIWantsToFire) && ammo > 0)
        {
           
            //   for (int i = 0; i < shots; i++)
            //  {
          
            Shoot(true, (int)shots);
            //  else Shoot(false,shots);
            //   }


            lastFireTime = Time.time;
        }
        if (((hInput.GetButtonDown("Fire1") && !hud.f.isAI) || AIWantsToFire) && ammo <= 0  && !Pause.isPaused && !Pause.outOfFocus)
            {
            Reload();
            }
        if (aimPosHolder.magnitude < 0.001f)

        {
            if (Aiming)
            {
                hud.f.setMouseSensitivity(aimSensitivity, false);
                //   if (aimFOV > 1) cam.GetComponent<Camera>().fieldOfView = aimFOV;
            }

            aimProcess = false;
        }
        if (isLockon)
        {
            if (targetLocked)
            {
                lockOnVal += lockOnSpeed*Time.deltaTime;

            }
            else
            {
                lockOnVal -= lockOnSpeed * Time.deltaTime;
            }

            lockOnVal = Mathf.Clamp01(lockOnVal);
            if (lockOnVal ==1 && !lockOnToneSource.isPlaying) lockOnToneSource.Play();
            else if (lockOnVal < 1) lockOnToneSource.Stop();
            float scale = Mathf.Lerp(fullSize, LockedOnSize,lockOnVal);
            targIm.localScale = new Vector3(scale,scale,scale);
        }
        //overlay scop crosshair
        if (overlayCrossHair && hud.f.isLocalPlayer)
        {
            GunModel.gameObject.SetActive(!(Aiming && !aimProcess && Vector3.Distance(GunModel.localPosition, AimPos.localPosition) < Vector3.Distance(DefaultModelPos, aimPosHolder) / 2));
            hud.overlayCrossHair.gameObject.SetActive((Aiming && !aimProcess && Vector3.Distance(GunModel.localPosition, AimPos.localPosition) < Vector3.Distance(DefaultModelPos, aimPosHolder) / 2));
            // if the scope has thermal/nightvision, fucking do that shit

                hud.f.nightVision.enabled = (hasNightvision && hud.overlayCrossHair.gameObject.activeInHierarchy);
                hud.f.thermal.enabled = (hasThermal && hud.overlayCrossHair.gameObject.activeInHierarchy);
            
            if ((Aiming && !aimProcess && Vector3.Distance(GunModel.localPosition, AimPos.localPosition) < Vector3.Distance(DefaultModelPos, aimPosHolder) / 2))
                cam.GetComponent<Camera>().fieldOfView = scopeFOV;
            else
                cam.GetComponent<Camera>().fieldOfView = normalFOV;
        }
    }
    void LateUpdate()
    {
        //if (hud && hud.f && hud.f.isLocalPlayer)
       // hud.f.hands.GetBoneTransform(HumanBodyBones.Chest).localEulerAngles = currentSway;
    }
    void FixedUpdate()
    {
        if (isLockon && hud)
        {
            RaycastHit r;
            var layerMask = ~((1 << 9) | (1 << 12));
            if (Physics.Raycast(hud.transform.position,hud.transform.forward,out r,lockOnRange,layerMask) && Aiming && !aimProcess)
            {
                if (r.collider.attachedRigidbody &&( r.collider.attachedRigidbody.transform.GetComponent<HeliController>() || r.collider.attachedRigidbody.GetComponent<UnityStandardAssets.Vehicles.Aeroplane.AeroplaneController>()))
                {
                  
                    if (r.transform.gameObject == targetLocked)
                        return;
                    else
                    {
                        targetLocked = r.transform.gameObject;
                        lockOnVal = 0;
                    }
                }
                else targetLocked = null;
            }
            else targetLocked = null;
        }
        //transform.localEulerAngles = Vector3.LerpUnclamped(transform.localEulerAngles, cam.localEulerAngles, 0.6f);
    }
    void OnGUI()
    {
   //     if (overlayCrossHair && Aiming)
      //  {
    //        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), overlayCrossHair);
      //  }
    }

    public void Shoot(bool b, int amount)
    {
        if (fireAnim && hud)
        {
          
            hud.f.hands.SetTrigger("FireWeapon");
          
        }
        if (gunAnimator) gunAnimator.SetTrigger("Fire");
        if (!Aiming)
        {
            calculatedRandomRecoilTranslate = new Vector3(Random.Range(-randomRecoilTranslate.x, randomRecoilTranslate.x), Random.Range(-randomRecoilTranslate.y, randomRecoilTranslate.y), Random.Range(-randomRecoilTranslate.z, randomRecoilTranslate.z)) + staticRecoilTranslate;
            calculatedRandomRecoilRotate = new Vector3(Random.Range(-randomRecoilRotate.x, randomRecoilRotate.x), Random.Range(-randomRecoilRotate.y, randomRecoilRotate.y), Random.Range(-randomRecoilRotate.z, randomRecoilRotate.z)) + staticRecoilRotate;
        }
        else {
            calculatedRandomRecoilTranslate = new Vector3(Random.Range(-randomRecoilTranslateAim.x, randomRecoilTranslateAim.x), Random.Range(-randomRecoilTranslateAim.y, randomRecoilTranslateAim.y), Random.Range(-randomRecoilTranslateAim.z, randomRecoilTranslateAim.z)) + staticRecoilTranslateAim;
            calculatedRandomRecoilRotate = new Vector3(Random.Range(-randomRecoilRotateAim.x, randomRecoilRotateAim.x), Random.Range(-randomRecoilRotateAim.y, randomRecoilRotateAim.y), Random.Range(-randomRecoilRotateAim.z, randomRecoilRotateAim.z)) + staticRecoilRotateAim;
        }
       
        if (fireDelay > 0)
            StartCoroutine(FireGunWithDelay(amount));
        else
            if (!fireFromMuzzlePos)
                hud.f.FireGun(amount, b, cam.position, cam.rotation);
            else
                hud.f.FireGun(amount, b, muzzlePos.position, muzzlePos.rotation);  
        if (type == WeaponType.Grenade) hud.f.body.SetTrigger("Grenade");
    }
    public void Fire(bool sound)
    {
        
        if (type == WeaponType.Melee)
        {
            RaycastHit h;
            LayerMask nine = 9;
            if (Physics.Raycast(cam.position, cam.forward, out h, m.range, nine.value))
            {
                if (h.transform.GetComponent<Target>() || h.transform.GetComponent<VehicleHealth>() || h.transform.GetComponent<PlayerHitBox>()) hud.f.hitMark();
                if (h.transform.GetComponent<ObjectEffects>() && h.transform.GetComponent<ObjectEffects>().bulletHitEffect) Instantiate(h.transform.GetComponent<ObjectEffects>().bulletHitEffect, transform.position, Quaternion.FromToRotation(Vector3.forward, h.normal));

                h.transform.SendMessage("ApplyDamage", new DamageInfo(m.damage, hud.f, DamageType.Melee), SendMessageOptions.DontRequireReceiver);
            }
            if (Physics.Raycast(cam.position, cam.forward, out h, m.range, 1 << LayerMask.NameToLayer("HitBox")))
            {
                hud.f.hitMark();
                if (h.transform.GetComponent<ObjectEffects>() && h.transform.GetComponent<ObjectEffects>().bulletHitEffect) Instantiate(h.transform.GetComponent<ObjectEffects>().bulletHitEffect, transform.position, Quaternion.FromToRotation(Vector3.forward, h.normal));
                h.transform.SendMessage("ApplyDamage", new DamageInfo(m.damage, hud.f, DamageType.Melee), SendMessageOptions.DontRequireReceiver);
            }



        }
        else
        {
            float angle = Random.Range(0, 90);
            // Instantiate(bullet, muzzlePos.position, muzzlePos.rotation);
            GameObject g = Instantiate(bullet) as GameObject;
            g.transform.position = cam.position;
            float actualAccuracy = accuracy + (velocity * velocityAccuracy);//, accuracy.y + (velocity * velocityAccuracy));
            float angleConst = Random.Range(-actualAccuracy, actualAccuracy);
            g.transform.eulerAngles = cam.eulerAngles + new Vector3(angleConst * Mathf.Cos(angle), angleConst * Mathf.Sin(angle));
            //   BulletFireCalc.transform.localEulerAngles = new Vector3(Random.Range(-accuracy.x*velocityAccuracy,accuracy.x*velocityAccuracy),0,Random.Range(0, 360));
            //     g.transform.eulerAngles = BulletFireCalc.transform.eulerAngles;
            // g.transform.eulerAngles =
            g.GetComponent<Bullet>().Firer = hud.f;
            foreach (Collider c in hud.f.hitboxes)
            {
                Physics.IgnoreCollision(g.GetComponent<Collider>(), c);
            }
            foreach (Collider c in otherIgnoreCols)
            {
                Physics.IgnoreCollision(g.GetComponent<Collider>(), c);
            }
            g.SendMessage("Activate");
            hud.expandCursor(crosshairSpreadAmount, crossHairSpreadDecreaseRate, crosshairSpreadDelay, crossHairMaxSpread);
            if (sound)
            {
                SoundEffect.CreateSound(fire, cam.position, 10);
                targetRecoil += shotRecoil;
                ammo--;
                targetRecoil = Mathf.Min(maxRecoil, targetRecoil);
                recoilReduceHolder = 0;
                Instantiate(muzzleFlashEffect, muzzlePos.position, muzzlePos.rotation);

            }
        }
        AIWantsToFire = false;
    }

    IEnumerator Burst()
    {
        for (int i = 0; i < burst; i++)
        {
            //   CmdFire(true);
            yield return new WaitForSeconds(burstDelay);
        }
    }
    void OnDisable()
    {
        if (p.objectPlaceHolder) Destroy(p.objectPlaceHolder);
        targetRecoil = 0;
        currentRecoil = 0;
        if (GunModel && !useHands) GunModel.transform.localPosition = DefaultModelPos;
        if (useHands && GunModel && hud) GunModel.gameObject.SetActive(false);
        Aiming = false;
        reloading = false;
        lastReload = -100;
        hud = null;
        aimPosHolder = Vector3.zero;
        
        if (hud) hud.fadeOutCrossHair = false;
        if (thirdPersonGun) thirdPersonGun.SetActive(false);
      if (hud && hud.f)  hud.f.setMouseSensitivity(Vector2.zero, true);
        if (cam && cam.GetComponent<Camera>()) cam.GetComponent<Camera>().fieldOfView = 60;
        if (!hud) hud = transform.parent.GetComponent<HUD>();
        if (hud)
        {
         //   hud.f.hands.SetTrigger("DoneWithWeapon");
            hud.f.body.SetTrigger("DoneWithWeapon");
        }
       if (useHands && hud)
        {
            hud.f.hands.transform.localPosition = oldHandsPos;
        }

    }
   
    Vector3 oldHandsPos;
    void OnEnable()
    {
        if (!hud) hud = transform.parent.GetComponent<HUD>();
        if (hud)   hud.fadeOutCrossHair = false;
if (thirdPersonGun)         thirdPersonGun.SetActive(true);
        if (useHands && hud)
        {
          
       AnimatorOverrideController   r = new AnimatorOverrideController(hud.f.body.runtimeAnimatorController);

            if (equipAnim)
            {equipAnim.name = "KnifeEquip"; r["KnifeEquip"] = equipAnim; }
            if (fireAnim)
            { fireAnim.name = "KnifeSwing"; r["KnifeSwing"] = fireAnim; }
                if (idleAnim)
                { idleAnim.name = "Knife Idle"; r["Knife Idle"] = idleAnim; }
                if (reloadAnim)
                { reloadAnim.name = "Basic_Reload"; r["Basic_Reload"] = reloadAnim; }
            hud.f.body.runtimeAnimatorController = r;
            r = new AnimatorOverrideController(hud.f.hands.runtimeAnimatorController);

            if (equipAnim)
            { equipAnim.name = "KnifeEquip"; r["KnifeEquip"] = equipAnim; }
            if (fireAnim)
            { fireAnim.name = "KnifeSwing"; r["KnifeSwing"] = fireAnim; }
            if (idleAnim)
            { idleAnim.name = "Knife Idle"; r["Knife Idle"] = idleAnim; }
            if (reloadAnim)
            { reloadAnim.name = "Basic_Reload"; r["Basic_Reload"] = reloadAnim; }
           
            hud.f.hands.runtimeAnimatorController = r;
          
          
            if ( hud.f.isLocalPlayer) hud.f.hands.gameObject.SetActive(true);

                hud.f.hands.ResetTrigger("DoneWithWeapon");
                hud.f.body.ResetTrigger("DoneWithWeapon");
                hud.f.body. SetTrigger("EquipWeapon");
                hud.f.hands.SetTrigger("EquipWeapon");
            oldHandsPos = hud.f.hands.transform.localPosition;
            hud.f.hands.transform.localPosition = handsPos;
            
            
        }
    if (GunModel)    GunModel.gameObject.SetActive(true);
        equippedAt = Time.time;
        
        
    }
    float equippedAt;
   void CycleFireMode()
    {
        currentFireMode++;
        if (firemodes == null || firemodes.Length == 0)
        {
            fireMode = FireModeType.Semi;
            return;
        }
        if ( currentFireMode >= firemodes.Length) currentFireMode = 0;

        fireMode = firemodes[currentFireMode];

    }
    
        void Reload()
    {
        lastReload = Time.time;
        Aiming = false;
      if (reloadSource)  reloadSource.Play();
        reloading = true;
        AIWantsToReload = false;
        hud.f.setMouseSensitivity(Vector2.zero, true);
        if (anim)
        {
            anim.clip = reloadClip;
            anim.Play();
        }
    }
   [HideInInspector] public float endpos;
    float deltaCam;
    void Recoil()
    {

  
        if (arcRecoil)
        {
            if (Time.time > lastFireTime && Time.time < lastFireTime + recoilReturnTime)
            {
                currentRecoil = Mathf.SmoothDamp(currentRecoil, targetRecoil, ref recoilReduceHolder, recoilReturnTime);
            }
            else
            {

                currentRecoil = Mathf.SmoothDamp(currentRecoil, 0, ref recoilReduceHolder, recoilReturnTime);
                targetRecoil = currentRecoil;
            }
        }
        else
        {
            if (Time.time < lastFireTime + waitAfterFireTime)
            {
               
                currentRecoil = Mathf.SmoothDamp(currentRecoil, targetRecoil, ref recoilReduceHolder, 0.05f);
            }
            else
            {
               // hud.f.m_MouseLook.m_CameraTargetRot *= Quaternion.Euler(-currentRecoil, 0, 0f);
                //currentRecoil = 0;
                targetRecoil = currentRecoil;
                currentRecoil = Mathf.SmoothDamp(currentRecoil, 0, ref recoilReduceHolder, recoilReturnTime);
            
               
            }
            
          
         
        }
        
        horizontalRecoil = Mathf.SmoothDamp(horizontalRecoil, 0, ref horizontalRecoilHolder, horizontalRecoilReturnSpeed);
     //   if (!Aiming)
        {
            if (Time.time > lastFireTime && Time.time < lastFireTime + proceduralRecoilTranslateFromTime)
            {
          //      currentProceduralRecoilTranslate = Mathf.SmoothDamp(currentProceduralRecoilTranslate, proceduralRecoilTranslate, ref proceduralRecoilTranslateHolder, proceduralRecoilTranslateToTime);
                currentRandomRecoilTranslate = Vector3.SmoothDamp(currentRandomRecoilTranslate,calculatedRandomRecoilTranslate,ref randomRecoilTranslateHolder, proceduralRecoilTranslateToTime);

            }
            else
            {

        //        currentProceduralRecoilTranslate = Mathf.SmoothDamp(currentProceduralRecoilTranslate, 0, ref proceduralRecoilTranslateHolder, proceduralRecoilTranslateFromTime);
                currentRandomRecoilTranslate = Vector3.SmoothDamp(currentRandomRecoilTranslate, Vector3.zero, ref randomRecoilTranslateHolder, proceduralRecoilTranslateFromTime);
            }
            if (Time.time > lastFireTime && Time.time < lastFireTime + proceduralRecoilRotateFromTime)
            {
                //      currentProceduralRecoilTranslate = Mathf.SmoothDamp(currentProceduralRecoilTranslate, proceduralRecoilTranslate, ref proceduralRecoilTranslateHolder, proceduralRecoilTranslateToTime);
                currentRandomRecoilRotate = Vector3.SmoothDamp(currentRandomRecoilRotate, calculatedRandomRecoilRotate, ref randomRecoilRotateHolder, proceduralRecoilRotateToTime);

            }
            else
            {

                //        currentProceduralRecoilTranslate = Mathf.SmoothDamp(currentProceduralRecoilTranslate, 0, ref proceduralRecoilTranslateHolder, proceduralRecoilTranslateFromTime);
                currentRandomRecoilRotate = Vector3.SmoothDamp(currentRandomRecoilRotate, Vector3.zero, ref randomRecoilRotateHolder, proceduralRecoilTranslateFromTime);
            }
        }
   //     else
        {
            if (Time.time > lastFireTime && Time.time < lastFireTime + proceduralRecoilTranslateFromTime)
            {
        //        currentProceduralRecoilTranslate = Mathf.SmoothDamp(currentProceduralRecoilTranslate, proceduralRecoilAimTranslate, ref proceduralRecoilTranslateHolder, proceduralRecoilTranslateToTime);
            }
            else
            {

           //     currentProceduralRecoilTranslate = Mathf.SmoothDamp(currentProceduralRecoilTranslate, 0, ref proceduralRecoilTranslateHolder, proceduralRecoilTranslateFromTime);

            }
        }
    }
    void CalculateGunSway()
    {
        // if (hud.f.m_IsWalking)
        //  {
        /*    float MoveOnX = 0, MoveOnY = 0;
            if (Time.time > 0.5)
            {
                if (!Aiming)
                {
                    MoveOnX =Mathf.Clamp( Input.GetAxis("Mouse X") * Time.deltaTime * swaySpeed,-0.05f, 0.05f);
                    MoveOnY = Mathf.Clamp( Input.GetAxis("Mouse Y") * Time.deltaTime * swaySpeed,-0.05f, 0.05f);
                }
                Vector3 NewGunPos = new Vector3(Mathf.Min(Mathf.Abs(DefaultPos.x + MoveOnX), maxSway) * Mathf.Sign(DefaultPos.x + MoveOnX), Mathf.Min(Mathf.Abs(DefaultPos.y + MoveOnY), maxSway) * Mathf.Sign(DefaultPos.y + MoveOnY), DefaultPos.z);
                //        NewGunPos =  Vector3.Min(maxSwayVector,NewGunPos);
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, Mathf.SmoothDampAngle(transform.localEulerAngles.z, -tiltOnStrafe * hud.f.m_Input.x, ref tiltHolder, 0.2f));

                transform.localPosition = Vector3.SmoothDamp(transform.localPosition, NewGunPos, ref swayHolder, swaySpeed);

            }*/
        //    }
        //   else
        // {

        //  }

    }
    IEnumerator FireGunWithDelay(int amount)
    {

        yield return new WaitForSeconds(fireDelay);
        if (!fireFromMuzzlePos)
            hud.f.FireGun(amount, b, cam.position, cam.rotation);
        else
            hud.f.FireGun(amount, b, muzzlePos.position, muzzlePos.rotation);
    }
}