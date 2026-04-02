/// <summary>
/// The Identity: Dynamic item data, unique to each instance
/// </summary>
[System.Serializable]
public class ItemInstance
{
    public ItemDataSo DataSo { get; private set; }
    public int stackSize; // Tracks current amount in this instance
    
    //future properties will go here

    public ItemInstance(ItemDataSo itemDataSo, int amount = 1)
    {
        DataSo = itemDataSo;
        stackSize = amount;
    }
}
