namespace ContactsBackendDotnet.Models
{
    public class Contact
    {
        public Guid Id { get; set; }= Guid.NewGuid();
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public Contact Clone() =>
            (Contact)MemberwiseClone();
    }
}