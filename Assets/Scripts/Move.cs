public class Move
{
    private Bottle sourceBottle;
    private Bottle destinationBottle;
    private int    liquidCount;

    public Move( Bottle sourceBottle,
                 Bottle destinationBottle,
                 int liquidCount )
    {
        this.sourceBottle = sourceBottle;
        this.destinationBottle = destinationBottle;
        this.liquidCount = liquidCount;
    }

    public Bottle GetSourceBottle()
    {
        return sourceBottle;
    }

    public Bottle GetDestinationBottle()
    {
        return destinationBottle;
    }

    public int GetLiquidCount()
    {
        return liquidCount;
    }

    public void Undo()
    {
      destinationBottle.ResetCompleteBottle();
      destinationBottle.TransferLiquid( sourceBottle, true, liquidCount );
    }

    public override string ToString()
    {
        return this.GetType().Name + "["
               + "sourceBottle=" + sourceBottle + ","
               + "destinationBottle=" + destinationBottle + ","
               + "liquidCount=" + liquidCount + "]";
    }
}
