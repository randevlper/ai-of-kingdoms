
//Default should be 1f for all
public struct Buffs
{
    public float knightAttackMult;
    public float knightHealthMult;
    public float knightAutoHealMult;

    public float serfHealthMult;
    public float serfGatherSpeedMult;
    public float serfSpeedMult;
    public float serdAutoHealMult;

	//Initilizes all varibles to 1f
    public Buffs(float f)
    {
        knightAttackMult = f;
        knightHealthMult = f;
        knightAutoHealMult = f;

        serfHealthMult = f;
        serfGatherSpeedMult = f;
        serfSpeedMult = f;
        serdAutoHealMult = f;
    }
}
