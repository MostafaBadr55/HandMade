namespace HandMade.ViewModels.Address
{
    public class AddressResponseVM
    {
        public Guid Id { get; set; }
        public string Label { get; set; }
        public string DetailedAddress { get; set; }
        public bool IsDefault { get; set; }
    }
}
