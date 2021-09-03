using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using static LostInCryptic.Shared.Enums.Enums;

namespace LostInCryptic.Shared.DbModels
{
    [Table("UrlCodes")]
    public class Word
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull]
        public string Value { get; set; }

    }
}
