namespace Components
{
    public interface INumberProcessor
    {
        int Sum(int[] values);
        int Product(int[] values);
        int AbsoluteValue(int value);
    }
}