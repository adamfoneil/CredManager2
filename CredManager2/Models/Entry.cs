using Postulate.Base;
using Postulate.Base.Attributes;
using Postulate.Base.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Threading.Tasks;

namespace CredManager2.Models
{
    [Identity(nameof(Id))]
    public class Entry : Record
    {     
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(100)]
        [Required]
        public string Url { get; set; }

        [MaxLength(50)]
        [Required]
        public string UserName { get; set; }

        [MaxLength(50)]
        [Required]
        public string Password { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime DateCreated { get; set; } = DateTime.Now;

        public DateTime? DateModified { get; set; }

        public int Id { get; set; }

        public override async Task BeforeSaveAsync(IDbConnection connection, SaveAction action, IUser user)
        {
            await base.BeforeSaveAsync(connection, action, user);

            this.BeforeSave(connection, action, user);
        }

        public override void BeforeSave(IDbConnection connection, SaveAction action, IUser user)
        {
            base.BeforeSave(connection, action, user);

            switch (action)
            {
                case SaveAction.Insert:
                    DateCreated = DateTime.Now;
                    break;

                case SaveAction.Update:
                    DateModified = DateTime.Now;
                    break;
            }
        }
    }
}
