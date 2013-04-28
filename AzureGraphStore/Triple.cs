using System;

namespace Two10.AzureGraphStore
{
    public class Triple
    {
        public Triple(string subject, string property, string value)
        {
            if (string.IsNullOrWhiteSpace(subject)) throw new ArgumentNullException("subject");
            if (string.IsNullOrWhiteSpace(property)) throw new ArgumentNullException("property");
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentNullException("value");

            if (subject.Contains("~") || property.Contains("~") || value.Contains("~")) throw new ArgumentException("Triples may not contains the '~' character");

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
