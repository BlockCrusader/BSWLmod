using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace BSWLmod;

public class Config : ModConfig
{
	public override ConfigScope Mode => ConfigScope.ServerSide;

	[Header("HeaderXP")]

	[BackgroundColor(150, 150, 255)]
	[DefaultValue(true)]
	public bool xpGain;

	[BackgroundColor(150, 150, 255)]
	[Range(0.25f, 10f)]
	[Increment(.25f)]
	[DefaultValue(1f)]

	public float xpMultiplier;

	[BackgroundColor(150, 150, 255)]
	[Range(0.2f, 2f)]
	[Increment(.2f)]
	[DefaultValue(0.4f)]
	public float bossXP;

	[BackgroundColor(150, 150, 255)]
	[Range(0.2f, 1f)]
	[Increment(.2f)]
	[DefaultValue(0.6f)]
	public float wormXP;

	[BackgroundColor(150, 150, 255)]
	[Range(0.1f, 1f)]
	[Increment(.1f)]
	[DefaultValue(0.4f)]
	public float eventXP;

	[BackgroundColor(150, 150, 255)]
	[Range(0.25f, 3f)]
	[Increment(.25f)]
	[DefaultValue(1f)]
	public float orbMultiplier;

    [BackgroundColor(150, 150, 255)]
    [Range(0.5f, 2f)]
    [Increment(.25f)]
    [DefaultValue(1f)]
    public float effortMultiplier;

    [BackgroundColor(150, 150, 255)]
	[Range(100, 3250000)]
	[DefaultValue(1750000)]
	public int customBaseXP;

	[BackgroundColor(150, 150, 255)]
	[DefaultValue(true)]

	public bool playerXP;

	[Header("HeaderDMG")]

	[BackgroundColor(210, 30, 30)]
	[Range(0f, 2.5f)]
	[Increment(.1f)]
	[DefaultValue(1f)]
	public float dmgMultiplier;

	[BackgroundColor(210, 30, 30)]
	[Range(0f, 2.5f)]
	[Increment(.1f)]
	[DefaultValue(0f)]

	public float playerDamage;

	[Header("HeaderCaps")]

	[BackgroundColor(65, 120, 150)]
	[Range(1, 100)]
	[DefaultValue(100)]
	public int levelCap;

	[BackgroundColor(65, 120, 150)]
	[Range(1, 2000000000)]
	[DefaultValue(2000000000)]
	public int xpCap;

	[BackgroundColor(65, 120, 150)]
	[Range(1.1f, 3f)]
	[Increment(.1f)]
	[DefaultValue(3f)]
	public float dmgCap;

	[Header("HeaderText")]

	[BackgroundColor(200, 200, 200)]
	[DefaultValue(true)]
	public bool coloredDisplays;
	 
	[BackgroundColor(200, 200, 200)]
	[DefaultValue(false)]
	public bool showBaseXP;

	[BackgroundColor(200, 200, 200)]
	[DefaultValue(false)]
	public bool showDMG;

	[BackgroundColor(200, 200, 200)]
	[DefaultValue(false)]
	public bool showLvl100;

	[BackgroundColor(200, 200, 200)]
	[DefaultValue(false)]
	public bool showMaxXP;

	[BackgroundColor(200, 200, 200)]
	[DefaultValue(true)]
	public bool showMaxDMG;

	[BackgroundColor(200, 200, 200)]
	[DefaultValue(true)]
	public bool showDisabledLeveling;

	[BackgroundColor(200, 200, 200)]
	[DefaultValue(false)]
	public bool detailedDisabledLeveling;

	[Header("HeaderEtc")]

	[BackgroundColor(170, 165, 145)]
	[DefaultValue(true)]
	public bool dropOrbs;

    [BackgroundColor(170, 165, 145)]
    [Range(0f, 1f)]
    [Increment(.2f)]
    [DefaultValue(1f)]
    public float xpDust;

    [BackgroundColor(170, 165, 145)]
	[DefaultValue(true)]
	public bool lvlUpSound;

	[BackgroundColor(170, 165, 145)]
	[DefaultValue(true)]
	public bool lvlUpText;

	[BackgroundColor(170, 165, 145)]
	[DefaultValue(true)]
	public bool augmentCrossmod;
}
