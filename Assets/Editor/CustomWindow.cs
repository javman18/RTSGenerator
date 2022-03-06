using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEditorInternal;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
public class CustomWindow : EditorWindow
{
    
    Vector2 scrollView;
    private static int maxTags = 10000;
    private static int maxLayers = 31;
    private static int maxSortingLayers = 31;

    //toolbar
    string[] toolbarString = { "Agent basics", "Behaviors", "Squad", "Animator" };
    int toolbarSelection = 0;
    static CustomWindow window;
    string objectName;
    //steering behaviors
    public bool isSeek;
    public bool isFlee;
    public bool isPursue;
    public bool isEvade;
    public bool isArrive;
    public bool isCohesion;

    public float maxForce = 120;
    public float maxVel = 100;
    public float mass = 0.3f;

    public float fleePerception;
    public float pursuePerception;

    public float healthAmount;

    public int carryLimit = 0;
    //RTS behaviors
    public bool isCollector;
    public bool colectScrap;
    public bool colectMetal;
    public bool colectCopper;
    public bool canshoot;
    public bool isTitan;
    public bool isMelee;
    public float timeBetweenShots = 0.1f;
    public float bulletForce = 30;
    public int ammo;
    public float shootingRange = 100;
    public bool noAmmo;
    //squad
    public int rank;
    public int leaderID;
    public int team;
    Object agentSprite;

    //animator
    
    public string animName;
    public string animControllerName;
    //public Texture2D[] sprites;           
    public int frameHeight;
    public int frameWidth;
    public Texture2D pasteTo;
    AnimationClip clip;
    public Sprite[] _sprites;
    UnityEditor.Animations.AnimatorController controller;
    public int firstState;
    public int lastState;
    UnityEditor.Animations.AnimatorStateMachine rootStateMachine;
    SerializedObject so;
    public bool hasCondition;
    public bool playerMove;
    public bool playerStopped;
    public bool anyState;
    public bool exitTime;
    public int exitTimeValue;
    public float bulletDamage;
    public float meleeDamage;
    public bool isLeaderDead;
    public bool enemyClose;
    GameObject createdAgent;
    Editor gameObjectEditor;
    GameObject agent;
    [MenuItem("Window/RTS Agent Designer")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow<CustomWindow>("Agent");
        window = (CustomWindow)GetWindow(typeof(CustomWindow));
        
        //window.minSize = new Vector2(300, 400);
        //window.maxSize = new Vector2(600, 800);

    }
    void OnEnable()
    {
        ScriptableObject target = this;
        so = new SerializedObject(target);
    }

    Rect prect = new Rect();
    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        toolbarSelection = GUILayout.Toolbar(toolbarSelection, toolbarString);
        GUILayout.EndHorizontal();
        if(toolbarSelection == 0)
        {
            objectName = EditorGUILayout.TextField("Agent name", objectName);
            agentSprite = EditorGUILayout.ObjectField("Agent aspect", agentSprite, typeof(Sprite), true);
            maxForce = EditorGUILayout.FloatField("Max force", maxForce);
            maxVel = EditorGUILayout.FloatField("Max velocity", maxVel);
            mass = EditorGUILayout.Slider("Mass", mass, 0.01f, 1f);
            healthAmount = EditorGUILayout.FloatField("hp", healthAmount);
            

        }
        else if(toolbarSelection == 1){

            scrollView = EditorGUILayout.BeginScrollView(scrollView);
            isTitan = EditorGUILayout.Toggle("Titan", isTitan);
            //isSeek = EditorGUILayout.Toggle("Seek", isSeek);
            isFlee = EditorGUILayout.Toggle("Flee", isFlee);
            if (isFlee)
            {
                
                fleePerception = EditorGUILayout.FloatField("Flee range", fleePerception);
            }
            EditorGUILayout.Space();
            isPursue = EditorGUILayout.Toggle("Pursue", isPursue);
            if (isPursue)
            {
                pursuePerception = EditorGUILayout.FloatField("Pursue range", pursuePerception);
               
            }
            EditorGUILayout.Space();
            isEvade = EditorGUILayout.Toggle("Evade", isEvade);
            
            isCollector = EditorGUILayout.Toggle("Collector", isCollector);
            if (isCollector)
            {
                
                carryLimit = EditorGUILayout.IntField("Capacidad de carga", carryLimit);
                EditorGUILayout.HelpBox("Recurso que recoge", MessageType.Info, false);
                colectCopper = EditorGUILayout.Toggle("Copper", colectCopper);
                colectMetal = EditorGUILayout.Toggle("Metal", colectMetal);
                colectScrap = EditorGUILayout.Toggle("scrap", colectScrap);
            }
            if(isCollector && isFlee)
            {
                EditorGUILayout.HelpBox("Condiciones de escape", MessageType.Info, false);
                enemyClose = EditorGUILayout.Toggle("Enemigo Cerca", enemyClose);
            }
            EditorGUILayout.Space();
            isMelee = EditorGUILayout.Toggle("Is Attacker", isMelee);
            if (isMelee && !canshoot)
            {
                meleeDamage = EditorGUILayout.FloatField("Melee damage", meleeDamage);
                EditorGUILayout.HelpBox("Comportamiento de aproximacion al objetivo", MessageType.Info, false);
                isArrive = EditorGUILayout.Toggle("Arrive", isArrive);
                isSeek = EditorGUILayout.Toggle("Seek", isSeek);
            }
            canshoot = EditorGUILayout.Toggle("Shooter", canshoot);
            if (canshoot)
            {
                timeBetweenShots = EditorGUILayout.FloatField("Rate of fire", timeBetweenShots);
                bulletForce = EditorGUILayout.FloatField("Bullet speed", bulletForce);
                shootingRange = EditorGUILayout.FloatField("Shooting Range", shootingRange);
                ammo = EditorGUILayout.IntField("Ammo", ammo);
                bulletDamage = EditorGUILayout.FloatField("Damage", bulletDamage);
            }
            if( isFlee && canshoot)
            {
                EditorGUILayout.HelpBox("Condiciones de escape", MessageType.Info, false);
                noAmmo = EditorGUILayout.Toggle("no ammo", noAmmo);
                isLeaderDead = EditorGUILayout.Toggle("leader dead", isLeaderDead);
            }
            if (isMelee && canshoot)
            {
                EditorGUILayout.HelpBox("Condiciones de ataque cuarpo a cuerpo", MessageType.Info, false);
                noAmmo = EditorGUILayout.Toggle("no ammo", noAmmo);
                if (noAmmo)
                {
                    EditorGUILayout.HelpBox("Comportamiento de aproximacion al objetivo", MessageType.Info, false);
                    isArrive = EditorGUILayout.Toggle("Arrive", isArrive);
                    isSeek = EditorGUILayout.Toggle("Seek", isSeek);
                }
                //isLeaderDead = EditorGUILayout.Toggle("leader dead", noAmmo);
            }
            
            
            EditorGUILayout.EndScrollView();

        }
        else if(toolbarSelection == 2)
        {
            rank = EditorGUILayout.IntField("Rank", rank);
            leaderID = EditorGUILayout.IntField("Squad", leaderID);
            team = EditorGUILayout.IntField("Team", team);
            GameObject a = null;
            if (GUILayout.Button("Create Agent"))
            {

                TagLayerManager.CreateLayer("Agents");
                TagLayerManager.CreateLayer("Minimap");
                TagLayerManager.CreateLayer("Walls");
                TagLayerManager.CreateLayer("FogOfWarMain");
                TagLayerManager.CreateLayer("FogOfWarSecondary");
                TagLayerManager.CreateLayer("hb");
                TagLayerManager.CreateTag("Agent");
                TagLayerManager.CreateTag("tile");
                TagLayerManager.CreateTag("FogOfWar");
                TagLayerManager.CreateTag("storeage");
                TagLayerManager.CreateTag("Leader");
                TagLayerManager.CreateTag("Wall");
                TagLayerManager.CreateTag("Resource");
                TagLayerManager.CreateTag("Spawn");
                SaveObjectAsPrefab(agent);
                
            }

            //a = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Resources/Agents/" + objectName + ".prefab");
            //if (a != null)
            //{
            //    a.transform.localScale = new Vector3(1f, 1f, 1f);
            //    if (gameObjectEditor == null)
            //    {
            //        gameObjectEditor = Editor.CreateEditor(a);
            //    }
            //    gameObjectEditor.OnPreviewGUI(GUILayoutUtility.GetRect(300, 300), EditorStyles.whiteLabel);
            //}
            


            

        }
        else if (toolbarSelection == 3)
        {
            scrollView = EditorGUILayout.BeginScrollView(scrollView);
            EditorGUILayout.HelpBox("Primero crear el controller y despues las animaciones", MessageType.Info, false);
           
          
            animControllerName = EditorGUILayout.TextField("Animation Controller Name", animControllerName);
            EditorGUILayout.HelpBox("Solamente crear un animator controller por agente.  nota: el nombre del controller tiene que ser el mismo al del agente", MessageType.Info, false);
            if (GUILayout.Button("Create Controller"))
            {

                CreateController();


            }
            so.Update();
            EditorGUILayout.HelpBox("poner nombre a la animacion con el nombre del agente separado por '_' y el nombre de la animacion ej. collector_idle", MessageType.Info, false);
            animName = EditorGUILayout.TextField("Animation Name", animName);
            SerializedProperty spriteProp = so.FindProperty("_sprites");
            EditorGUILayout.HelpBox("Arrastrar los sprites que se quieran utilizar para la animacion", MessageType.Info, false);
            EditorGUILayout.PropertyField(spriteProp, true);
            so.ApplyModifiedProperties();
            
            if (GUILayout.Button("Create Animation"))
            {
                makeAnimation();
                AddState(controller);


            }
            EditorGUILayout.HelpBox("Cuando se tengan todas las animaciones requeridas crear las transiciones nota: al tratarse de la creacion de agentes para juegos RTS entonces solo hay 2 animaciones basicas que son 'idle' y 'walk'" +
                " si se quiere que un agente tenga un comportamiento de ataque cuerpo a cuerpo, entonces crear la animacion correspondiente", MessageType.Info, false);
            EditorGUILayout.HelpBox("Las transiciones se conectan siguiendo el orden de creacion de las animaciones" +
                " empezando desde 0, por ejemplo, si se crea primero la animacion 'idle' y despues la animacion 'walk' y se quere una transicion de idle a walk, entonces es " +
                " 'from 0' 'to 1'", MessageType.Info, false);
            firstState = EditorGUILayout.IntField("from", firstState);
            lastState = EditorGUILayout.IntField("to", lastState);
            EditorGUILayout.HelpBox("si se quiere que la animacion tenga una condicion, entonces marcar la casilla 'Condition?' seguido por la condicion" +
                " nota: al tratarse de la creacion de agentes para juegos RTS entonces solo hay 2 animaciones basicas que son 'idle' y 'walk', finalmente presionar el boton 'Create Transition'", MessageType.Info, false);

            hasCondition = EditorGUILayout.Toggle("Condition?", hasCondition);
            playerMove = EditorGUILayout.Toggle("Agent started moving", playerMove);
            playerStopped = EditorGUILayout.Toggle("Agent stopped moving", playerStopped);
            EditorGUILayout.HelpBox("Si se creo la animacion de ataque cuerpo a cuerpo, entonces activar las casillas 'Any State' y 'Has Exit time' y establecer el valor a 1" +
                " finalmente poner que la transicion es desde la animacion de ataque a la animacion idle y presionar Create Transition", MessageType.Info, false);

            anyState = EditorGUILayout.Toggle("Any State", anyState);
            exitTime = EditorGUILayout.Toggle("Has Exit Time", exitTime);
            exitTimeValue = EditorGUILayout.IntField("Exit Time Value", exitTimeValue);
            if (GUILayout.Button("Create transition"))
            {
                SetTransition();
            }
            EditorGUILayout.EndScrollView();
        }
    }

    void SaveObjectAsPrefab(GameObject agent)
    {
        agent = new GameObject();

        agent.gameObject.name = objectName;
        agent.AddComponent<SpriteRenderer>();
        if (agentSprite != null)
        {
            agent.GetComponent<SpriteRenderer>().sprite = (Sprite)agentSprite;
        }

        agent.AddComponent<Rigidbody2D>();
        agent.GetComponent<Rigidbody2D>().gravityScale = 0;
        agent.GetComponent<Rigidbody2D>().freezeRotation = true;
        agent.AddComponent<BoxCollider2D>();
        //agent.GetComponent<BoxCollider2D>().offset = new Vector2(0.1f, 0f);
        //agent.GetComponent<BoxCollider2D>().size = new Vector2(mass *15, mass*15);
        agent.AddComponent<PathAgent>();
        agent.AddComponent<AgentManager>();
        agent.AddComponent<Animator>();
        agent.AddComponent<Animation>();
        AgentManager agentM = agent.GetComponent<AgentManager>();
        agentM.maxForce = maxForce;
        agentM.maxVel = maxVel;
        agentM.mass = mass;
        agentM.seekPerception = pursuePerception;
        agentM.fleePerception = fleePerception;
        agentM.isSeek = isSeek;
        agentM.isFlee = isFlee;
        agentM.isPursue = isPursue;
        agentM.isEvade = isEvade;
        agentM.isArrive = isArrive;
        agentM.isCollector = isCollector;
        agentM.isShooter = canshoot;
        agentM.bulletForce = bulletForce;
        agentM.startTimeBtwShots = timeBetweenShots;
        agentM.ammo = ammo;
        agentM.shootingRaange = shootingRange;
        agentM.team = team;
        agentM.isTitan = isTitan;
        agentM.isAtacker = isMelee;
        agentM.rank = rank;
        agentM.squad = leaderID;
        agentM.healthAmount = healthAmount;
        agentM.bulletDamage = bulletDamage;
        agentM.meleeDamage = meleeDamage;
        agentM.resourcesCarryLimt = carryLimit;
        agent.GetComponent<SpriteRenderer>().sortingLayerName = "Agents";
        agent.transform.localScale = new Vector3(7f, 7f, 1f);
        agent.transform.tag = "Agent";
        
        string path = "Assets/Resources/Agents/" + agent.name + ".prefab";
        path = AssetDatabase.GenerateUniqueAssetPath(path);
        PrefabUtility.SaveAsPrefabAssetAndConnect(agent, path, InteractionMode.UserAction);
       
        DestroyImmediate(agent);

    }

    
    void CreateController()
    {
        
        controller = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath("Assets/Resources/Animation/" + animControllerName + ".controller");

        controller.AddParameter("Speed", UnityEngine.AnimatorControllerParameterType.Float);
        controller.AddParameter("Attack", UnityEngine.AnimatorControllerParameterType.Trigger);
       
    }
    
    void AddState(UnityEditor.Animations.AnimatorController controller)
    {
        rootStateMachine = controller.layers[0].stateMachine;

        var state = rootStateMachine.AddState(clip.name);
        
        state.mirror = true;
        state.motion = clip;
    }
    void SetTransition()
    {
        var states = rootStateMachine.states;
        
        var transition = states[firstState].state.AddTransition(states[lastState].state);
        if (exitTime)
        {
            transition.hasExitTime = true;
        }
        else
        {
            transition.hasExitTime = false;
        }
        transition.exitTime = exitTimeValue;
        transition.duration = 0;
        if (playerMove&&hasCondition)
        {
            transition.AddCondition(UnityEditor.Animations.AnimatorConditionMode.Greater, 0.01f, "Speed");
        }
        else if(playerStopped&&hasCondition)
        {
            transition.AddCondition(UnityEditor.Animations.AnimatorConditionMode.Less, 0.01f, "Speed");
        }
        if (anyState)
        {
            var copy = rootStateMachine.AddAnyStateTransition(states[firstState].state);
            copy.AddCondition(UnityEditor.Animations.AnimatorConditionMode.If, 0.0f, "Attack");
            copy.hasExitTime = false;
            copy.duration = 0;
        }

        
    }

    private void makeAnimation()
    {
        clip = new AnimationClip();
        clip.frameRate = 60f;
        
        EditorCurveBinding spriteBinding = new EditorCurveBinding();
        spriteBinding.type = typeof(SpriteRenderer);
        spriteBinding.path = "";
        spriteBinding.propertyName = "m_Sprite";

        ObjectReferenceKeyframe[] spriteKeyframes = new ObjectReferenceKeyframe[_sprites.Length];
        for (int i = 0; i < _sprites.Length; i++)
        {
            spriteKeyframes[i] = new ObjectReferenceKeyframe();
            spriteKeyframes[i].time = (i * 30f)/600f;
            spriteKeyframes[i].value = _sprites[i];
        }
        Debug.Log(_sprites.Length);
        AnimationUtility.SetObjectReferenceCurve(clip, spriteBinding, spriteKeyframes);
        AssetDatabase.CreateAsset(clip, "Assets/Resources/Animation/" + animName + ".anim");
        AnimationClip ac = AssetDatabase.LoadAssetAtPath("Assets/Resources/Animation/" + animName + ".anim", typeof(AnimationClip)) as AnimationClip;
        AnimationClipSettings tsettings = AnimationUtility.GetAnimationClipSettings(ac);
        tsettings.loopTime = true;
        AnimationUtility.SetAnimationClipSettings(ac, tsettings); 
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    
    
}


