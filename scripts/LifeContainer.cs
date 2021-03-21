using Godot;
using System;

public class LifeContainer : HBoxContainer
{
    public override void _Ready()
    {
        
    }

    public int GetLife()
    {
        int life = 0;
        var children = GetChildren();
        foreach(TextureRect child in children)
        {
            if(child.Visible)
                life++;
        }

        return life;
    }

    public void AddLife()
    {
        int index = GetLife();
        TextureRect textureLife = (TextureRect) GetChild(index);
        textureLife.Visible = true;
    }

    public void DeductLife()
    {
        int index = GetLife() - 1;
        TextureRect textureLife = (TextureRect) GetChild(index);
        textureLife.Visible = false;
    }
}
