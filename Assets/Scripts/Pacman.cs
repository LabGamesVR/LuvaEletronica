using Assets.Scripts;
using System;
using System.IO.Ports;
using UnityEngine;

[RequireComponent(typeof(Movement))]
public class Pacman : MonoBehaviour
{
    public AnimatedSprite deathSequence;
    public SpriteRenderer spriteRenderer { get; private set; }
    public new Collider2D collider { get; private set; }
    public Movement movement { get; private set; }

    public Direcao direcaoAnterior;
    public int normalX;
    public int normalY;

    private GerenciadorMovimentos sensor;

    public Game game;
    private void Start()
    {
        print("START PACMAN");
        direcaoAnterior = Direcao.estavel;
        game = new Game();
        int playerPrefX = PlayerPrefs.GetInt("x");
        int playerPrefY = PlayerPrefs.GetInt("y");
        
        if (playerPrefX > 0 && playerPrefY > 0)
        {
            normalX = playerPrefX;
            normalY = playerPrefY;
        }
        else
        {
            normalX = 310;
            normalY = 340;
        }

        // serialPort = new SerialPort(namePort, baudRate);
        // serialPort.Open();
        // serialPort.ReadTimeout = 50;
    }

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        collider = GetComponent<Collider2D>();
        movement = GetComponent<Movement>();
        sensor = FindObjectOfType<GerenciadorMovimentos>();
    }

    // public int GetInputArduino()
    // {
    //     int entrada = 5;
    //     if (serialPort.IsOpen)
    //     {
    //         try
    //         {
    //             string data = serialPort.ReadLine();
    //             string[] tokens = data.Split(',');
    //             int x = Convert.ToInt32(tokens[0]);
    //             int y = Convert.ToInt32(tokens[1]);
                
    //             entrada = ObterDirecao(x , y);
    //         }
    //         catch (Exception ex)
    //         {
    //             Debug.Log("error" + ex.Message);
    //         }
    //     }

    //     return entrada;
    // }

    //private int ObterDirecao(int x, int y)
    private int ObterDirecao()
    {
        switch (sensor.ObterDirecao())
        {
            case Movimento2Eixos.Direcao.top:
                return (int)Direcao.cima;
            case Movimento2Eixos.Direcao.bot:
                return (int)Direcao.baixo;
            case Movimento2Eixos.Direcao.left:
                return (int)Direcao.esquerda;
            case Movimento2Eixos.Direcao.right:
                return (int)Direcao.direita;
            default:
                break;
        }
        if (Input.GetKey("up"))
            return (int)Direcao.cima;
        if (Input.GetKey("down"))
            return (int)Direcao.baixo;
        if (Input.GetKey("left"))
            return (int)Direcao.esquerda;
        if (Input.GetKey("right"))
            return (int)Direcao.direita;
        return (int)Direcao.estavel;    
    }

    private int SetarERetornarDirecao(Direcao direcao, Direcao direcaoOposta)
    {
        if (direcaoAnterior == direcaoOposta)
        {
            direcaoAnterior = Direcao.estavel;
            return (int)Direcao.estavel;
        }

        if (direcaoAnterior != direcao)
        {
            AdicionarQuantidadesDirecoesNoGame(direcao);
        }

        direcaoAnterior = direcao;

        return (int)direcao;
    }

    private void AdicionarQuantidadesDirecoesNoGame(Direcao direcao)
    {
        switch (direcao)
        {
            case Direcao.baixo:
                Debug.Log("Direcao.baixo");
                game.QtdVezesAbaixo += 1;
                break;
            case Direcao.cima:
                Debug.Log("Direcao.cima");
                game.QtdVezesAcima += 1;
                break;
            case Direcao.direita:
                Debug.Log("Direcao.direita");
                game.QtdVezesDireita += 1;
                break;
            case Direcao.esquerda:
                Debug.Log("Direcao.esquerda");
                game.QtdVezesEsquerda += 1;
                break;
            default:
                break;
        }
    }

    private void Update()
    {
        int entrada = ObterDirecao();


        switch (entrada)
        {
            case 0:
                //baixo
                movement.SetDirection(Vector2.down);
                break;
            case 1:
                //cima
                movement.SetDirection(Vector2.up);
                break;
            case 2:
                //direita
                movement.SetDirection(Vector2.right);
                break;
            case 3:
                //esquerda
                movement.SetDirection(Vector2.left);
                break;
        }

        //Set the new direction based on the current input
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            movement.SetDirection(Vector2.up);
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            movement.SetDirection(Vector2.down);
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            movement.SetDirection(Vector2.left);
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            movement.SetDirection(Vector2.right);
        }

        float angle = Mathf.Atan2(movement.direction.y, movement.direction.x);
        transform.rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, Vector3.forward);
    }

    public void ResetState()
    {
        enabled = true;
        spriteRenderer.enabled = true;
        collider.enabled = true;
        deathSequence.enabled = false;
        deathSequence.spriteRenderer.enabled = false;
        movement.ResetState();
        gameObject.SetActive(true);
    }

    public void DeathSequence()
    {
        enabled = false;
        spriteRenderer.enabled = false;
        collider.enabled = false;
        movement.enabled = false;
        deathSequence.enabled = true;
        deathSequence.spriteRenderer.enabled = true;
        deathSequence.Restart();
    }

}

public enum Direcao
{
    baixo = 0,
    cima = 1,
    direita = 2,
    esquerda = 3,
    estavel = 5
}
