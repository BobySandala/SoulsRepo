using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStatsClass
{
    private string Name;

    private int HP;
    private int MaxHP;

    private int DMG;

    private int SP;
    private int MaxSP;

    public int getHP() { return HP; }
    public int getMaxHP() { return MaxHP; }
    public int getDMG() { return DMG; }
    public int getSP() { return SP; }
    public int getMaxSP() { return MaxSP; }
    public string getName() { return Name; }

    public void setHP(int HP_) { HP = HP_; }
    public void setMaxHP(int MaxHP_) { MaxHP = MaxHP_; }
    public void setDMG(int DMG_) { DMG = DMG_; }
    public void setSP(int SP_) { SP = SP_; }
    public void setMaxSP(int MaxSP_) { MaxSP = MaxSP_; }
    public void setName(string Name_) { Name = Name_; }

    public CharacterStatsClass(string name_, int MaxHP_, int DMG_,int MaxSP_) 
    {
        this.Name = name_;

        this.MaxHP = MaxHP_ ;
        this.HP = MaxHP_;

        this.DMG = DMG_;

        this.MaxSP = MaxSP_;
        this.SP = MaxSP_;
    }
    public CharacterStatsClass()
    {
        this.HP = 0;
        this.DMG = 0;
        this.MaxSP = 0;
        this.SP = 0;
        this.MaxSP = 0;
    }
}
