﻿using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.IMGUI.Controls.PrimitiveBoundsHandle;

public class Raycaster : MonoBehaviour
{
    public Texture2D texture; // A texture to hold the rendered frame
    private Color32[] pixels; // Array to hold pixel colors
    //define a screen width + height for no other reason than to do so. I GUESS we could define it based on the quad, but i digress
    private int screenWidth = 640;
    private int screenHeight = 480;
    int texHeight = 64;
    int texWidth = 64;
    
    private int[,] worldMap =
    {
        {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
        {1,0,0,0,0,0,0,0,4,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
        {1,0,0,0,0,0,2,2,2,2,2,0,0,0,0,3,0,3,0,3,0,0,0,1},
        {1,0,0,0,0,0,2,0,0,0,2,0,0,0,0,0,0,0,0,0,0,0,0,1},
        {1,0,0,0,0,0,2,0,0,0,2,0,0,0,0,3,0,0,0,3,0,0,0,1},
        {1,0,0,0,0,0,2,0,0,0,2,0,0,0,0,0,0,0,0,0,0,0,0,1},
        {1,0,0,0,0,0,2,2,0,2,2,0,0,0,0,3,0,3,0,3,0,0,0,1},
        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
        {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
        {1,0,0,0,0,0,0,0,0,1,1,1,1,1,1,1,1,0,0,0,0,0,0,1},
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
    private uint[,] buffer;

    private List<Color32[]> textures = new List<Color32[]>();

    private void Start()
    {
        pixels = new Color32[screenWidth * screenHeight];
        texture = new Texture2D(screenWidth, screenHeight);
        material.mainTexture = texture; // Assign our texture to the material's main texture
          buffer = new uint[screenHeight, screenWidth];
        for (int i = 0; i < 8; i++)
        {
            textures.Add(new Color32[texWidth * texHeight]);
            for (int x = 0; x < texWidth; x++)
            {
                for (int y = 0; y < texHeight; y++)
                {
                    int xorcolor = (x * 256 / texWidth) ^ (y * 256 / texHeight);
                    //int xcolor = x * 256 / texWidth;
                    int ycolor = y * 256 / texHeight;
                    int xycolor = y * 128 / texHeight + x * 128 / texWidth;
                    switch (i)
                    {
                        case 0: textures[i][texWidth * y + x] = new Color32(255, 0, 0, 255); break; // Red
                        case 1: textures[i][texWidth * y + x] = new Color32((byte)xycolor, (byte)xycolor, (byte)xycolor, 255); break; // Greyscale
                        case 2: textures[i][texWidth * y + x] = new Color32(255, (byte)xycolor, (byte)xycolor, 255); break; // Yellow gradient
                        case 3: textures[i][texWidth * y + x] = new Color32((byte)xorcolor, (byte)xorcolor, (byte)xorcolor, 255); break; // XOR greyscale
                        case 4: textures[i][texWidth * y + x] = new Color32(0, (byte)xorcolor, 0, 255); break; // XOR green
                        case 5: textures[i][texWidth * y + x] = new Color32(255, 0, 0, 255); break; // Red bricks
                        case 6: textures[i][texWidth * y + x] = new Color32(255, (byte)ycolor, 0, 255); break; // Red gradient
                        case 7: textures[i][texWidth * y + x] = new Color32(128, 128, 128, 255); break; // Grey
                    }
                }
            }


        }

    }

    private void Update()
    {
        // Clear the screen to black, but don't need this currently
        //for (int i = 0; i < pixels.Length; i++)
        //{
        //    pixels[i] = Color.black;
        //}

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

            int texNum = worldMap[mapX, mapY] - 1;
            double wallX;
            if (side == 0)
            {
                wallX = posY + perpWallDist * rayDirY;
            }
            else
            {
                wallX = posX + perpWallDist * rayDirX;
            }

            wallX -= Math.Floor(wallX);
            int texX = (int)(wallX * texWidth);
            if (side == 0 && rayDirX > 0) texX = texWidth - texX - 1;
            if (side == 1 && rayDirY < 0) texX = texWidth - texX - 1;

            double step = 1.0 * texHeight / lineHeight;
            double texPos = (drawStart - screenHeight / 2 + lineHeight / 2) * step;

            for (int y = drawStart; y < drawEnd; y++)
            {
                // Cast the texture coordinate to integer, and mask with (texHeight - 1) in case of overflow
                int texY = (int)texPos & (texHeight - 1);
                texPos += step;

                // Get the color from the texture
                Color32 color = textures[texNum][texHeight * texY + texX];

                // Apply shading for side walls
                if (side == 1)
                {
                    color = new Color32((byte)(color.r * 0.5f), (byte)(color.g * 0.5f), (byte)(color.b * 0.5f), color.a);
                }

                // Set the pixel color for the entire column
                pixels[y * screenWidth + x] = color;
            }

            // Floor casting
            double floorXWall, floorYWall;

            if (side == 0 && rayDirX > 0)
            {
                floorXWall = mapX;
                floorYWall = mapY + wallX;
            }
            else if (side == 0 && rayDirX < 0)
            {
                floorXWall = mapX + 1.0;
                floorYWall = mapY + wallX;
            }
            else if (side == 1 && rayDirY > 0)
            {
                floorXWall = mapX + wallX;
                floorYWall = mapY;
            }
            else
            {
                floorXWall = mapX + wallX;
                floorYWall = mapY + 1.0;
            }

            double distWall = perpWallDist;
            double distPlayer = 0.0;

            if (drawEnd < 0)
            {
                drawEnd = screenHeight;
            }

            for (int y = drawEnd + 1; y < screenHeight; y++)
            {
                double currentDist = screenHeight / (2.0 * y - screenHeight);

                double weight = (currentDist - distPlayer) / (distWall - distPlayer);

                double currentFloorX = weight * floorXWall + (1.0 - weight) * posX;
                double currentFloorY = weight * floorYWall + (1.0 - weight) * posY;

                int floorTexX, floorTexY;
                floorTexX = (int)(currentFloorX * texWidth) % texWidth;
                floorTexY = (int)(currentFloorY * texHeight) % texHeight;

                // Floor texture
                Color32 floorColor = textures[4][texWidth * floorTexY + floorTexX];
                floorColor = new Color32((byte)(floorColor.r * 0.5f), (byte)(floorColor.g * 0.5f), (byte)(floorColor.b * 0.5f), floorColor.a);

                // Ceiling (just a solid color for now)
                Color32 ceilingColor = new Color32(128, 128, 128, 255);

                // Set floor and ceiling colors
                pixels[y * screenWidth + x] = floorColor;
                pixels[(screenHeight - y - 1) * screenWidth + x] = ceilingColor;
            }
        }

        // Apply the pixels to the texture
        texture.SetPixels32(pixels);
        texture.Apply();

        // Handle player movement
        double moveSpeed = Time.deltaTime * 5.0;
        double rotSpeed = Time.deltaTime * 3.0;

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

        // Assign the texture to the material for display
        material.mainTexture = texture;
    }



}
