 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameControllerScript : MonoBehaviour
{
    public static GameControllerScript instantiate;

    private int scoreCount = 0;
    private int counColor = 0;

    private bool targetOK = false;
    private bool gameOver = false;
    public bool firstClickDown = false;

    private float i = 0;
    private float comboCount;
    private float lastPosX;
    private float lastPosZ;
    private float lastScaleX;
    private float lastScaleZ;
    private float targetPoint;
    private float speed = 1.5f;


    [Header("GameObject")]
    public GameObject firstGameObject;
    public GameObject cameraObject;
    public GameObject tileObject;
    public GameObject pieceCube;

    private GameObject pieceObject;
    private GameObject hillTileObject;
    private GameObject lastHillTileObject;

    [Header("GUI GameObject")]
    public GameObject restartText;
    public GameObject textDefault;
    public GameObject textDefault2;
    public GameObject textBestScore;
    //public GameObject panelButton;
    //public GameObject panelStackButton;
    //public GameObject soundButton1;
    //public GameObject soundButton2;

    private Vector3 createVector;
    private Vector3 stackSize;


    [Header("Color")]
    private int colorR ;
    private int colorG ;
    private int colorB ;

    [Header("Audio")]
    private AudioSource musicSource;
    private AudioClip musicClip;

    [Header("Text")]
    public Text scoreText;
    public Text bestScoreText;

    private void Awake()
    {
        instantiate = this;
    }

    private void Start()
    {
        firstClickDown = false;
        gameOver = false;

        restartText.SetActive(false);
        textBestScore.SetActive(false);

        musicSource = GetComponent<AudioSource>();
        musicClip = musicSource.clip;

        StartColorChange();

        createVector = new Vector3(0, 5f + 0.3f, 0);
        lastHillTileObject = firstGameObject;
    }

    private void Update()
    { 
        if (gameOver)
        {
            GameOverFunctions();
        }
        else if (firstClickDown && firstGameObject.transform.position.y == 5f)
        {
            textDefault.SetActive(false);
            textDefault2.SetActive(false);

            if (hillTileObject != null)
            {
                MoveTile();
                if (Mathf.Abs(hillTileObject.transform.position.y - cameraObject.transform.position.y) < 9)
                {
                    Vector3 _position = new Vector3(cameraObject.transform.position.x,
                        cameraObject.transform.position.y + hillTileObject.transform.localScale.y, cameraObject.transform.position.z);

                    cameraObject.transform.position = Vector3.Lerp(cameraObject.transform.position, _position, 0.1f);
                }

                if (gameOver == false)
                {
                    if (Input.GetMouseButtonDown(0) && !gameOver)
                        UpdateOtherOperation();
                }
            }
            else
                CreateNewObject();
        }
    }

    /// The following method is game over worked.
    private void GameOverFunctions()
    {
        if (scoreCount >= 10 && scoreCount < 15)
        {
            if (cameraObject.GetComponent<Camera>().orthographicSize < 6)
                cameraObject.GetComponent<Camera>().orthographicSize += 0.5f;
        }
        else if (scoreCount >= 15 && scoreCount < 25)
        {
            if (cameraObject.GetComponent<Camera>().orthographicSize < 8.5f)
                cameraObject.GetComponent<Camera>().orthographicSize += 0.5f;
        }
        else if (scoreCount >= 25 && scoreCount < 50)
        {
            if (cameraObject.GetComponent<Camera>().orthographicSize < 10)
                cameraObject.GetComponent<Camera>().orthographicSize += 0.5f;
        }
        else if (scoreCount >= 50)
        {
            if (cameraObject.GetComponent<Camera>().orthographicSize < (scoreCount / 5))
                cameraObject.GetComponent<Camera>().orthographicSize += 0.5f;
        }

        if (scoreCount > PlayerPrefs.GetInt("BestScore"))
            PlayerPrefs.SetInt("BestScore", scoreCount);

        textBestScore.SetActive(true);
        bestScoreText.text = "  " + PlayerPrefs.GetInt("BestScore").ToString();
        if (Input.GetMouseButtonDown(0))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }

    /// The following method is go cut AND new method and new create object , gravity true 
    private void UpdateOtherOperation()
    {
        stackSize = hillTileObject.transform.localScale;
        if (gameOver == false)
            CutAndNew(hillTileObject);           
     
        lastPosX = hillTileObject.transform.position.x;
        lastPosZ = hillTileObject.transform.position.z;
        scoreCount++;

        scoreText.text = scoreCount.ToString();
        lastHillTileObject = hillTileObject;
        hillTileObject = null;
    }

    /// The following method is used to create new tile   
    private void CreateNewObject()
    {
        if(hillTileObject == null)
        {
           if(scoreCount % 2 == 0)
           {
                createVector.x = lastPosX + 4f;
                createVector.z = lastPosZ;
                hillTileObject = Instantiate(tileObject, createVector, Quaternion.identity, transform);
                hillTileObject.transform.localScale = lastHillTileObject.transform.localScale;
                hillTileObject.name = "Cube" + i;
                i++;
                createVector.y += hillTileObject.transform.localScale.y;
                
           }
           else
           {
                createVector.z = lastPosZ + 4f ;
                createVector.x = lastPosX;
                hillTileObject = Instantiate(tileObject, createVector, Quaternion.identity, transform);
                hillTileObject.transform.localScale = lastHillTileObject.transform.localScale;
                hillTileObject.name = "Cube" + i;
                i++;
                createVector.y += hillTileObject.transform.localScale.y;
           }
           
            hillTileObject.GetComponent<Renderer>().material.SetColor("_Color",RainbowColor());
        }
    }

    /// The following method is color rainbow 
    private Color RainbowColor()
    {
        Color newColor;
        if(colorR == 255 && colorG >= 0  && colorB == 0)
        {
            colorG += 17;
            newColor = new Color32((byte)colorR, (byte)colorG, (byte)colorB, 255);
            if(colorG == 255)
            {
                colorR -= 17;
            }
        }
        else if (colorR == 0 && colorG >= 0 && colorB == 255)
        {
            colorG -= 17;
            newColor = new Color32((byte)colorR, (byte)colorG, (byte)colorB, 255);
            if (colorG == 0)
            {
                colorR += 17;
            }
        }
        else if (colorR == 255 && colorG == 0 && colorB >= 0)
        {
            colorB -= 17;
            newColor = new Color32((byte)colorR, (byte)colorG, (byte)colorB, 255);
            if (colorB == 0)
            {
                colorG += 17;
            }
        }
        else if (colorR == 0 && colorG == 255 && colorB >= 0)
        {
            colorB += 17;
            newColor = new Color32((byte)colorR, (byte)colorG, (byte)colorB, 255);
            if (colorB == 255)
            {
                colorG -= 17;
            }
        }
        else if(colorR >= 0 && colorG == 255 && colorB == 0)
        {
            colorR -= 17;
            newColor = new Color32((byte)colorR, (byte)colorG, (byte)colorB, 255);
            if (colorR == 0)
            {
                colorB += 17;
            }
        }
        else if(colorR >= 0 && colorG == 0 && colorB == 255)
        {
            colorR += 17;
            newColor = new Color32((byte)colorR, (byte)colorG, (byte)colorB, 255);
            if(colorR == 255)
            {
                colorB -= 17;
            }
        }
        

        newColor = new Color32((byte)colorR, (byte)colorG, (byte)colorB, 255);
        return newColor;
    }

    /// The following method is used to move the tile position
    private void MoveTile()
    {
        if (targetOK)
            targetPoint = 3f;
        else
            targetPoint = -3f;

        if (scoreCount % 2 == 0)
        {
            Vector3 _vector = hillTileObject.transform.position;

            _vector.x = Mathf.MoveTowards(_vector.x, targetPoint, 0.05f * speed);
            if (Mathf.Abs(_vector.x - targetPoint) < 0.01f)
            {
                _vector.x = targetPoint;
                if (_vector.x == -3f)
                    targetOK = true;
                else if (_vector.x == 3f)
                    targetOK = false;
            }
            hillTileObject.transform.position = _vector;
        }
        else
        {
            Vector3 _vector = hillTileObject.transform.position;

            _vector.z = Mathf.MoveTowards(_vector.z, targetPoint, 0.05f * speed);
            if (Mathf.Abs(_vector.z - targetPoint) < 0.01f)
            {
                 _vector.z = targetPoint;
                 if (_vector.z == -3f)
                    targetOK = true;
                 else if (_vector.z == 3f)
                    targetOK = false;
            }
            hillTileObject.transform.position = _vector;
        }
    }

    ///The following method is cute a gameobject and create piece
    private void CutAndNew(GameObject currentCutObject)
    {
        if (scoreCount % 2 == 0)
        {
            if (Mathf.Abs(lastHillTileObject.transform.localPosition.x - currentCutObject.transform.localPosition.x) < 0.09f)
            {
                comboCount++;
                currentCutObject.transform.position = new Vector3(lastHillTileObject.transform.localPosition.x, currentCutObject.transform.localPosition.y,
                    lastHillTileObject.transform.localPosition.z);
            }
            else
            {
                float posX = lastHillTileObject.transform.localPosition.x - currentCutObject.transform.localPosition.x;
                musicSource.Play();
                stackSize.x -= Mathf.Abs(posX);
                if (stackSize.x < 0.01f)
                {
                    gameOver = true;
                    firstClickDown = false;
                    restartText.SetActive(true);
                    PieceObject(currentCutObject.transform.localPosition.x,lastHillTileObject.transform.localScale.x,
                        pieceCube, currentCutObject.GetComponent<Renderer>().material.color);
                    Destroy(currentCutObject);
                    scoreCount--;
                    return;
                }
                comboCount = 0;

                currentCutObject.transform.localScale = new Vector3(stackSize.x, stackSize.y, stackSize.z);
                float midPosX = (currentCutObject.transform.localPosition.x + lastHillTileObject.transform.localPosition.x) / 2;

                currentCutObject.transform.localPosition = new Vector3(midPosX, currentCutObject.transform.position.y,
                    currentCutObject.transform.localPosition.z);

                float _value;
                if (midPosX > lastHillTileObject.transform.localPosition.x)
                     _value = midPosX + (2 * (stackSize.x / 3));
                else
                     _value = midPosX - (2 * (stackSize.x / 3));

                PieceObject(_value, lastHillTileObject.transform.localScale.x - stackSize.x,
                       pieceCube, currentCutObject.GetComponent<Renderer>().material.color);
            }
        }
        else
        {
            if (Mathf.Abs(lastHillTileObject.transform.localPosition.z - currentCutObject.transform.localPosition.z) < 0.09f)
            {
                comboCount++;
                currentCutObject.transform.position = new Vector3(lastHillTileObject.transform.localPosition.x, currentCutObject.transform.localPosition.y,
                        lastHillTileObject.transform.localPosition.z);
            }
            else
            {
                float posZ = lastHillTileObject.transform.localPosition.z - currentCutObject.transform.localPosition.z;
                musicSource.Play();
                stackSize.z -= Mathf.Abs(posZ);
                if (stackSize.z < 0.01f)
                {
                    gameOver = true;
                    firstClickDown = false;
                    restartText.SetActive(true);
                    PieceObject(currentCutObject.transform.localPosition.z, lastHillTileObject.transform.localScale.z,
                        pieceCube, currentCutObject.GetComponent<Renderer>().material.color);
                    Destroy(currentCutObject);
                    scoreCount--;
                    return;
                }
                comboCount = 0;

                currentCutObject.transform.localScale = new Vector3(stackSize.x, stackSize.y, stackSize.z);
                float midPosZ = (currentCutObject.transform.localPosition.z + lastHillTileObject.transform.localPosition.z) / 2;

                currentCutObject.transform.localPosition = new Vector3(currentCutObject.transform.localPosition.x,
                    currentCutObject.transform.position.y, midPosZ);

                float _value;
                if (midPosZ > lastHillTileObject.transform.localPosition.z)
                    _value = midPosZ + (2 * (stackSize.z / 3));
                else
                    _value = midPosZ - (2 * (stackSize.z / 3));

                PieceObject(_value, lastHillTileObject.transform.localScale.z - stackSize.z,
                        pieceCube, currentCutObject.GetComponent<Renderer>().material.color);
            }
        }
    }
    
    ///Create piece method
    private void PieceObject(float piecePos,float pieceScale,GameObject piece,Color _color)
    {

        if(scoreCount % 2 == 0)
        {
            Vector3 _vectorPos = new Vector3(piecePos, hillTileObject.transform.position.y, hillTileObject.transform.position.z);
            pieceObject = Instantiate(piece, _vectorPos, Quaternion.identity);
            pieceObject.transform.localScale = new Vector3(pieceScale, piece.transform.localScale.y, hillTileObject.transform.localScale.z);

        }
        else
        {
            Vector3 _vectorPos = new Vector3(hillTileObject.transform.position.x, hillTileObject.transform.position.y, piecePos);
            pieceObject = Instantiate(piece, _vectorPos, Quaternion.identity);
            pieceObject.transform.localScale = new Vector3(hillTileObject.transform.localScale.x, piece.transform.localScale.y, pieceScale);
        }

        pieceObject.GetComponent<Renderer>().material.SetColor("_Color", _color);
        pieceObject.GetComponent<Rigidbody>().useGravity = true;
    }

    //Method is Canvas Play Button 
    public void FirstClickPlay()
    {
        if(firstGameObject.transform.position.y >= 5f)
            firstClickDown = true;
    }

    ///Start Color Background
    private void StartColorChange()
    {
        colorR = Random.Range(0, 255);
        colorG = Random.Range(0, 255);
        colorB = Random.Range(0, 255);

        if (counColor == 0)
        {
            if (colorR > colorB && colorR > colorG)
            {
                colorR = 255;
                colorB = 238;
                colorG = 0;
            }
            else if (colorG > colorB && colorG > colorR)
            {
                colorG = 255;
                colorR = 0;
                colorB = 238;
            }
            else if (colorB > colorR && colorB > colorG)
            {
                colorB = 255;
                colorG = 238;
                colorR = 0;
            }
            counColor = 1;
        }

        cameraObject.GetComponent<Camera>().backgroundColor = new Color32((byte)colorR, (byte)colorG, (byte)colorB, (byte)Random.Range(50, 150));
        firstGameObject.GetComponent<Renderer>().material.color = new Color32((byte)colorR, (byte)colorG, (byte)colorB, 255);
    }

  /*
  This project was started on 11.08.2019 and completed on 15.08.2019.
  It only produces the wrong object as a result of an error in the function being cut.
  When I look at the project again, I will try to solve that error.
  
				-Developer by İbrahim Atmaca
  */

}