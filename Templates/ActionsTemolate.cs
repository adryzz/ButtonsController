using System;


namespace CustomActions 
{
  public static class Actions 
  {
    public static void Initialize()
    {
      Console.WriteLine("uwu");
    }

    public static void ExecuteActions(byte code)
    {
      Console.WriteLine("Action" + code.ToString())
    }
    
    public static void Exit() 
    {
      Console.WriteLine("Exiting...")
    }
  }
}
