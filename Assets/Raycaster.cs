﻿using UnityEngine;
using static UnityEditor.IMGUI.Controls.PrimitiveBoundsHandle;

public class Raycaster : MonoBehaviour
{
    public Texture2D texture; // A texture to hold the rendered frame
    private Color32[] pixels; // Array to hold pixel colors

    private int screenWidth = 640;
    private int screenHeight = 480;
    private int mapWidth = 24;
    private int mapHeight = 24;

    private int[,] worldMap = new int[,]
    {
        {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
        {1,0,0,0,0,0,2,2,2,2,2,0,0,0,0,3,0,3,0,3,0,0,0,1},
        {1,0,0,0,0,0,2,0,0,0,2,0,0,0,0,0,0,0,0,0,0,0,0,1},
        {1,0,0,0,0,0,2,0,0,0,2,0,0,0,0,3,0,0,0,3,0,0,0,1},
        {1,0,0,0,0,0,2,0,0,0,2,0,0,0,0,0,0,0,0,0,0,0,0,1},
        {1,0,0,0,0,0,2,2,0,2,2,0,0,0,0,3,0,3,0,3,0,0,0,1},
        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
        {1,4,4,4,4,4,4,4,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
        {1,4,0,4,0,0,0,0,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
        {1,4,0,0,0,0,5,0,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
        {1,4,0,4,0,0,0,0,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
        {1,4,0,4,4,4,4,4,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
        {1,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
        {1,4,4,4,4,4,4,4,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
        {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1}
    };

    private double posX = 22;
    private double posY = 12;
    private double dirX = -1;
    private double dirY = 0;
    private double planeX = 0;
    private double planeY = 0.66;
    public Material material;
    private double time = 0;
    private double oldTime = 0;
    int side;
    private void Start()
    {
        pixels = new Color32[screenWidth * screenHeight];
        texture = new Texture2D(screenWidth, screenHeight);

        material = new Material(Shader.Find("Material")); 
        material.mainTexture = texture; // Assign our texture to the material's main texture

        // Create a quad that will act as our screen
        GameObject screenQuad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        screenQuad.transform.SetParent(transform); // Set the quad as a child of this GameObject
        screenQuad.transform.localPosition = new Vector3(0, 0, 1); // Move the quad slightly in front of the camera
        screenQuad.GetComponent<Renderer>().material = material; // Assign the material to the quad's renderer

        screen(screenWidth, screenHeight, 0, "Raycaster");
    }

    private void Update()
    {

        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.black; // You can set it to any color you want
        }
        for (int x = 0; x < screenWidth; x++)
        {
            // Calculate ray position and direction
            double cameraX = 2 * x / (double)screenWidth - 1;
            double rayDirX = dirX + planeX * cameraX;
            double rayDirY = dirY + planeY * cameraX;

            int mapX = (int)posX;
            int mapY = (int)posY;

            double sideDistX;
            double sideDistY;

            double deltaDistX = (rayDirX == 0) ? 1e30 : Mathf.Abs(1 / (float)rayDirX);
            double deltaDistY = (rayDirY == 0) ? 1e30 : Mathf.Abs(1 / (float)rayDirY);

            double perpWallDist;

            int stepX;
            int stepY;

            int hit = 0;
           

            if (rayDirX < 0)
            {
                stepX = -1;
                sideDistX = (posX - mapX) * deltaDistX;
            }
            else
            {
                stepX = 1;
                sideDistX = (mapX + 1.0 - posX) * deltaDistX;
            }
            if (rayDirY < 0)
            {
                stepY = -1;
                sideDistY = (posY - mapY) * deltaDistY;
            }
            else
            {
                stepY = 1;
                sideDistY = (mapY + 1.0 - posY) * deltaDistY;
            }

            while (hit == 0)
            {
                if (sideDistX < sideDistY)
                {
                    sideDistX += deltaDistX;
                    mapX += stepX;
                    side = 0;
                }
                else
                {
                    sideDistY += deltaDistY;
                    mapY += stepY;
                    side = 1;
                }

                if (worldMap[mapX, mapY] > 0)
                    hit = 1;
            }

            if (side == 0)
                perpWallDist = (sideDistX - deltaDistX);
            else
                perpWallDist = (sideDistY - deltaDistY);

            int lineHeight = (int)(screenHeight / perpWallDist);

            int drawStart = -lineHeight / 2 + screenHeight / 2;
            if (drawStart < 0) drawStart = 0;
            int drawEnd = lineHeight / 2 + screenHeight / 2;
            if (drawEnd >= screenHeight) drawEnd = screenHeight - 1;

            Color color;
            switch (worldMap[mapX, mapY])
            {
                case 1: color = Color.red; break;
                case 2: color = Color.green; break;
                case 3: color = Color.blue; break;
                case 4: color = Color.white; break;
                default: color = Color.yellow; break;
            }

            if (side == 1) color /= 2;

            for (int y = drawStart; y < drawEnd; y++)
            {
                pixels[y * screenWidth + x] = color;
            }
        }

        texture.SetPixels32(pixels);
        texture.Apply();

        oldTime = time;
        time = Time.time;
        double frameTime = (time - oldTime);
        Debug.Log("FPS: " + (1.0 / frameTime));

        // Input handling
        double moveSpeed = frameTime * 5.0;
        double rotSpeed = frameTime * 3.0;

        if (Input.GetKey(KeyCode.W))
        {
            if (worldMap[(int)(posX + dirX * moveSpeed), (int)posY] == 0)
                posX += dirX * moveSpeed;
            if (worldMap[(int)posX, (int)(posY + dirY * moveSpeed)] == 0)
                posY += dirY * moveSpeed;
        }
        if (Input.GetKey(KeyCode.S))
        {
            if (worldMap[(int)(posX - dirX * moveSpeed), (int)posY] == 0)
                posX -= dirX * moveSpeed;
            if (worldMap[(int)posX, (int)(posY - dirY * moveSpeed)] == 0)
                posY -= dirY * moveSpeed;
        }
        if (Input.GetKey(KeyCode.D))
        {
            double oldDirX = dirX;
            dirX = dirX * Mathf.Cos((float)-rotSpeed) - dirY * Mathf.Sin((float)-rotSpeed);
            dirY = oldDirX * Mathf.Sin((float)-rotSpeed) + dirY * Mathf.Cos((float)-rotSpeed);

            double oldPlaneX = planeX;
            planeX = planeX * Mathf.Cos((float)-rotSpeed) - planeY * Mathf.Sin((float)-rotSpeed);
            planeY = oldPlaneX * Mathf.Sin((float)-rotSpeed) + planeY * Mathf.Cos((float)-rotSpeed);
        }
        if (Input.GetKey(KeyCode.A))
        {
            double oldDirX = dirX;
            dirX = dirX * Mathf.Cos((float)rotSpeed) - dirY * Mathf.Sin((float)rotSpeed);
            dirY = oldDirX * Mathf.Sin((float)rotSpeed) + dirY * Mathf.Cos((float)rotSpeed);

            double oldPlaneX = planeX;
            planeX = planeX * Mathf.Cos((float)rotSpeed) - planeY * Mathf.Sin((float)rotSpeed);
            planeY = oldPlaneX * Mathf.Sin((float)rotSpeed) + planeY * Mathf.Cos((float)rotSpeed);
        }

        material.mainTexture = texture;
    }

    private void screen(int width, int height, int depth, string title)
    {
        
    }
}
