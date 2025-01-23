namespace CS3750Assignment1.Models
{
    public class User
    {
        /// <summary>
        /// create the auto generated Id field for the database items
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// create the email variable for the database
        /// </summary>
        public string email {  get; set; }
        /// <summary>
        /// create the name variable for the database
        /// </summary>
        public string name { get; set; } = string.Empty;

        public string firstName { get; set; } = string.Empty;
        public string lastName { get; set; } = string.Empty;
        public string addressLine1 { get; set; } = string.Empty;
        public string addressLine2 { get; set; } = string.Empty;
        public string city { get; set; } = string.Empty;
        public string state { get; set; } = string.Empty;
        public string zip { get; set; } = string.Empty;
        public string phone { get; set; } = string.Empty;

        /// <summary>
        /// create the password variable for the database
        /// </summary>
        public string password { get; set; }
        /*
        public string
        */
        public DateTime BirthDate { get; set; }

        //public char userType { get; set; }
    }
}
