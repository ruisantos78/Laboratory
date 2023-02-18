namespace RuiSantos.ZocDoc.Api.Contracts
{
    public struct DoctorsPostRequest
    {
        public string License { get; set; }
        public List<string> Specialities { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<string> ContactNumbers { get; set; }
    }
}
