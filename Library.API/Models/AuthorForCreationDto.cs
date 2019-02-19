using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Library.API.Models
{
    public class AuthorForCreationDto
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
        [XmlIgnore]
        public ICollection<BookForCreationDto> Books { get; set; }
    }
}
