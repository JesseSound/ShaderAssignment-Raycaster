using System;
using System.Collections.Generic;
using UnityEngine;
//using static UnityEditor.IMGUI.Controls.PrimitiveBoundsHandle;

public class Raycaster : MonoBehaviour
{
    public Texture2D texture; // A texture to hold the rendered frame
    private Color32[] pixels; // Array to hold pixel colors
    //define a screen width + height for no other reason than to do so. I GUESS we could define it based on the quad, but i digress
    private int screenWidth = 1280 ;
    private int screenHeight = 720 ;
    int texHeight = 64;
    int texWidth = 64;

    private int[,] worldMap =
 {
    {4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4},
    {4, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 4},
    {4, 0, 1, 0, 1, 0, 0, 0, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 4},
    {4, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 4},
    {4, 0, 0, 0, 0, 1, 4, 4, 4, 4, 4, 0, 0, 0, 0, 4, 0, 4, 0, 4, 0, 0, 0, 4},
    {4, 0, 1, 1, 1, 0, 4, 0, 0, 0, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 4},
    {4, 0, 0, 1, 0, 0, 4, 0, 0, 0, 4, 0, 0, 0, 0, 4, 0, 0, 0, 4, 0, 0, 0, 4},
    {4, 0, 0, 1, 0, 0, 4, 0, 0, 0, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 4},
    {4, 0, 0, 1, 0, 0, 4, 4, 0, 4, 4, 0, 0, 0, 0, 4, 0, 4, 0, 4, 0, 0, 0, 4},
    {4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 4},
    {4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 4},
    {4, 0, 0, 0, 0, 0, 0, 0, 0, 4, 4, 4, 4, 4, 4, 4, 4, 0, 1, 1, 0, 0, 0, 4},
    {4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 4},
    {4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 4},
    {4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 4},
    {4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 4},
    {4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 1, 1, 1, 0, 1, 1, 0, 0, 0, 4},
    {4, 4, 4, 4, 4, 4, 4, 4, 4, 0, 0, 1, 0, 0, 1, 1, 1, 0, 1, 1, 0, 0, 0, 4},
    {4, 4, 0, 4, 0, 0, 0, 0, 4, 0, 0, 1, 0, 0, 1, 1, 1, 0, 1, 1, 0, 0, 0, 4},
    {4, 4, 0, 0, 0, 0, 5, 0, 4, 0, 0, 1, 1, 1, 1, 0, 1, 1, 0, 1, 0, 0, 0, 4},
    {4, 4, 0, 4, 0, 0, 0, 0, 4, 0, 0, 1, 1, 0, 1, 0, 0, 1, 1, 1, 1, 0, 0, 4},
    {4, 4, 0, 4, 4, 4, 4, 4, 4, 0, 0, 1, 1, 1, 1, 1, 1, 0, 1, 1, 1, 0, 0, 4},
    {4, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 1, 1, 0, 0, 0, 0, 0, 0, 4},
    {4, 4, 4, 4, 4, 4, 4, 4, 4, 0, 0, 0, 1, 1, 0, 0, 0, 1, 0, 1, 0, 0, 0, 4},
    {4, 4, 4, 4, 4, 4, 4, 4, 4, 0, 0, 0, 1, 1, 0, 1, 1, 1, 1, 0, 0, 0, 0, 4},
    {4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4},
};


    private double posX = 22;
    private double posY = 12;
    private double dirX = -1;
    private double dirY = 0;
    private double planeX = 0;
    private double planeY = 0.66;
    public Material material;
    public Texture2D[] pngTexture;
    Color32[] _png;
    /*outdated logic
    private double time = 0;
     private double oldTime = 0;
    */
    int side;
    private uint[,] buffer; //maybe not needed?

    private List<Color32[]> textures = new List<Color32[]>();

    private void Start()
    {
        // Check if the texture is assigned
        if (pngTexture != null)
        {
            for (int pngCount = 0; pngCount < 7; pngCount++)
            {
                // Get the original PNG texture
                Texture2D originalTexture = pngTexture[pngCount];

                // Resize the texture
                Texture2D resizedTexture = ResizeTexture(originalTexture, texWidth, texHeight);

                // Convert the resized texture to a Color32 array
                Color32[] colors = resizedTexture.GetPixels32();

                // Add the Color32 array to the list
                textures.Add(colors);
            }
        }
        else
        {
            Debug.LogError("No PNG textures assigned!");
        }
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
                    textures[i][texWidth * y + x] = textures[i][texWidth * y + x];


                    /*out dated logic but worth keeping for study
                     * int xorcolor = (x * 256 / texWidth) ^ (y * 256 / texHeight);
                    //int xcolor = x * 256 / texWidth;
                    int ycolor = y * 256 / texHeight;
                    int xycolor = y * 128 / texHeight + x * 128 / texWidth;
                    switch (i)
                    {
                        case 0: textures[i][texWidth * y + x] = textures[i][texWidth * y + x];  break; 
                        case 1: textures[i][texWidth * y + x] = new Color32((byte)xycolor, (byte)xycolor, (byte)xycolor, 255); break; // Greyscale
                        case 2: textures[i][texWidth * y + x] = new Color32(255, (byte)xycolor, (byte)xycolor, 255); break; // Yellow gradient
                        case 3: textures[i][texWidth * y + x] = new Color32((byte)xorcolor, (byte)xorcolor, (byte)xorcolor, 255); break; // XOR greyscale
                        case 4: textures[i][texWidth * y + x] = new Color32(0, (byte)xorcolor, 0, 255); break; // XOR green
                        case 5: textures[i][texWidth * y + x] = new Color32(((x / 8) % 2 != 0 ^ (y / 8) % 2 != 0) ? (byte)192 : (byte)255, (byte)192, (byte)192, 255); break; // Red bricks
                        case 6: textures[i][texWidth * y + x] = new Color32(255, (byte)ycolor, 0, 255); break; // Red gradient
                        case 7: textures[i][texWidth * y + x] = new Color32(128, 128, 128, 255); break; // Grey
                    
                    }
                    */
                }
            }


        }

    }
    //Logic to resize images so I don't need a website to do it
    private Texture2D ResizeTexture(Texture2D originalTexture, int newWidth, int newHeight)
    {
        Texture2D resizedTexture = new Texture2D(newWidth, newHeight, TextureFormat.RGBA32, false);

        Color[] pixels = new Color[newWidth * newHeight];
        for (int y = 0; y < newHeight; y++)
        {
            for (int x = 0; x < newWidth; x++)
            {
                pixels[y * newWidth + x] = originalTexture.GetPixelBilinear((float)x / newWidth, (float)y / newHeight);
            }
        }

        resizedTexture.SetPixels(pixels);
        resizedTexture.Apply();

        return resizedTexture;
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

                if (mapX >= 0 && mapX < worldMap.GetLength(0) && mapY >= 0 && mapY < worldMap.GetLength(1))
                {
                    if (worldMap[mapX, mapY] > 0)
                        hit = 1;
                }
                else
                {
                    Debug.LogError("Index out of bounds: mapX=" + mapX + ", mapY=" + mapY);
                }

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

            int texNum = worldMap[mapX, mapY] - 1; // important!!! this will set the texture shown minus one, keep in mind when building map
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
                    //we're shifting each color component (r, g, b) to the right by 1 bit and then creating a new Color32 instance with the shifted values. This will effectively darken each color component by halving its value.
                    color = new Color32((byte)(color.r >> 1), (byte)(color.g >> 1), (byte)(color.b >> 1), color.a);

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
                Color32 floorColor = textures[3][texWidth * floorTexY + floorTexX];
                //floorColor = new Color32((byte)(floorColor.r * 0.5f), (byte)(floorColor.g * 0.5f), (byte)(floorColor.b * 0.5f), floorColor.a);

                // Ceiling 
                Color32 ceilingColor = textures[3][texWidth * floorTexY + floorTexX];

                // Set floor and ceiling colors
                pixels[y * screenWidth + x] = ceilingColor;
                pixels[(screenHeight - y - 1) * screenWidth + x] = floorColor;
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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Calculate the tile coordinates based on the direction you're facing
            int tileX = (int)(posX + dirX);
            int tileY = (int)(posY + dirY);

            // Check if the calculated tile coordinates are within the world map bounds
            if (tileX >= 0 && tileX < worldMap.GetLength(0) && tileY >= 0 && tileY < worldMap.GetLength(1))
            {
                if(worldMap[tileX, tileY] == 0)
                    worldMap[tileX, tileY] = 1;
                
            }
        }
        // Assign the texture to the material for display
        material.mainTexture = texture;

        if (Time.frameCount % 30 == 0) 
        {
            ApplyGameOfLifeRules();
        }
    }

    private void ApplyGameOfLifeRules()
    {
        int[,] newWorldMap = new int[worldMap.GetLength(0), worldMap.GetLength(1)];

        for (int x = 0; x < worldMap.GetLength(0); x++)
        {
            for (int y = 0; y < worldMap.GetLength(1); y++)
            {
                int aliveNeighbors = CountAliveNeighbors(x, y);

                if (worldMap[x, y] == 1 && (aliveNeighbors < 2 || aliveNeighbors > 3))
                    newWorldMap[x, y] = 0; // Cell dies
                else if (worldMap[x, y] == 0 && aliveNeighbors == 3)
                    newWorldMap[x, y] = 1; // Cell becomes alive
                else
                    newWorldMap[x, y] = worldMap[x, y]; // Cell remains the same
            }
        }

        worldMap = newWorldMap; // Update the world map
    }

    private int CountAliveNeighbors(int x, int y)
    {
        int count = 0;

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                int neighborX = x + i;
                int neighborY = y + j;

                if (neighborX >= 0 && neighborX < worldMap.GetLength(0) &&
                    neighborY >= 0 && neighborY < worldMap.GetLength(1) &&
                    !(i == 0 && j == 0))
                {
                    count += worldMap[neighborX, neighborY];
                }
            }
        }

        return count;
    }


}
