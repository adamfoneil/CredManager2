using CredManager2.Models;
using Postulate.Base;
using Postulate.Base.Attributes;

namespace CredManager2.Queries
{
    public class Entries : Query<Entry>
    {
        public Entries() : base("SELECT * FROM [Entry] WHERE [IsActive]=@isActive {andWhere} ORDER BY [Name]")
        {
        }

        public bool IsActive { get; set; }

        [Where("([Name] LIKE '%'+@search+'%' OR [Url] LIKE '%'+@search+'%')")]
        public string Search { get; set; }
    }
}
