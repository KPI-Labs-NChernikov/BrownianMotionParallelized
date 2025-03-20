namespace Cells1;

public static class Presenter
{
    public static void ShowCrystal(Crystal1D crystal)
    {
        var snapshot = crystal.CreateSnapshot();
        Console.Write("[");
        Console.Write(string.Join(", ", snapshot));
        Console.Write("]");
        Console.WriteLine($" Atoms Count: {snapshot.GetAtomsCount()}");
    }
}
