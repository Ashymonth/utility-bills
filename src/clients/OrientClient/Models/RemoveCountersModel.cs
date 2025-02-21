namespace OrientClient.Models;

/// <summary>
/// Remove counters model form sending in remove counters data request.
/// </summary>
internal class RemoveCountersModel
{
    public int Id { get; set; } 

    public string Key { get; set; }= null!;

    public int Traiff { get; set; }

    public int Value { get; set; }
}