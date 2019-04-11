using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Library.API.Models
{
    public class AuthorForCreationWithDateOfDeathDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Genre { get; set; }

        [XmlElement("DateOfBirth")]
        public string DateOfBirthForXml
        {
            get { return DateOfBirth.ToString("o"); }
            set { DateOfBirth = DateTimeOffset.Parse(value); }
        }

        [XmlIgnore]
        public DateTimeOffset DateOfBirth { get; set; }

        [XmlElement("DateOfDeath")]
        public string DateOfDeathForXml
        {
            get { return DateOfDeath?.ToString("o") ?? null; }
            set
            {
                if (value != null)
                    DateOfDeath = DateTimeOffset.Parse(value);
            }
        }

        [XmlIgnore]
        public DateTimeOffset? DateOfDeath { get; set; }
    }
}
