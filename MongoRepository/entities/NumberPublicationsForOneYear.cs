using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoRepository.entities
{
    public class NumberPublicationsForOneYear
    {
        public string year { get; set; }

        public int numberOfPublications { get; set; }
    }
}
