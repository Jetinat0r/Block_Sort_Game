using System;

public class Move
{
    public Bottle Source { get; private set; }

    public Bottle Destination { get; private set; }

    public int LiquidCount { get; private set; }

    public Move( Bottle source, Bottle destination, int liquidCount )
    {
        Source = source ?? throw new ArgumentNullException(nameof(source));
        Destination = destination ?? throw new ArgumentNullException(nameof(destination));
        LiquidCount = liquidCount;
    }

    public void Undo()
    {
      Destination.ResetCompleteBottle();
      Destination.TransferLiquid( Source, true, LiquidCount );
    }

    public void Redo()
    {
      Source.TransferLiquid( Destination, true, LiquidCount );
      Destination.CheckCompleted();
    }

    public override string ToString()
    {
        return this.GetType().Name + "["
               + "Source=" + Source + ","
               + "Destination=" + Destination + ","
               + "LiquidCount=" + LiquidCount + "]";
    }
}
