
namespace Two10.AzureGraphStore
{
    public class Triple
    {
        public Triple(string subject, string property, string value)
        {
            this.Subject = subject;
            this.Property = property;
            this.Value = value;
        }

        public string Subject { get; set; }
        public string Property { get; set; }
        public string Value { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1}'s {2}", this.Subject, this.Property, this.Value);
        }
    }
}
